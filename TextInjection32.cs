using MemTools;
using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TalosTextTool {
  class TextInjection32 : ITextInjection {
    private readonly MemManager manager;
    private readonly AddressFinder addr;
    private IntPtr injectedAddr;

    public bool IsHooked { get { return manager.IsHooked; } }
    public bool IsInjected {
      get {
        if (!IsHooked) {
          return false;
        }
        if (injectedAddr != IntPtr.Zero) {
          return true;
        }

        // If we already read a jump call in the inject location
        if (manager.Read(addr.DrawTextCall, 1)[0] == 0xE9) {
          injectedAddr = manager.ReadOffset(IntPtr.Add(addr.DrawTextCall, 1));
          return true;
        }
        return false;
      }
    }

    public TextInjection32(MemManager manager) {
      this.manager = manager;

      addr = new AddressFinder(manager);
      if (!addr.FoundAddresses) {
        throw new InjectionFailedException("Could not find locations to inject into! Most likely you are not running a compatible game version.");
      }
    }

    // TODO: Replace the variables here reasonable defaults + 0xFF for pointers
    private static readonly byte[] INJECTED_CODE = new byte[] {
      0x68,                     0x00, 0x00, 0x00, 0xB0,   // push <box colour>
      0x68,                     0xFF, 0xFF, 0xFF, 0xFF,   // push <box pos>
      0xFF, 0x35,               0xFF, 0xFF, 0xFF, 0xFF,   // push [<viewport>]
      0xE8,                     0xFF, 0xFF, 0xFF, 0xFF,   // call <DrawBox>
      0x83, 0xC4, 0x0C,                                   // add esp,0C
      0xC7, 0x44, 0x24, 0x04,   0xFF, 0xFF, 0xFF, 0xFF,   // mov [esp+04],<text>
      0xC7, 0x44, 0x24, 0x08,   0xFF, 0xFF, 0xFF, 0xFF,   // mov [esp+08],<text pos>
      0xC7, 0x44, 0x24, 0x0C,   0xFF, 0xFF, 0xFF, 0xFF,   // mov [esp+0C],<text colour>
      0xE8,                     0xFF, 0xFF, 0xFF, 0xFF,   // call <DrawText>
      0xE9,                     0xFF, 0xFF, 0xFF, 0xFF,   // jmp <return addr>
      // Text Pos:
      0x00, 0xC0, 0x8A, 0x44,
      0x00, 0x00, 0xF0, 0x41,
      0x00, 0x00, 0x00, 0x00,
      // Box Pos:
      0x00, 0x00, 0x8C, 0x44,
      0x00, 0x00, 0xA0, 0x41,
      0x00, 0x00, 0x00, 0x00,
      0x00, 0x80, 0x9D, 0x44,
      0x00, 0x00, 0x82, 0x42,
      0x00, 0x00, 0x00, 0x00,
      // Text:
      0x00
    };

    private const int BOX_COLOUR =     0x1;
    private const int BOX_POS =        0x6;
    private const int VIEWPORT =       0xC;
    private const int DRAW_BOX =      0x11;
    private const int TEXT_ADDR =     0x1C;
    private const int TEXT_POS =      0x24;
    private const int TEXT_COLOUR =   0x2C;
    private const int DRAW_TEXT =     0x31;
    private const int RETURN =        0x36;

    private const int TEXT_X =        0x3A;
    private const int TEXT_Y =        0x3E;
    private const int TEXT_Z =        0x42;

    private const int BOX_X1 =        0x46;
    private const int BOX_Y1 =        0x4A;
    private const int BOX_Z1 =        0x4E;

    private const int BOX_X2 =        0x52;
    private const int BOX_Y2 =        0x56;
    private const int BOX_Z2 =        0x5A;

    private const int TEXT =          0x5E;

    private const int ALLOC_AMOUNT =   512;

    // INJECTED_CODE already includes the null byte we need on the string
    public static int MaxTextLength { get { return ALLOC_AMOUNT - INJECTED_CODE.Length; } }

    public void Inject() {
      if (IsInjected) {
        return;
      }

      injectedAddr = manager.Alloc(ALLOC_AMOUNT);

      manager.Write(injectedAddr, INJECTED_CODE);

      // Fill in the three which point to our own block of code
      manager.WriteInt32(
        IntPtr.Add(injectedAddr, TEXT_ADDR),
        injectedAddr.ToInt32() + TEXT
      );

      manager.WriteInt32(
        IntPtr.Add(injectedAddr, TEXT_POS),
        injectedAddr.ToInt32() + TEXT_X
      );

      manager.WriteInt32(
        IntPtr.Add(injectedAddr, BOX_POS),
        injectedAddr.ToInt32() + BOX_X1
      );

      // Viewport pushes an address so it's absolute
      manager.WriteInt32(
        IntPtr.Add(injectedAddr, VIEWPORT),
        addr.Viewport.ToInt32()
      );

      // Deal with the jumps/calls, which use relative addresses

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, DRAW_TEXT),
        // Ignore the E8, read out the address
        manager.ReadOffset(IntPtr.Add(addr.DrawTextCall, 1))
      );

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, DRAW_BOX),
        addr.DrawBox
      );

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, RETURN),
        // Return after the draw call
        IntPtr.Add(addr.DrawTextCall, 5)
      );

      // Inject the jump into the main process
      // We have to do this in one step - if we used two the game might take the jump/call between the steps and crash
      // This means we have to manually work out the offset
      int jumpOffset = injectedAddr.ToInt32() - (addr.DrawTextCall.ToInt32() + 5);
      manager.Write(
        addr.DrawTextCall,
        new byte[] {
            0xE9
        }.Concat(BitConverter.GetBytes(jumpOffset)).ToArray()
      );

      // Nop out the cvar check
      manager.Write(
        addr.CvarCheck,
        new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
      );
    }

    public Color TextColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }
        manager.WriteInt32(
          IntPtr.Add(injectedAddr, TEXT_COLOUR),
          value.ToArgb()
        );
      }
    }

    public Vector3<float> TextPos {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }

        manager.WriteFloat(
          IntPtr.Add(injectedAddr, TEXT_X),
          value.X
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, TEXT_Y),
          value.Y
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, TEXT_Z),
          value.Z
        );
      }
    }

    public string Text {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }
        if (value.Length > MaxTextLength) {
          throw new InjectionFailedException("Provided string is too long!");
        }
        manager.WriteUtf8(
          IntPtr.Add(injectedAddr, TEXT),
          value
        );
      }
    }

    public Color BoxColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }
        manager.WriteInt32(
          IntPtr.Add(injectedAddr, BOX_COLOUR),
          value.ToArgb()
        );
      }
    }

    public Vector3<float> BoxPosMin {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }

        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_X1),
          value.X
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Y1),
          value.Y
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Z1),
          value.Z
        );
      }
    }

    public Vector3<float> BoxPosMax {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }

        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_X2),
          value.X
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Y2),
          value.Y
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Z2),
          value.Z
        );
      }
    }
  }
}
