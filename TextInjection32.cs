using MemTools;
using System;
using System.Collections.Generic;

namespace TalosTextTool {
  class TextInjection32 : ATextInjection {
    public TextInjection32(MemManager manager, IAddressList addr) : base(manager, addr) { }

    // TODO: Jump over all this if viewport is null
    private static readonly byte[] INJECTED_CODE_PRE = new byte[] {
      0x68,         0x00, 0x00, 0x00, 0xB0, // push <box colour>
      0x68,         0xFF, 0xFF, 0xFF, 0xFF, // push <box pos>
      0xFF, 0x35,   0xFF, 0xFF, 0xFF, 0xFF, // push [<viewport>]
      0xE8,         0xFF, 0xFF, 0xFF, 0xFF, // call <DrawBox>
      0x68,         0xFF, 0xFF, 0xFF, 0xFF, // push <font>
      0xE8,         0xFF, 0xFF, 0xFF, 0xFF, // call <SetFont>
      0x68,         0xFF, 0xFF, 0xFF, 0xFF, // push <text colour>
      0x68,         0xFF, 0xFF, 0xFF, 0xFF, // push <text pos>
      0x68,         0xFF, 0xFF, 0xFF, 0xFF, // push <text>
      0xFF, 0x35,   0xFF, 0xFF, 0xFF, 0xFF, // push [<viewport>]
      0xE8,         0xFF, 0xFF, 0xFF, 0xFF, // call <DrawText>
      0x83, 0xC4, 0x20,                     // add esp, 20
    };                                      // <Copied overwritten instructions>
    private static readonly byte[] INJECTED_CODE_POST = new byte[] {
      0xE9,         0xFF, 0xFF, 0xFF, 0xFF, // jmp <return addr>
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

    protected override int BOX_COLOUR   { get { return  0x1; } }
    protected int BOX_POS_PUSH          { get { return  0x6; } }
    protected int BOX_VIEWPORT          { get { return  0xC; } }
    protected int DRAW_BOX              { get { return 0x11; } }

    protected int FONT                  { get { return 0x16; } }
    protected int SET_FONT              { get { return 0x1B; } }

    protected override int TEXT_COLOUR  { get { return 0x20; } }
    protected int TEXT_POS_PUSH         { get { return 0x25; } }
    protected int TEXT_ADDR             { get { return 0x2A; } }
    protected int TEXT_VIEWPORT         { get { return 0x30; } }
    protected int DRAW_TEXT             { get { return 0x35; } }

    protected int COPIED_CODE           { get { return 0x3C; } }

    protected int RETURN                { get { return 0x3D; } }

    protected override int TEXT_POS     { get { return 0x41; } }
    protected override int BOX_POS_MIN  { get { return 0x4D; } }
    protected override int BOX_POS_MAX  { get { return 0x59; } }
    protected override int TEXT         { get { return 0x65; } }

    public override void Inject() {
      if (IsInjected) {
        return;
      }

      injectedAddr = manager.Alloc(ALLOC_AMOUNT);

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

      // Fill in the three which point to our own block of code
      manager.Write<Int32>(
        IntPtr.Add(injectedAddr, TEXT_ADDR),
        injectedAddr.ToInt32() + TEXT + addr.InjectInstructionLength
      );

      manager.Write<Int32>(
        IntPtr.Add(injectedAddr, TEXT_POS_PUSH),
        injectedAddr.ToInt32() + TEXT_POS + addr.InjectInstructionLength
      );

      manager.Write<Int32>(
        IntPtr.Add(injectedAddr, BOX_POS_PUSH),
        injectedAddr.ToInt32() + BOX_POS_MIN + addr.InjectInstructionLength
      );

      // Viewport and Font push addresses so they're absolute
      manager.Write<IntPtr>(
        IntPtr.Add(injectedAddr, BOX_VIEWPORT),
        addr.Viewport
      );
      manager.Write<IntPtr>(
        IntPtr.Add(injectedAddr, TEXT_VIEWPORT),
        addr.Viewport
      );

      manager.Write<IntPtr>(
        IntPtr.Add(injectedAddr, FONT),
        addr.Font
      );

      // The jumps/calls use relative addresses

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, DRAW_BOX),
        addr.DrawBox,
        false
      );

      manager.WriteDisplacement(
        IntPtr.Add(injectedAddr, SET_FONT),
        addr.SetFont,
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
      data.AddRange(BitConverter.GetBytes(
        injectedAddr.ToInt32() - (addr.InjectLocation.ToInt32() + 5)
      ));
      // Don't technically need to add nops but it looks neater in the disassembly
      for (int i = data.Count; i < addr.InjectInstructionLength; i++) {
        data.Add(0x90);
      }
      manager.Write(addr.InjectLocation, data.ToArray());
    }
  }
}
