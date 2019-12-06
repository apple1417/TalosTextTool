using MemTools;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TalosTextTool {
  class TextInjection32 : ITextInjection {
    private readonly MemManager manager;
    private readonly AddressList32 addr;
    private IntPtr injectedAddr;

    public bool IsHooked { get { return manager.IsHooked; } }
    public bool IsInjected {
      get {
        if (!IsHooked) {
          return false;
        }
        if (injectedAddr != IntPtr.Zero) {
          return true;
        }

        // If we already read a jump call in the inject location
        if (manager.ReadByte(addr.InjectLocation) == 0xE9) {
          injectedAddr = manager.ReadOffset(IntPtr.Add(addr.InjectLocation, 1));
          return true;
        }
        return false;
      }
    }

    public TextInjection32(MemManager manager) {
      this.manager = manager;

      addr = new AddressList32(manager);
      if (!addr.FoundAddresses) {
        throw new InjectionFailedException("Could not find locations to inject into! Most likely you are not running a compatible game version.");
      }
    }

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
      0x83, 0xC4, 0x20,                     // add esp,20
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

    private const int BOX_COLOUR =     0x1;
    private const int BOX_POS =        0x6;
    private const int BOX_VIEWPORT =   0xC;
    private const int DRAW_BOX =      0x11;

    private const int FONT =          0x16;
    private const int SET_FONT =      0x1B;

    private const int TEXT_COLOUR =   0x20;
    private const int TEXT_POS =      0x25;
    private const int TEXT_ADDR =     0x2A;
    private const int TEXT_VIEWPORT = 0x30;
    private const int DRAW_TEXT =     0x35;

    private const int COPIED_CODE =   0x3C;

    private const int RETURN =        0x3D;

    private const int TEXT_X =        0x41;
    private const int TEXT_Y =        0x45;
    private const int TEXT_Z =        0x49;

    private const int BOX_X1 =        0x4D;
    private const int BOX_Y1 =        0x51;
    private const int BOX_Z1 =        0x55;

    private const int BOX_X2 =        0x59;
    private const int BOX_Y2 =        0x5D;
    private const int BOX_Z2 =        0x61;

    private const int TEXT =          0x65;

    private const int ALLOC_AMOUNT =   512;

    public void Inject() {
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
      manager.WriteInt32(
        IntPtr.Add(injectedAddr, TEXT_ADDR),
        injectedAddr.ToInt32() + TEXT + addr.InjectInstructionLength
      );

      manager.WriteInt32(
        IntPtr.Add(injectedAddr, TEXT_POS),
        injectedAddr.ToInt32() + TEXT_X + addr.InjectInstructionLength
      );

      manager.WriteInt32(
        IntPtr.Add(injectedAddr, BOX_POS),
        injectedAddr.ToInt32() + BOX_X1 + addr.InjectInstructionLength
      );

      // Viewport and Font push addresses so they're absolute
      manager.WritePtr(
        IntPtr.Add(injectedAddr, BOX_VIEWPORT),
        addr.Viewport
      );
      manager.WritePtr(
        IntPtr.Add(injectedAddr, TEXT_VIEWPORT),
        addr.Viewport
      );

      manager.WritePtr(
        IntPtr.Add(injectedAddr, FONT),
        addr.Font
      );

      // Deal with the jumps/calls, which use relative addresses

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, DRAW_BOX),
        addr.DrawBox
      );

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, SET_FONT),
        addr.SetFont
      );

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, DRAW_TEXT),
        addr.DrawText
      );

      manager.WriteOffset(
        IntPtr.Add(injectedAddr, RETURN + addr.InjectInstructionLength),
        IntPtr.Add(addr.InjectLocation, addr.InjectInstructionLength)
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

    public Color TextColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        manager.WriteInt32(
          IntPtr.Add(injectedAddr, TEXT_COLOUR),
          value.ToArgb()
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return Color.FromArgb(manager.ReadInt32(IntPtr.Add(injectedAddr, TEXT_COLOUR)));
      }
    }

    public Vector3<float> TextPos {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        manager.WriteFloat(
          IntPtr.Add(injectedAddr, TEXT_X + addr.InjectInstructionLength),
          value.X
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, TEXT_Y + addr.InjectInstructionLength),
          value.Y
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, TEXT_Z + addr.InjectInstructionLength),
          value.Z
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return new Vector3<float>() {
          X = manager.ReadFloat(IntPtr.Add(injectedAddr, TEXT_X + addr.InjectInstructionLength)),
          Y = manager.ReadFloat(IntPtr.Add(injectedAddr, TEXT_Y + addr.InjectInstructionLength)),
          Z = manager.ReadFloat(IntPtr.Add(injectedAddr, TEXT_Z + addr.InjectInstructionLength)),
        };
      }
    }

    public string Text {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        if (value.Length > ALLOC_AMOUNT - (INJECTED_CODE_PRE.Length + INJECTED_CODE_POST.Length + addr.InjectInstructionLength) - 1) {
          throw new InjectionFailedException("Provided string is too long!");
        }
        manager.WriteUtf8(
          IntPtr.Add(injectedAddr, TEXT + addr.InjectInstructionLength),
          value
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        return manager.ReadUtf8(IntPtr.Add(injectedAddr, TEXT + addr.InjectInstructionLength));
      }
    }

    public Color BoxColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        manager.WriteInt32(
          IntPtr.Add(injectedAddr, BOX_COLOUR),
          value.ToArgb()
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return Color.FromArgb(manager.ReadInt32(IntPtr.Add(injectedAddr, BOX_COLOUR)));
      }
    }

    public Vector3<float> BoxPosMin {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_X1 + addr.InjectInstructionLength),
          value.X
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Y1 + addr.InjectInstructionLength),
          value.Y
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Z1 + addr.InjectInstructionLength),
          value.Z
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return new Vector3<float>() {
          X = manager.ReadFloat(IntPtr.Add(injectedAddr, BOX_X1 + addr.InjectInstructionLength)),
          Y = manager.ReadFloat(IntPtr.Add(injectedAddr, BOX_Y1 + addr.InjectInstructionLength)),
          Z = manager.ReadFloat(IntPtr.Add(injectedAddr, BOX_Z1 + addr.InjectInstructionLength)),
        };
      }
    }

    public Vector3<float> BoxPosMax {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_X2 + addr.InjectInstructionLength),
          value.X
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Y2 + addr.InjectInstructionLength),
          value.Y
        );
        manager.WriteFloat(
          IntPtr.Add(injectedAddr, BOX_Z2 + addr.InjectInstructionLength),
          value.Z
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return new Vector3<float>() {
          X = manager.ReadFloat(IntPtr.Add(injectedAddr, BOX_X2 + addr.InjectInstructionLength)),
          Y = manager.ReadFloat(IntPtr.Add(injectedAddr, BOX_Y2 + addr.InjectInstructionLength)),
          Z = manager.ReadFloat(IntPtr.Add(injectedAddr, BOX_Z2 + addr.InjectInstructionLength)),
        };
      }
    }
  }
}
