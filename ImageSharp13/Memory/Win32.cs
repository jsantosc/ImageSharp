namespace ImageSharp
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Pinvoke Win32 methods for memory
    /// </summary>
    public static class Win32
    {
#pragma warning disable CS1591
#pragma warning disable SA1600
#pragma warning disable SA1310
#pragma warning disable SA1201
        // Missing XML comment for publicly visible type or member
        //////////////////////////////////////////////////////////////////////////////////
        // Note:  IntPtr will automatically magically adjust to int (4 bytes) for 32-bit,
        //        and long (8 bytes) for 64-bit.
        //////////////////////////////////////////////////////////////////////////////////

        // dwSize - basically size will be int (for 32-bit) and long (for 64-bit)
        // return value - basically will be 4 byte pointer address (for 32-bit)
        //                and 8 byte pointer address (for 64-bit)
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, IntPtr dwSize, int flAllocationType, int flProtect);

        public const int MEM_COMMIT = 0x1000;
        public const int MEM_RESERVE = 0x2000;

        public const int PAGE_NOACCESS = 0x01;
        public const int PAGE_READONLY = 0x02;
        public const int PAGE_READWRITE = 0x04;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree(IntPtr lpAddress, IntPtr dwSize, int dwFreeType);

        public const int MEM_DECOMMIT = 0x4000;
        public const int MEM_RELEASE = 0x8000;

        // length - basically length will be int (for 32-bit) and long (for 64-bit)
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void MoveMemory(IntPtr destination, IntPtr source, IntPtr length);

        // dwSize - basically size will be int (for 32-bit) and long (for 64-bit)
        [DllImport("kernel32.dll", EntryPoint = "RtlFillMemory", SetLastError = false)]
        public static extern void FillMemory(IntPtr lpAddress, IntPtr dwSize, byte fill);

        // dwSize - basically size will be int (for 32-bit) and long (for 64-bit)
        [DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        public static extern void ZeroMemory(IntPtr lpAddress, IntPtr dwSize);
#pragma warning restore CS1591
#pragma warning restore SA1600
#pragma warning restore SA1310
#pragma warning restore SA1201
    }
}