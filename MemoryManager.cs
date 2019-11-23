﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TalosTextTool {
  class MemoryManager : IDisposable {
    private const int PROCESS_ALL_ACCESS =  0x1F0FFF;

    private const int MEM_COMMIT =            0x1000;
    private const int MEM_RESERVE =           0x2000;

    private const int PAGE_EXECUTE_READWRITE =  0x40;

    private const int ERROR_INVALID_HANDLE =       6;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(Int32 dwDesiredAccess, bool bInheritHandle, Int32 dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr dwSize, ref IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, Int32 flAllocationType, Int32 flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr dwSize, ref IntPtr lpNumberOfBytesWritten);

    public Process HookedProcess;
    public bool IsHooked {
      get {
        HookedProcess.Refresh();
        if (HookedProcess.HasExited) {
          _isHooked = false;
        }
        return _isHooked;
      }
    }

    private IntPtr handle;
    private bool _isHooked = true;

    public MemoryManager(Process ProcessToHook) {
      HookedProcess = ProcessToHook;

      handle = OpenProcess(PROCESS_ALL_ACCESS, false, HookedProcess.Id);
      if (handle == IntPtr.Zero) {
        _isHooked = false;
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }

    public void Dispose() {
      if (handle != IntPtr.Zero) {
        bool result = CloseHandle(handle);
        handle = IntPtr.Zero;
        if (!result) {
          throw new Win32Exception(Marshal.GetLastWin32Error());
        }
      }
      _isHooked = false;
    }

    public IntPtr Alloc(long len) {
      if (!IsHooked) {
        throw new Win32Exception(ERROR_INVALID_HANDLE, "Process is not hooked!");
      }

      IntPtr addr = VirtualAllocEx(handle, IntPtr.Zero, new IntPtr(len), MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
      if (addr == IntPtr.Zero) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      return addr;
    }

    public byte[] Read(IntPtr addr, long len) {
      if (!IsHooked) {
        throw new Win32Exception(ERROR_INVALID_HANDLE, "Process is not hooked!");
      }

      IntPtr lenRead = IntPtr.Zero;
      byte[] buf = new byte[len];
      bool result = ReadProcessMemory(handle, addr, buf, new IntPtr(len), ref lenRead);
      if (!result) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }

      return buf;
    }

    public void Write(IntPtr addr, byte[] buffer) {
      if (!IsHooked) {
        throw new Win32Exception(ERROR_INVALID_HANDLE, "Process is not hooked!");
      }

      IntPtr lenWritten = IntPtr.Zero;
      bool result = WriteProcessMemory(handle, addr, buffer, new IntPtr(buffer.Length), ref lenWritten);
      if (!result) {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
    }
  }
}
