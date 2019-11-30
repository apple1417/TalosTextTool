using System;
using System.Diagnostics;
using MemTools;

namespace TalosTextTool {
  class AddressFinder {
    public bool FoundAddresses { get; private set; }

    public IntPtr CvarCheck { get; private set; } // The address of the jump that skips the show level block, right after comparing against the cvar.
    public IntPtr DrawTextCall { get; private set; } // The address of the call to the function that draws the text, which we're overwriting.
    public IntPtr DrawBox { get; private set; } // The address of the function that draws the background of the cheats box.
    public IntPtr Viewport { get; private set; } // The address containing the viewport to draw to.

    private const int CVAR_CHECK_244 = 0x00641AF3;
    private const int DRAW_TEXT_CALL_244 = 0x00641B76;
    private const int DRAW_BOX_244 = 0x082FE20;
    private const int VIEWPORT_244 = 0x011E8A20;

    public AddressFinder(MemManager manager) {
      // TODO: Make this work with ore than just 244
      ProcessModule exe = manager.HookedProcess.MainModule;
      FoundAddresses = exe.ModuleMemorySize == 0x012b1000;
      if (!FoundAddresses) {
        return;
      }

      CvarCheck = IntPtr.Add(exe.BaseAddress, CVAR_CHECK_244);
      DrawTextCall = IntPtr.Add(exe.BaseAddress, DRAW_TEXT_CALL_244);
      DrawBox = IntPtr.Add(exe.BaseAddress, DRAW_BOX_244);
      Viewport = IntPtr.Add(exe.BaseAddress, VIEWPORT_244);
    }
  }
}
