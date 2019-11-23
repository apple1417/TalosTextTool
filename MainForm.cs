using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TalosTextTool {
  public partial class MainForm : Form {
    private class AssertFailedException : Exception {
      public AssertFailedException(string message) : base(message) { }
    }

    /*
      This is injected right before the function call that draws the text on screen
      It swaps out the stack contents to point to our data instead
      All 0xFFs are placeholders

      TODO: 64 bit version
    */
    private static readonly byte[] INJECTED_CODE = new byte[] {
      0xC7, 0x44, 0x24, 0x04,  0xFF, 0xFF, 0xFF, 0xFF,  // mov [esp+04],<custom string>
      0xC7, 0x44, 0x24, 0x0C,  0xFF, 0xFF, 0xFF, 0xFF,  // mov [esp+0C],<colour>
      0x50,                                             // push eax   // Probably don't need to save eax but just in case
      0x8B, 0x44, 0x24, 0x0C,                           // mov eax,[esp + 0C]
      0xC7, 0x00,              0xFF, 0xFF, 0xFF, 0xFF,  // mov[eax],<x>
      0xC7, 0x40, 0x04,        0xFF, 0xFF, 0xFF, 0xFF,  // mov[eax + 04],<y>
      0xC7, 0x40, 0x08,        0xFF, 0xFF, 0xFF, 0xFF,  // mov[eax + 08],<z>
      0x58,                                             // pop eax
      0xE8,                    0xFF, 0xFF, 0xFF, 0xFF,  // call <copied draw address>
      0xE9,                    0xFF, 0xFF, 0xFF, 0xFF   // jmp <return address>
      // Custom string goes here
    };
    private const int STRING_ADDR_OFFSET = 0x4;
    private const int COLOUR_OFFSET = 0xC;
    private const int X_OFFSET = 0x17;
    private const int Y_OFFSET = 0x1E;
    private const int Z_OFFSET = 0x25;
    private const int DRAW_OFFSET = 0x2B;
    private const int RETURN_OFFSET = 0x30;
    private const int STRING_DATA_OFFSET = 0x34;

    private const int ALLOC_AMOUNT = 512;

    private MemoryManager manager = null;
    private AddressFinder addr = null;
    private IntPtr injectedAddr = IntPtr.Zero;

    public MainForm() {
      InitializeComponent();

      colourDialog.Color = Color.White;
      textInput.MaxLength = ALLOC_AMOUNT - INJECTED_CODE.Length - 1;
    }

    private void ColourBox_Click(object sender, EventArgs e) {
      colourDialog.ShowDialog();

      Color newColour = Color.FromArgb((int) opacityInput.Value, colourDialog.Color);
      colourDialog.Color = newColour;
      colourBox.BackColor = newColour;
      colourBox.ForeColor = GetTextColourForBackground(newColour);
      colourBox.Text = $"{colourDialog.Color.ToArgb() & 0x00FFFFFF:X6}";
    }

    private void OpacityInput_ValueChanged(object sender, EventArgs e) {
      Color newColour = Color.FromArgb((int) opacityInput.Value, colourDialog.Color);
      colourDialog.Color = newColour;
      colourBox.BackColor = newColour;
      colourBox.ForeColor = GetTextColourForBackground(newColour);
    }

    // Taken from https://graphicdesign.stackexchange.com/a/77747
    private Color GetTextColourForBackground(Color background) {
      double gamma = 2.2;
      double newR = 0.2126 * Math.Pow(background.R, gamma);
      double newG = 0.7152 * Math.Pow(background.G, gamma);
      double newB = 0.0722 * Math.Pow(background.B, gamma);
      return (newR + newB + newG) > Math.Pow(128, gamma) ? Color.Black : Color.White;
    }

    private void InjectButton_Click(object sender, EventArgs e) {
      try {
        Inject();
      } catch (AssertFailedException ex) {
        MessageBox.Show(ex.Message, "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      } catch (Win32Exception ex) {
        MessageBox.Show($"A windows errors occured: {ex.Message}", "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void Assert(bool condition, string message) {
      if (!condition) {
        throw new AssertFailedException(message);
      }
    }

    private void Inject() {
      if (manager == null || !manager.IsHooked) {
        injectedAddr = IntPtr.Zero;

        Process game = Process.GetProcessesByName("Talos").FirstOrDefault();
        Assert(game != null, "Could not find game process to inject into!");

        manager = new MemoryManager(game);
        Assert(manager.IsHooked, "Could not find game process to inject into!");

        addr = new AddressFinder(manager);
        Assert(addr.FoundAddresses, "Could not find locations to inject into! Most likely you are not running a compatible game version.");
      }

      // Check if we already have jump call in the right location - if we injected then restarted the program
      byte[] call = manager.Read(addr.DrawCall, 5);
      if (call[0] == 0xE9) {
        injectedAddr = IntPtr.Add(addr.DrawCall, BitConverter.ToInt32(call, 1) + 5);
      }

      // If we haven't injected into this game yet
      if (injectedAddr == IntPtr.Zero) {
        injectedAddr = manager.Alloc(ALLOC_AMOUNT);

        // Setup the injected data to properly point to everything
        manager.Write(injectedAddr, INJECTED_CODE);

        manager.Write(
          IntPtr.Add(injectedAddr, STRING_ADDR_OFFSET),
          BitConverter.GetBytes(injectedAddr.ToInt32() + STRING_DATA_OFFSET)
        );

        // Unfortuantly all function calls/jumps use relative addresses, so we need to do a bunch of maths

        // Ignore the E8, read out the address
        int originalCallOffset = BitConverter.ToInt32(manager.Read(IntPtr.Add(addr.DrawCall, 1), 4), 0);
        int callActual = addr.DrawCall.ToInt32() + originalCallOffset + 5;
        int newCallOffset = callActual - (injectedAddr.ToInt32() + DRAW_OFFSET + 4);
        manager.Write(
          IntPtr.Add(injectedAddr, DRAW_OFFSET),
          BitConverter.GetBytes(newCallOffset)
        );

        int drawActual = addr.DrawCall.ToInt32() + 5;
        int returnOffset = drawActual - (injectedAddr.ToInt32() + RETURN_OFFSET + 4);
        manager.Write(
          IntPtr.Add(injectedAddr, RETURN_OFFSET),
          BitConverter.GetBytes(returnOffset)
        );

        // Inject the jump into the main process
        int jumpOffset = injectedAddr.ToInt32() - drawActual;
        manager.Write(
          addr.DrawCall,
          new byte[] {
            0xE9
          }.Concat(BitConverter.GetBytes(jumpOffset)).ToArray()
        );

        // Nop out the cvar check
        manager.Write(
          addr.SkipJump,
          new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
        );
      }

      // Update all vars that you're allowed to change
      manager.Write(
        IntPtr.Add(injectedAddr, COLOUR_OFFSET),
        BitConverter.GetBytes(colourDialog.Color.ToArgb())
      );

      manager.Write(
        IntPtr.Add(injectedAddr, X_OFFSET),
        BitConverter.GetBytes((float) xInput.Value)
      );
      manager.Write(
        IntPtr.Add(injectedAddr, Y_OFFSET),
        BitConverter.GetBytes((float) yInput.Value)
      );
      manager.Write(
        IntPtr.Add(injectedAddr, Z_OFFSET),
        BitConverter.GetBytes((float) zInput.Value)
      );

      manager.Write(
        IntPtr.Add(injectedAddr, STRING_DATA_OFFSET),
        Encoding.UTF8.GetBytes(textInput.Text + "\x00")
      );
    }
  }
}
