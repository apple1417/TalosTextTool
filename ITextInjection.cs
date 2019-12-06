using System;
using System.Drawing;

namespace TalosTextTool {
  public struct Vector3<T> {
    public T X;
    public T Y;
    public T Z;
  }

  public class InjectionFailedException : Exception {
    public InjectionFailedException(string message) : base(message) { }
  }

  public interface ITextInjection {
    public void Inject();

    public bool IsHooked { get; }
    public bool IsInjected { get; }

    public Color TextColour { get;  set; }
    public Vector3<float> TextPos { get; set; }
    public string Text { get; set; }

    public Color BoxColour { get; set; }
    public Vector3<float> BoxPosMin { get; set; }
    public Vector3<float> BoxPosMax { get; set; }
  }
}
