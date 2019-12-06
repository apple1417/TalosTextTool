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
              Vector3<float> pos = ParseXMLPos(child);
              xInput.Value = (decimal) pos.X;
              yInput.Value = (decimal) pos.Y;
              zInput.Value = (decimal) pos.Z;
              break;
            }
            case COLOUR: {
              SplitColour(colourInput, opacityInput, Color.FromArgb(int.Parse(
                child.Value,
                NumberStyles.HexNumber
              )));
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
              Vector3<float> pos = ParseXMLPos(child);
              xInputBox.Value = (decimal) pos.X;
              yInputBox.Value = (decimal) pos.Y;
              zInputBox.Value = (decimal) pos.Z;
              break;
            }
            case POS_MAX: {
              Vector3<float> pos = ParseXMLPos(child);
              x2InputBox.Value = (decimal) pos.X;
              y2InputBox.Value = (decimal) pos.Y;
              z2InputBox.Value = (decimal) pos.Z;
              break;
            }
            case COLOUR: {
              SplitColour(colourInputBox, opacityInputBox, Color.FromArgb(int.Parse(
                child.Value,
                NumberStyles.HexNumber
              )));
              break;
            }
          }
        }
      
      // If any exceptions occur we'll just leave what we managed to parse
      } catch (FileNotFoundException) {
      } catch (FormatException) {
      } catch (XmlException) { }
    }

    private Vector3<float> ParseXMLPos(XElement pos) {
      Vector3<float> output = new Vector3<float>();

      XElement child = pos.Elements(X).FirstOrDefault();
      if (child == null) {
        throw new FormatException();
      }
      output.X = float.Parse(child.Value);

      child = pos.Elements(Y).FirstOrDefault();
      if (child == null) {
        throw new FormatException();
      }
      output.Y = float.Parse(child.Value);

      child = pos.Elements(Z).FirstOrDefault();
      if (child == null) {
        throw new FormatException();
      }
      output.Z = float.Parse(child.Value);

      return output;
    }

    private void SaveSettings() {
      using (XmlWriter writer = XmlWriter.Create(SETTINGS_FILE, new XmlWriterSettings() {
        Indent = true,
        NewLineOnAttributes = true
      })) {
        writer.WriteStartDocument();
        writer.WriteStartElement(SETTINGS_HEADER);

        writer.WriteElementString(AUTO_INJECT, autoInject.Checked.ToString());

        writer.WriteStartElement(TEXT_HEADER);

        writer.WriteStartElement(POS);
        writer.WriteElementString(X, xInput.Value.ToString());
        writer.WriteElementString(Y, yInput.Value.ToString());
        writer.WriteElementString(Z, zInput.Value.ToString());
        writer.WriteEndElement();

        int textColour = colourInput.BackColor.ToArgb() & 0x00FFFFFF;
        textColour |= (int) opacityInput.Value << 24;
        writer.WriteElementString(COLOUR, textColour.ToString("X8"));

        writer.WriteElementString(VALUE, textInput.Text);

        writer.WriteEndElement();
        writer.WriteStartElement(BOX_HEADER);

        writer.WriteStartElement(POS_MIN);
        writer.WriteElementString(X, xInputBox.Value.ToString());
        writer.WriteElementString(Y, yInputBox.Value.ToString());
        writer.WriteElementString(Z, zInputBox.Value.ToString());
        writer.WriteEndElement();

        writer.WriteStartElement(POS_MAX);
        writer.WriteElementString(X, x2InputBox.Value.ToString());
        writer.WriteElementString(Y, y2InputBox.Value.ToString());
        writer.WriteElementString(Z, z2InputBox.Value.ToString());
        writer.WriteEndElement();

        int boxColour = colourInputBox.BackColor.ToArgb() & 0x00FFFFFF;
        boxColour |= (int) opacityInputBox.Value << 24;
        writer.WriteElementString(COLOUR, boxColour.ToString("X8"));

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }
  }
}
