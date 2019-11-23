using System;
using System.Diagnostics;

namespace TalosTextTool {
  class AddressFinder {
    public bool FoundAddresses { get; private set; }

    public IntPtr SkipJump { get; private set; } // The address of the jump that skips drawing the text, right after comparing against the cvar
    public IntPtr DrawCall { get; private set; } // The address of the call to the function that draws the text.

    private const int CMP_JUMP_OFFSET_244 = 0x00641AF3;
    private const int DRAW_CALL_OFFSET_244 = 0x00641B76;

    public AddressFinder(MemoryManager manager) {
      // TODO: Make this work with mnore than just 244
      ProcessModule exe = manager.HookedProcess.MainModule;
      FoundAddresses = exe.ModuleMemorySize == 0x012b1000;
      if (!FoundAddresses) {
        return;
      }

      SkipJump = IntPtr.Add(manager.HookedProcess.MainModule.BaseAddress, CMP_JUMP_OFFSET_244);
      DrawCall = IntPtr.Add(manager.HookedProcess.MainModule.BaseAddress, DRAW_CALL_OFFSET_244);
    }
  }
}
