using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TalosTextTool {
  public partial class MainForm : Form {
    private const string SETTINGS_FILE = "TextToolSettings.xml";

    private const string SETTINGS_HEADER = "Settings";
    private const string AUTO_INJECT = "AutoInject";
    private const string TEXT_HEADER = "Text";
    private const string BOX_HEADER = "Background";

    private const string POS = "Pos";
    private const string POS_MIN = "PosMin";
    private const string POS_MAX = "PosMax";

    private const string X = "X";
    private const string Y = "Y";
    private const string Z = "Z";

    private const string COLOUR = "Colour";
    private const string VALUE = "Value";

    private void LoadSettings() {
      try {
        XElement root = XElement.Load(SETTINGS_FILE);
        XElement auto = root.Descendants(AUTO_INJECT).FirstOrDefault();
        XElement text = root.Descendants(TEXT_HEADER).FirstOrDefault();
        XElement box = root.Descendants(BOX_HEADER).FirstOrDefault();

        if (auto == null || text == null || box == null) {
          throw new FormatException();
        }

        autoInject.Checked = bool.Parse(auto.Value);

        foreach (XElement child in text.Elements()) {
          switch (child.Name.ToString()) {
            case POS: {
              Vector3F pos = ParseVector3F(child);
              xInput.Value = (decimal) pos.X;
              yInput.Value = (decimal) pos.Y;
              zInput.Value = (decimal) pos.Z;
              break;
            }
            case COLOUR: {
              SplitColour(colourInput, opacityInput, Color.FromArgb(
                int.Parse(child.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)
              ));
              break;
            }
            case VALUE: {
              textInput.Text = child.Value;
              break;
            }
          }
        }

        foreach (XElement child in box.Elements()) {
          switch (child.Name.ToString()) {
            case POS_MIN: {
              Vector3F pos = ParseVector3F(child);
              xInputBox.Value = (decimal) pos.X;
              yInputBox.Value = (decimal) pos.Y;
              zInputBox.Value = (decimal) pos.Z;
              break;
            }
            case POS_MAX: {
              Vector3F pos = ParseVector3F(child);
              x2InputBox.Value = (decimal) pos.X;
              y2InputBox.Value = (decimal) pos.Y;
              z2InputBox.Value = (decimal) pos.Z;
              break;
            }
            case COLOUR: {
              SplitColour(colourInputBox, opacityInputBox, Color.FromArgb(
                int.Parse(child.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)
              ));
              break;
            }
          }
        }
      
      // If any exceptions occur we'll just leave what we managed to parse
      } catch (FileNotFoundException) {
      } catch (FormatException) {
      } catch (XmlException) { }
    }

    private Vector3F ParseVector3F(XElement pos) {
      Vector3F output = new Vector3F();

      XElement child = pos.Elements(X).FirstOrDefault();
      if (child == null) {
        throw new FormatException();
      }
      output.X = float.Parse(child.Value, CultureInfo.InvariantCulture);

      child = pos.Elements(Y).FirstOrDefault();
      if (child == null) {
        throw new FormatException();
      }
      output.Y = float.Parse(child.Value, CultureInfo.InvariantCulture);

      child = pos.Elements(Z).FirstOrDefault();
      if (child == null) {
        throw new FormatException();
      }
      output.Z = float.Parse(child.Value, CultureInfo.InvariantCulture);

      return output;
    }

    private void SaveSettings() {
      using (XmlWriter writer = XmlWriter.Create(SETTINGS_FILE, new XmlWriterSettings() {
        Indent = true,
        NewLineOnAttributes = true
      })) {
        writer.WriteStartDocument();
        writer.WriteStartElement(SETTINGS_HEADER);

        writer.WriteElementString(AUTO_INJECT, autoInject.Checked.ToString(CultureInfo.InvariantCulture));

        writer.WriteStartElement(TEXT_HEADER);

        writer.WriteStartElement(POS);
        writer.WriteElementString(X, xInput.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElementString(Y, yInput.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElementString(Z, zInput.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndElement();

        int textColour = colourInput.BackColor.ToArgb() & 0x00FFFFFF;
        textColour |= (int) opacityInput.Value << 24;
        writer.WriteElementString(COLOUR, textColour.ToString("X8", CultureInfo.InvariantCulture));

        writer.WriteElementString(VALUE, textInput.Text);

        writer.WriteEndElement();
        writer.WriteStartElement(BOX_HEADER);

        writer.WriteStartElement(POS_MIN);
        writer.WriteElementString(X, xInputBox.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElementString(Y, yInputBox.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElementString(Z, zInputBox.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndElement();

        writer.WriteStartElement(POS_MAX);
        writer.WriteElementString(X, x2InputBox.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElementString(Y, y2InputBox.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElementString(Z, z2InputBox.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndElement();

        int boxColour = colourInputBox.BackColor.ToArgb() & 0x00FFFFFF;
        boxColour |= (int) opacityInputBox.Value << 24;
        writer.WriteElementString(COLOUR, boxColour.ToString("X8", CultureInfo.InvariantCulture));

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }
  }
}
