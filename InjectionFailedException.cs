using System;

namespace TalosTextTool {
  public class InjectionFailedException : Exception {
    public InjectionFailedException(string message) : base(message) { }
  }
}
