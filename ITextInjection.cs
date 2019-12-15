using System.Drawing;

namespace TalosTextTool {
  public interface ITextInjection {
    public void Inject();

    public bool IsHooked { get; }
    public bool IsInjected { get; }

    public Color TextColour { get;  set; }
    public Vector3F TextPos { get; set; }
    public string Text { get; set; }

    public Color BoxColour { get; set; }
    public Vector3F BoxPosMin { get; set; }
    public Vector3F BoxPosMax { get; set; }
  }
}
