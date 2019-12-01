using System;

namespace TalosTextTool {
  public interface IAddressList {
    public bool FoundAddresses { get; }

    public IntPtr InjectLocation { get; }
    public int InjectInstructionLength { get; }

    public IntPtr DrawText { get; }
    public IntPtr DrawBox { get; }
    public IntPtr SetFont { get; }

    public IntPtr Font { get; }
    public IntPtr Viewport { get; }
  }
}
