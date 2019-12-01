using MemTools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TalosTextTool {
  public partial class MainForm : Form {
    private ITextInjection injection = null;

    public MainForm() {
      InitializeComponent();
    }


    private void MainForm_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        e.SuppressKeyPress = true;
        InjectButton_Click(sender, e);
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
          Process game = Process.GetProcessesByName("Talos").FirstOrDefault();
          if (game == null) {
            throw new InjectionFailedException("Could not find game process to inject into!");
          }

          MemManager manager = new MemManager(game);
          if (!manager.IsHooked) {
            throw new InjectionFailedException("Could not find game process to inject into!");
          }

          // TODO: 64 bit version
          injection = new TextInjection32(manager);
        }

        if (!injection.IsInjected) {
          injection.Inject();
        }

        injection.Text = textInput.Text;
        injection.TextColour = Color.FromArgb((int) opacityInput.Value, colourInput.BackColor);

        injection.TextPos = new Vector3<float> {
          X = (float) xInput.Value,
          Y = (float) yInput.Value,
          Z = (float) zInput.Value
        };

        injection.BoxColour = Color.FromArgb((int) opacityInputBox.Value, colourInputBox.BackColor);
        injection.BoxPosMin = new Vector3<float> {
          X = (float) xInputBox.Value,
          Y = (float) yInputBox.Value,
          Z = (float) zInputBox.Value
        };
        injection.BoxPosMax = new Vector3<float> {
          X = (float) x2InputBox.Value,
          Y = (float) y2InputBox.Value,
          Z = (float) z2InputBox.Value
        };

      } catch (InjectionFailedException ex) {
        MessageBox.Show(ex.Message, "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
// Let these bubble up while debugging
#if !DEBUG
      } catch (Win32Exception ex) {
        MessageBox.Show($"A windows errors occured: {ex.Message}", "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
      }
    }
  }
}
