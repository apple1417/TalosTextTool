using MemTools;
using System;
using System.Drawing;
using System.Text;

namespace TalosTextTool {
  public abstract class ATextInjection {
    // Function to inject the code
    public abstract void Inject();

    protected abstract int INJECTED_CODE_LENGTH { get; } // Not including whatever overwritten code is also copied
    protected abstract int ALLOC_AMOUNT { get; }

    // Offsets from the injection address to where to write the various vars
    protected abstract int TEXT_COLOUR { get; }
    protected abstract int TEXT_POS { get; }
    protected abstract int TEXT { get; }
    protected abstract int BOX_COLOUR { get; }
    protected abstract int BOX_POS_MIN { get; }
    protected abstract int BOX_POS_MAX { get; }


    protected readonly MemManager manager;
    protected readonly IAddressList addr;
    protected IntPtr injectedAddr;

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
        if (manager.Read<Byte>(addr.InjectLocation) == 0xE9) {
          injectedAddr = manager.ReadDisplacement(IntPtr.Add(addr.InjectLocation, 1), false);
          return true;
        }
        return false;
      }
    }

    public ATextInjection(MemManager manager, IAddressList addr) {
      this.manager = manager;
      this.addr = addr;

      if (!addr.FoundAddresses) {
        throw new InjectionFailedException("Could not find locations to inject into! Most likely you are not running a compatible game version.");
      }
    }


    public Color TextColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        manager.Write<Int32>(
          IntPtr.Add(injectedAddr, TEXT_COLOUR),
          value.ToArgb()
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return Color.FromArgb(manager.Read<Int32>(IntPtr.Add(injectedAddr, TEXT_COLOUR)));
      }
    }

    public Vector3F TextPos {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        manager.Write<Vector3F>(
          IntPtr.Add(injectedAddr, TEXT_POS + addr.InjectInstructionLength),
          value
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return manager.Read<Vector3F>(IntPtr.Add(injectedAddr, TEXT_POS + addr.InjectInstructionLength));
      }
    }

    public string Text {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        if (value.Length > ALLOC_AMOUNT - (INJECTED_CODE_LENGTH + addr.InjectInstructionLength) - 1) {
          throw new InjectionFailedException("Provided string is too long!");
        }
        manager.WriteString(
          IntPtr.Add(injectedAddr, TEXT + addr.InjectInstructionLength),
          value,
          Encoding.UTF8
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        return manager.ReadString(IntPtr.Add(injectedAddr, TEXT + addr.InjectInstructionLength), Encoding.UTF8);
      }
    }

    public Color BoxColour {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }
        manager.Write<Int32>(
          IntPtr.Add(injectedAddr, BOX_COLOUR),
          value.ToArgb()
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return Color.FromArgb(manager.Read<Int32>(IntPtr.Add(injectedAddr, BOX_COLOUR)));
      }
    }

    public Vector3F BoxPosMin {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        manager.Write<Vector3F>(
          IntPtr.Add(injectedAddr, BOX_POS_MIN + addr.InjectInstructionLength),
          value
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return manager.Read<Vector3F>(IntPtr.Add(injectedAddr, BOX_POS_MIN + addr.InjectInstructionLength));
      }
    }

    public Vector3F BoxPosMax {
      set {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        manager.Write<Vector3F>(
          IntPtr.Add(injectedAddr, BOX_POS_MAX + addr.InjectInstructionLength),
          value
        );
      }
      get {
        if (!IsInjected) {
          throw new InjectionFailedException("Tried to access uninjected field!");
        }

        return manager.Read<Vector3F>(IntPtr.Add(injectedAddr, BOX_POS_MAX + addr.InjectInstructionLength));
      }
    }
  }
}
