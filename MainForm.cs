using MemTools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace TalosTextTool {
  public partial class MainForm : Form {
    private ATextInjection injection = null;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0069:Disposable fields should be disposed", Justification = "Is actually disposed in MainForm_FormClosing()")]
    private readonly ManagementEventWatcher watcher;

    public MainForm() {
      InitializeComponent();

      watcher = new ManagementEventWatcher(new WqlEventQuery(
        "SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'"
      ));
      watcher.EventArrived += new EventArrivedEventHandler(ProcessStart);
      watcher.Start();

      LoadSettings();

      try {
        HookGame();
      } catch (InjectionFailedException) {
        return;
      }

      // If we're already injected (tool was restarted, game wasn't) then overwrite the settings
      //  with what's currently injected
      if (injection.IsInjected) {
        textInput.Text = injection.Text;

        SplitColour(colourInput, opacityInput, injection.TextColour);

        Vector3F textPos = injection.TextPos;
        xInput.Value = (decimal) textPos.X;
        yInput.Value = (decimal) textPos.Y;
        zInput.Value = (decimal) textPos.Z;

        SplitColour(colourInputBox, opacityInputBox, injection.BoxColour);

        Vector3F boxPosMin = injection.BoxPosMin;
        xInputBox.Value = (decimal) boxPosMin.X;
        yInputBox.Value = (decimal) boxPosMin.Y;
        zInputBox.Value = (decimal) boxPosMin.Z;

        Vector3F boxPosMax = injection.BoxPosMax;
        x2InputBox.Value = (decimal) boxPosMax.X;
        y2InputBox.Value = (decimal) boxPosMax.Y;
        z2InputBox.Value = (decimal) boxPosMax.Z;

      // Inject if we're not already injected, but the game is running and autoinject is enabled
      // This makes it so starting the tool after the game automatically injects
      } else if (injection.IsHooked && autoInject.Checked) {
        InjectButton_Click(null, null);
      }
    }

    public void ProcessStart(object sender, EventArrivedEventArgs e) {
      if (!autoInject.Checked) {
        return;
      }
      if (e.NewEvent.ClassPath.ClassName != "__InstanceCreationEvent") {
        return;
      }

      string procName = (string) ((ManagementBaseObject) e.NewEvent.Properties["TargetInstance"].Value).Properties["Name"].Value;

      if (procName == "Talos.exe" || procName == "Talos_Unrestricted.exe") {
        HookGame();
        Thread.Sleep(15000);
        // If the game didn't properly start then don't try inject (which creates an error message)
        Process talos = Process.GetProcessesByName("Talos").FirstOrDefault();
        Process moddable = Process.GetProcessesByName("Talos_Unrestricted").FirstOrDefault();
        if (talos != null || moddable != null) {
          InjectButton_Click(null, null);
        }
      }
    }

    private void HookGame() {
      Process game = Process.GetProcessesByName("Talos").FirstOrDefault();
      if (game == null) {
        game = Process.GetProcessesByName("Talos_Unrestricted").FirstOrDefault();
        if (game == null) {
          throw new InjectionFailedException("Could not find game process to inject into!");
        }
      }

      MemManager manager = new MemManager(game);
      if (!manager.IsHooked) {
        throw new InjectionFailedException("Could not find game process to inject into!");
      }

      // TODO: Proper way of selecting addresses
      if (manager.Is64Bit) {
        injection = new TextInjection64(manager, new AddressList_440_64(manager));
      } else {
        injection = new TextInjection32(manager, new AddressList_244_32(manager));
      }
    }

    private void SplitColour(Label display, NumericUpDown opacity, Color colour) {
      opacity.Value = colour.A;

      Color opaque = Color.FromArgb((int) (colour.ToArgb() | 0xFF000000));
      display.BackColor = opaque;
      display.ForeColor = GetTextColourForBackground(opaque);
      display.Text = $"{colour.ToArgb() & 0x00FFFFFF:X6}";
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      watcher.Dispose();
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        e.SuppressKeyPress = true;
        InjectButton_Click(null, null);
      }
    }

    private void ColourInput_Click(object sender, EventArgs e) {
      colourDialog.Color = colourInput.BackColor;
      colourDialog.ShowDialog();

      colourInput.BackColor = colourDialog.Color;
      colourInput.ForeColor = GetTextColourForBackground(colourDialog.Color);
      colourInput.Text = $"{colourDialog.Color.ToArgb() & 0x00FFFFFF:X6}";
    }

    private void ColourInputBox_Click(object sender, EventArgs e) {
      colourDialog.Color = colourInputBox.BackColor;
      colourDialog.ShowDialog();

      colourInputBox.BackColor = colourDialog.Color;
      colourInputBox.ForeColor = GetTextColourForBackground(colourDialog.Color);
      colourInputBox.Text = $"{colourDialog.Color.ToArgb() & 0x00FFFFFF:X6}";
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
        if (injection == null || !injection.IsHooked) {
          HookGame();
        }

        if (!injection.IsInjected) {
          injection.Inject();
        }

        injection.Text = textInput.Text;
        injection.TextColour = Color.FromArgb((int) opacityInput.Value, colourInput.BackColor);

        injection.TextPos = new Vector3F {
          X = (float) xInput.Value,
          Y = (float) yInput.Value,
          Z = (float) zInput.Value
        };

        injection.BoxColour = Color.FromArgb((int) opacityInputBox.Value, colourInputBox.BackColor);
        injection.BoxPosMin = new Vector3F {
          X = (float) xInputBox.Value,
          Y = (float) yInputBox.Value,
          Z = (float) zInputBox.Value
        };
        injection.BoxPosMax = new Vector3F {
          X = (float) x2InputBox.Value,
          Y = (float) y2InputBox.Value,
          Z = (float) z2InputBox.Value
        };

        SaveSettings();

      } catch (InjectionFailedException ex) {
        MessageBox.Show(ex.Message, "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
// Let these bubble up while debugging
#if !DEBUG
      } catch (System.ComponentModel.Win32Exception ex) {
        MessageBox.Show($"A windows error occured: {ex.Message}", "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
      }
    }
  }
}
