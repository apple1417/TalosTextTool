using MemTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TalosTextTool {
  class TextInjection64 : ATextInjection {
    public TextInjection64(MemManager manager, IAddressList addr) : base(manager, addr) { }

    private static readonly byte[] INJECTED_CODE_PRE = new byte[] {
      0x83, 0x3D,         0xFF, 0xFF, 0xFF, 0xFF,   0x00,                 // cmp dword ptr [<viewport>], 00
      0x0F, 0x84, 0x65, 0x00, 0x00, 0x00,                                 // je <Copied overwritten instructions>
      0x51,                                                               // push rcx
      0x52,                                                               // push rdx
      0x41, 0x50,                                                         // push r8
      0x41, 0x51,                                                         // push r9
      0x48, 0xB9,         0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // mov rcx, <font>
      0xE8,               0xFF, 0xFF, 0xFF, 0xFF,                         // call <SetFont>
      0x48, 0x8B, 0x0D,   0xFF, 0xFF, 0xFF, 0xFF,                         // mov rcx, [<viewport>]
      0x48, 0xBA,         0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // mov rdx, <box pos>
      0x41, 0xB8,         0x00, 0x00, 0x00, 0xB0,                         // mov r8d, <box colour>
      0xE8,               0xFF, 0xFF, 0xFF, 0xFF,                         // call <DrawBox>
      0x48, 0x8B, 0x0D,   0xFF, 0xFF, 0xFF, 0xFF,                         // mov rcx, [<viewport>]
      0x48, 0xBA,         0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // mov rdx, <text>
      0x49, 0xB8,         0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // mov r8, <text pos>
      0x41, 0xB9,         0xFF, 0xFF, 0xFF, 0xFF,                         // mov r9d, <text colour>
      0x48, 0x83, 0xEC, 0x20,                                             // sub rsp, 20        // For some reason DrawText modifies rsp+8 and rsp+18, where
      0xE8,               0xFF, 0xFF, 0xFF, 0xFF,                         // call <DrawText>    //  we're storing registers we probably shouldn't change
      0x48, 0x83, 0xC4, 0x20,                                             // add rsp, 20
      0x41, 0x59,                                                         // pop r9
      0x41, 0x58,                                                         // pop r8
      0x5A,                                                               // pop rdx
      0x59,                                                               // pop rcx
    };                                                                    // <Copied overwritten instructions>
    private static readonly byte[] INJECTED_CODE_POST = new byte[] {
      0xE9,               0xFF, 0xFF, 0xFF, 0xFF,                         // jmp <return addr>
      // Text Pos:
      0x00, 0xC0, 0x8A, 0x44, // float x
      0x00, 0x00, 0xF0, 0x41, // float y
      0x00, 0x00, 0x00, 0x00, // float z
      // Box Pos:
      0x00, 0x00, 0x8C, 0x44, // float x1
      0x00, 0x00, 0xA0, 0x41, // float y1
      0x00, 0x00, 0x00, 0x00, // float z1
      0x00, 0x80, 0x9D, 0x44, // float x2
      0x00, 0x00, 0x82, 0x42, // float y2
      0x00, 0x00, 0x00, 0x00, // float z2
      // Text:
      0x00
    };

    protected override int INJECTED_CODE_LENGTH { get { return INJECTED_CODE_PRE.Length + INJECTED_CODE_POST.Length; } }
    protected override int ALLOC_AMOUNT { get { return 0x200; } }

    protected int NULL_VIEWPORT         { get { return  0x2; } }
    protected int FONT                  { get { return 0x15; } }
    protected int SET_FONT              { get { return 0x1E; } }

    protected int BOX_VIEWPORT          { get { return 0x25; } }
    protected int BOX_POS_PUSH          { get { return 0x2B; } }
    protected override int BOX_COLOUR   { get { return 0x35; } }
    protected int DRAW_BOX              { get { return 0x3A; } }

    protected int TEXT_VIEWPORT         { get { return 0x41; } }
    protected int TEXT_ADDR             { get { return 0x47; } }
    protected int TEXT_POS_PUSH         { get { return 0x51; } }
    protected override int TEXT_COLOUR  { get { return 0x5B; } }
    protected int DRAW_TEXT             { get { return 0x64; } }

    protected int COPIED_CODE           { get { return 0x72; } }

    protected int RETURN                { get { return 0x73; } }

    protected override int TEXT_POS     { get { return 0x77; } }
    protected override int BOX_POS_MIN  { get { return 0x83; } }
    protected override int BOX_POS_MAX  { get { return 0x8F; } }
    protected override int TEXT         { get { return 0x9B; } }

    public override void Inject() {
      if (IsInjected) {
        return;
      }

      // Want to get an address relatively near the main module so that we can use 32 bit displacements
      IntPtr wantedAddr = IntPtr.Add(manager.HookedProcess.MainModule.BaseAddress, -0x10000);
      injectedAddr = IntPtr.Zero;
      for (int i = 0; i < 0x100; i++) {
        try {
          injectedAddr = manager.Alloc(ALLOC_AMOUNT, wantedAddr);
        } catch (Win32Exception) {
          wantedAddr = IntPtr.Add(wantedAddr, -0x10000);
          continue;
        }
        break;
      }
      if (injectedAddr == IntPtr.Zero) {
        throw new InjectionFailedException("Unable to allocate memory for injected code.");
      }

      manager.Write(injectedAddr, INJECTED_CODE_PRE);
      manager.Write(
        IntPtr.Add(
          injectedAddr,
          COPIED_CODE
        ),
        manager.Read(
          addr.InjectLocation,
          addr.InjectInstructionLength
        )
      );
      manager.Write(
        IntPtr.Add(
          injectedAddr,
          COPIED_CODE + addr.InjectInstructionLength
        ),
        INJECTED_CODE_POST
      );

      // Absolute pointers - must be 64 bit
      manager.Write<Int64>(
        IntPtr.Add(injectedAddr, FONT),
        addr.Font.ToInt64()
      );

      manager.Write<Int64>(
        IntPtr.Add(injectedAddr, BOX_POS_PUSH),
        injectedAddr.ToInt64() + BOX_POS_MIN + addr.InjectInstructionLength
      );

      manager.Write<Int64>(
        IntPtr.Add(injectedAddr, TEXT_ADDR),
        injectedAddr.ToInt64() + TEXT + addr.InjectInstructionLength
      );

      manager.Write<Int64>(
        IntPtr.Add(injectedAddr, TEXT_POS_PUSH),
        injectedAddr.ToInt64() + TEXT_POS + addr.InjectInstructionLength
      );

      // Relative pointers - all 32 bit
      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, NULL_VIEWPORT),
        addr.Viewport,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, SET_FONT),
        addr.SetFont,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, BOX_VIEWPORT),
        addr.Viewport,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, DRAW_BOX),
        addr.DrawBox,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, TEXT_VIEWPORT),
        addr.Viewport,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, DRAW_TEXT),
        addr.DrawText,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, RETURN + addr.InjectInstructionLength),
        IntPtr.Add(addr.InjectLocation, addr.InjectInstructionLength),
        false
      );

      // Inject the jump into the main process
      // We have to do this in one step - if we wrote the E9 and offset seperately the game might take the jump between the steps and crash
      // This means we have to manually work out the offset
      List<byte> data = new List<byte> {
        0xE9
      };
      Int64 offset = injectedAddr.ToInt64() - (addr.InjectLocation.ToInt64() + 5);
      data.AddRange(BitConverter.GetBytes((Int32) (offset % 0x100000000)));
      // Don't technically need to add nops but it looks neater in the disassembly
      for (int i = data.Count; i < addr.InjectInstructionLength; i++) {
        data.Add(0x90);
      }
      manager.Write(addr.InjectLocation, data.ToArray());
    }
  }
}
