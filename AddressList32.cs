using System;
using System.Diagnostics;
using MemTools;

namespace TalosTextTool {
  class AddressList32 : IAddressList {
    public bool FoundAddresses { get; private set; }

    public IntPtr InjectLocation { get; private set; }
    public int InjectInstructionLength { get; private set; }

    public IntPtr DrawText { get; private set; }
    public IntPtr DrawBox { get; private set; }
    public IntPtr SetFont { get; private set; }

    public IntPtr Font { get; private set; }
    public IntPtr Viewport { get; private set; }

    public AddressList32(MemManager manager) {
      FoundAddresses = false;
      ProcessModule exe = manager.HookedProcess.MainModule;

      InjectLocation = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 5,
        "E8 ????????",        // call Talos.exe + 63C640

                              // jmp 017F0000
        "??????????",         // OR
                              // mov ecx,[esi+08]
                              // test ecx, ecx

        "0F84 ????????",      // je Talos.exe + 641B8E
        "E8 ????????",        // call Talos.exe + 867970
        "85 C0",              // test eax, eax
        "0F84 ????????",      // je Talos.exe + 641AEC
        "83 3D ???????? 00"   // cmp dword ptr[Talos.exe + 118860C], 00

      );
      if (InjectLocation == IntPtr.Zero) {
        return;
      }
      InjectInstructionLength = 5;

      IntPtr tmp = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 6,
        "F3 0F11 45 CC",  // movss[ebp - 34], xmm0
        "E8 ????????",    // call Talos.exe + 83A330
        "83 C4 14"        // add esp, 14
      );
      if (tmp == IntPtr.Zero) {
        return;
      }
      DrawText = manager.ReadRelativePtr(tmp);

      tmp = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 1,
        "E8 ????????",    // call Talos.exe + 82FE20
        "8B 4D FC",       // mov ecx,[ebp - 04]
        "8B 15 ????????", // mov edx,[Talos.exe + 11E8A20]
        "0F57 C0",        // xorps xmm0, xmm0
        "83 C4 18"        // add esp, 18
      );
      if (tmp == IntPtr.Zero) {
        return;
      }
      DrawBox = manager.ReadRelativePtr(tmp);
      Viewport = manager.Read<IntPtr>(IntPtr.Add(tmp, 9));

      FoundAddresses = true;

      tmp = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 8,
        "83 C4 1C",       // add esp,1C
        "85 C0",          // test eax, eax
        "74 ??",          // je Talos.exe + 641B7E
        "68 ????????",    // push Talos.exe + 11D6B60
        "E8 ????????"     // call Talos.exe + 81F230
      );
      Font = manager.Read<IntPtr>(tmp);
      SetFont = manager.ReadRelativePtr(IntPtr.Add(tmp, 5));
    }
  }
}
