using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TalosTextTool {
  public partial class MainForm : Form {
    private ITextInjection injection = null;

    public MainForm() {
      InitializeComponent();

      colourDialog.Color = Color.White;

      // This will be overwritten later
      textInput.MaxLength = 256;
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
        if (injection == null || !injection.IsHooked) {
          Process game = Process.GetProcessesByName("Talos").FirstOrDefault();
          if (game == null) {
            throw new InjectionFailedException("Could not find game process to inject into!");
          }

          MemoryManager manager = new MemoryManager(game);
          if (!manager.IsHooked) {
            throw new InjectionFailedException("Could not find game process to inject into!");
          }

          injection = new TextInjection32(manager);
        }

        if (!injection.IsInjected) {
          injection.Inject();
        }

        injection.Text = textInput.Text;
        injection.TextColour = colourDialog.Color;

        Vector3<float> pos = new Vector3<float>();
        pos.X = (float) xInput.Value;
        pos.Y = (float) yInput.Value;
        pos.Z = (float) zInput.Value;
        injection.TextPos = pos;
        
        // TODO: Plug in the remaining paramaters
        
      } catch (InjectionFailedException ex) {
        MessageBox.Show(ex.Message, "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      } catch (Win32Exception ex) {
        MessageBox.Show($"A windows errors occured: {ex.Message}", "Inject Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }
  }
}
