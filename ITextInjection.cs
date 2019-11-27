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
    public int MaxTextLength { get; }

    public Color TextColour { set; }
    public Vector3<float> TextPos { set; }
    public string Text { set; }

    public Color BoxColour { set; }
    public Vector3<float> BoxPosMin { set; }
    public Vector3<float> BoxPosMax { set; }
  }
}
