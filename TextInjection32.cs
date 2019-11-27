using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TalosTextTool {
  class TextInjection32 : ITextInjection {
    private readonly MemoryManager manager;
    private readonly AddressFinder addr;
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

        byte[] call = manager.Read(addr.DrawTextCall, 5);
        if (call[0] == 0xE9) {
          injectedAddr = IntPtr.Add(addr.DrawTextCall, BitConverter.ToInt32(call, 1) + 5);
          return true;
        }
        return false;
      }
    }

    public TextInjection32(MemoryManager manager) {
      this.manager = manager;

      addr = new AddressFinder(manager);
      if (!addr.FoundAddresses) {
        throw new InjectionFailedException("Could not find locations to inject into! Most likely you are not running a compatible game version.");
      }
    }

    // TODO: Replace the variables here reasonable defaults + 0xFF for pointers
    private static readonly byte[] INJECTED_CODE = new byte[] {
      0xC7, 0x44, 0x24, 0x04,   0x5E, 0x00, 0x89, 0x1D,   // mov [esp+04],<text>
      0xC7, 0x44, 0x24, 0x08,   0x3A, 0x00, 0x89, 0x1D,   // mov [esp+08],<text pos>
      0xC7, 0x44, 0x24, 0x0C,   0xEE, 0x00, 0x66, 0xFF,   // mov [esp+0C],<text colour>
      0xE8,                     0x13, 0xA3, 0x3A, 0xE3,   // call <DrawText>
      0x68,                     0x66, 0x00, 0xAA, 0x7F,   // push <box colour>
      0x68,                     0x46, 0x00, 0x89, 0x1D,   // push <box pos>
      0xFF, 0x35,               0x20, 0x8A, 0x5E, 0x01,   // push [<viewport>]
      0xE8,                     0xEE, 0xFD, 0x39, 0xE3,   // call <DrawBox>
      0x83, 0xC4, 0x0C,                                   // add esp,0C
      0xE9,                     0x41, 0x1B, 0x1B, 0xE3,   // jmp <return addr>
      // Text Pos:
      0x00, 0x00, 0xC8, 0x42,
      0x00, 0x00, 0xC8, 0x42,
      0x00, 0x00, 0x00, 0x00,
      // Box Pos:
      0x00, 0x00, 0xfa, 0x43,
      0x00, 0x00, 0xfa, 0x43,
      0x00, 0x00, 0x00, 0x00,
      0x00, 0x00, 0x16, 0x44,
      0x00, 0x00, 0x16, 0x44,
      0x00, 0x00, 0x00, 0x00,
      // Text:
      0x00
    };

    private const int TEXT_ADDR =      0x4;
    private const int TEXT_POS =       0xC;
    private const int TEXT_COLOUR =   0x14;
    private const int DRAW_TEXT =     0x19;
    private const int BOX_COLOUR =    0x1E;
    private const int BOX_POS =       0x23;
    private const int VIEWPORT =      0x29;
    private const int DRAW_BOX =      0x2E;
    private const int RETURN =        0x36;

    private const int TEXT_X =        0x3A;
    private const int TEXT_Y =        0x3E;
    private const int TEXT_Z =        0x42;

    private const int BOX_X1 =        0x46;
    private const int BOX_Y1 =        0x4A;
    private const int BOX_Z1 =        0x4E;

    private const int BOX_X2 =        0x52;
    private const int BOX_Y2 =        0x56;
    private const int BOX_Z2 =        0x5A;

    private const int TEXT =          0x5E;

    private const int ALLOC_AMOUNT =   512;

    // INJECTED_CODE already includes the null byte we need on the string
    public int MaxTextLength { get { return ALLOC_AMOUNT - INJECTED_CODE.Length; } }

    public void Inject() {
      if (IsInjected) {
        return;
      }

      injectedAddr = manager.Alloc(ALLOC_AMOUNT);

      manager.Write(injectedAddr, INJECTED_CODE);

      // Fill in the three which point to our own block of code
      manager.Write(
        IntPtr.Add(injectedAddr, TEXT_ADDR),
        BitConverter.GetBytes(injectedAddr.ToInt32() + TEXT)
      );

      manager.Write(
        IntPtr.Add(injectedAddr, TEXT_POS),
        BitConverter.GetBytes(injectedAddr.ToInt32() + TEXT_X)
      );

      manager.Write(
        IntPtr.Add(injectedAddr, BOX_POS),
        BitConverter.GetBytes(injectedAddr.ToInt32() + BOX_X1)
      );

      // Viewport pushes an address so it's absolute
      manager.Write(
        IntPtr.Add(injectedAddr, VIEWPORT),
        BitConverter.GetBytes(addr.Viewport.ToInt32())
      );

      // Deal with the jumps/calls, which use relative addresses

      // Ignore the E8, read out the address
      int originalDrawTextOffset = BitConverter.ToInt32(manager.Read(IntPtr.Add(addr.DrawTextCall, 1), 4), 0);
      int actualDrawText = addr.DrawTextCall.ToInt32() + originalDrawTextOffset + 5;
      int newDrawTextOffset = actualDrawText - (injectedAddr.ToInt32() + DRAW_TEXT + 4);
      manager.Write(
        IntPtr.Add(injectedAddr, DRAW_TEXT),
        BitConverter.GetBytes(newDrawTextOffset)
      );

      int drawBoxOffset = addr.DrawBox.ToInt32() - (injectedAddr.ToInt32() + DRAW_BOX + 4);
      manager.Write(
        IntPtr.Add(injectedAddr, DRAW_BOX),
        BitConverter.GetBytes(drawBoxOffset)
      );

      int actualReturn = addr.DrawTextCall.ToInt32() + 5;
      int returnOffset = actualReturn - (injectedAddr.ToInt32() + RETURN + 4);
      manager.Write(
        IntPtr.Add(injectedAddr, RETURN),
        BitConverter.GetBytes(returnOffset)
      );

      // Inject the jump into the main process
      int jumpOffset = injectedAddr.ToInt32() - (addr.DrawTextCall.ToInt32() + 5);
      manager.Write(
        addr.DrawTextCall,
        new byte[] {
            0xE9
        }.Concat(BitConverter.GetBytes(jumpOffset)).ToArray()
      );

      // Nop out the cvar check
      manager.Write(
        addr.CvarCheck,
        new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
      );
    }

    public Color TextColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }
        manager.Write(
          IntPtr.Add(injectedAddr, TEXT_COLOUR),
          BitConverter.GetBytes(value.ToArgb())
        );
      }
    }

    public Vector3<float> TextPos {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }

        manager.Write(
          IntPtr.Add(injectedAddr, TEXT_X),
          BitConverter.GetBytes(value.X)
        );
        manager.Write(
          IntPtr.Add(injectedAddr, TEXT_Y),
          BitConverter.GetBytes(value.Y)
        );
        manager.Write(
          IntPtr.Add(injectedAddr, TEXT_Z),
          BitConverter.GetBytes(value.Z)
        );
      }
    }

    public string Text {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }
        if (value.Length > MaxTextLength) {
          throw new InjectionFailedException("Provided string is too long!");
        }
        manager.Write(
          IntPtr.Add(injectedAddr, TEXT),
          Encoding.UTF8.GetBytes(value + "\x00")
        );
      }
    }

    public Color BoxColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }
        manager.Write(
          IntPtr.Add(injectedAddr, BOX_COLOUR),
          BitConverter.GetBytes(value.ToArgb())
        );
      }
    }

    public Vector3<float> BoxPosMin {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }

        manager.Write(
          IntPtr.Add(injectedAddr, BOX_X1),
          BitConverter.GetBytes(value.X)
        );
        manager.Write(
          IntPtr.Add(injectedAddr, BOX_Y1),
          BitConverter.GetBytes(value.Y)
        );
        manager.Write(
          IntPtr.Add(injectedAddr, BOX_Z1),
          BitConverter.GetBytes(value.Z)
        );
      }
    }

    public Vector3<float> BoxPosMax {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to edit field before injecting!");
        }

        manager.Write(
          IntPtr.Add(injectedAddr, BOX_X2),
          BitConverter.GetBytes(value.X)
        );
        manager.Write(
          IntPtr.Add(injectedAddr, BOX_Y2),
          BitConverter.GetBytes(value.Y)
        );
        manager.Write(
          IntPtr.Add(injectedAddr, BOX_Z2),
          BitConverter.GetBytes(value.Z)
        );
      }
    }
  }
}
