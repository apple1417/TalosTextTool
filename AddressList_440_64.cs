using System;
using System.Diagnostics;
using MemTools;

namespace TalosTextTool {
  class AddressList_440_64 : IAddressList {
    public bool FoundAddresses { get; private set; }

    public IntPtr InjectLocation { get; private set; }
    public int InjectInstructionLength { get; private set; }

    public IntPtr DrawText { get; private set; }
    public IntPtr DrawBox { get; private set; }
    public IntPtr SetFont { get; private set; }

    public IntPtr Font { get; private set; }
    public IntPtr Viewport { get; private set; }

    public AddressList_440_64(MemManager manager) {
      FoundAddresses = false;
      ProcessModule exe = manager.HookedProcess.MainModule;

      InjectLocation = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 8,
        "49 8B CE",           // mov rcx, r14
        "E8 ????????",        // call Talos.exe + 69C50

                              // jmp 13FFF0000
        "??????????????",     // OR
                              // mov rcx,[r14 + 10]
                              // test rcx, rcx

        "0F84 ????????",      // je Talos.exe + 688CC
        "83 3D ???????? 00",  // cmp dword ptr [Talos.exe + 1E015B8], 00
        "0F84 ????????"       // je Talos.exe + 688CC
      );
      if (InjectLocation == IntPtr.Zero) {
        return;
      }
      InjectInstructionLength = 7;

      IntPtr tmp = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 15,
        "F3 0F11 4C 24 44",       // movss[rsp + 44], xmm1
        "C7 44 24 48 00000000",   // mov[rsp + 48], 00000000
        "E8 ????????",            // call Talos.exe + A4AD80
        "48 8D 4C 24 50"          // lea rcx,[rsp+50]
      );
      if (tmp == IntPtr.Zero) {
        return;
      }
      DrawText = manager.ReadDisplacement(tmp, false);

      tmp = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 1,
        "E8 ????????",            // call Talos.exe + A47C60
        "48 8B 94 24 88000000",   // mov rdx, [rsp + 00000088]
        "48 8B 0D ????????"       // mov rcx, [Talos.exe + 1E44500]
      );
      if (tmp == IntPtr.Zero) {
        return;
      }
      DrawBox = manager.ReadDisplacement(tmp, false);
      Viewport = manager.ReadDisplacement(IntPtr.Add(tmp, 15), false);

      tmp = manager.SigScan(exe.BaseAddress, exe.ModuleMemorySize, 7,
        "85 C0",              // test eax, eax
        "74 4A",              // je Talos.exe + 688B8
        "48 8D 0D ????????",  // lea rcx, [Talos.exe + 1E017C0]
        "E8 ????????",        // call Talos.exe + A1AE80
        "F3 0F10 05 ????????" // movss xmm0, [Talos.exe + 12EDAA8]
      );
      if (tmp == IntPtr.Zero) {
        return;
      }
      Font = manager.ReadDisplacement(tmp, false);
      SetFont = manager.ReadDisplacement(IntPtr.Add(tmp, 5), false);

      FoundAddresses = true;
    }
  }
}
