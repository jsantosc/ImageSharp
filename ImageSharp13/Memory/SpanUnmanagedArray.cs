namespace ImageSharp
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Array in unmanaged array
    /// </summary>
    /// <typeparam name="T">Type of data fo the array</typeparam>
    public class SpanUnmanagedArray<T> : IDisposable
        where T : struct
    {
        private readonly int size;
        private readonly IntPtr array;
        private readonly bool gcPressure;
        private bool disposed;
        private Span<T> currentSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanUnmanagedArray{T}"/> class.
        /// </summary>
        /// <param name="length">Length of the array</param>
        /// <param name="gcPressure">True if the allocated memory should count for the Garbage Collector, False otherwise.</param>
        public SpanUnmanagedArray(int length, bool gcPressure = false)
        {
            this.size = Marshal.SizeOf<T>();
            this.Length = length;

            // _enumerationBehavior = enumerationBehavior;
            this.array = Win32.VirtualAlloc(IntPtr.Zero, (IntPtr)(this.Length * this.size), Win32.MEM_RESERVE | Win32.MEM_COMMIT, Win32.PAGE_READWRITE);
            if (this.array == IntPtr.Zero)
            {
                throw new Exception("Allocation request failed.", new Win32Exception(Marshal.GetLastWin32Error()));
            }

            unsafe
            {
                this.currentSpan = new Span<T>(this.array.ToPointer(), length);
            }

            this.gcPressure = gcPressure;
            if (this.gcPressure)
            {
                GC.AddMemoryPressure(this.Length * this.size);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpanUnmanagedArray{T}"/> class.
        /// </summary>
        ~SpanUnmanagedArray() => this.Dispose(false);

        /// <summary>
        /// Gets or shets the addres of the unmanaged array
        /// </summary>
        public IntPtr AddressOf => this.array;

        /// <summary>
        /// Gets the length of the array
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets or sets one element of the array
        /// </summary>
        /// <param name="index">Index of the array to modify</param>
        /// <returns>Element at the position</returns>
        public T this[int index]
        {
            get => this.currentSpan[index];
            set => this.currentSpan[index] = value;
        }

        public Span<T> Slice(int start, int length)
        {
            return this.currentSpan.Slice(start, length);
        }

        /// <summary>
        /// Loads an unmanaged array from a Stream
        /// </summary>
        /// <param name="stream">Stream to load</param>
        /// <returns>Array in unmanaged memory</returns>
        public static SpanUnmanagedArray<T> FromStream(Stream stream)
        {
            var a = new SpanUnmanagedArray<T>((int)stream.Length);
            byte[] bufferBytes = new byte[1024];
            int readed = 0;
            int totalReaded = 0;
            while ((readed = stream.Read(bufferBytes, 0, bufferBytes.Length)) != 0)
            {
                Marshal.Copy(bufferBytes, 0, a.AddressOf + totalReaded, readed);
                totalReaded += readed;
            }

            return a;
        }

        /// <summary>
        /// Loads an unmanaged array from a byte array
        /// </summary>
        /// <param name="array">Arrat to Load</param>
        /// <returns>Array in unmanaged memory</returns>
        public static SpanUnmanagedArray<byte> FromByteArray(byte[] array)
        {
            var a = new SpanUnmanagedArray<byte>(array.Length);
            Marshal.Copy(array, 0, a.AddressOf, array.Length);

            return a;
        }

        /// <summary>
        /// Dispose the unmanaged array
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // managed here...
                }

                // unmanaged here...
                this.Free();
            }

            this.disposed = true;
        }

        /// <summary>
        /// Releases all unmanaged memory backing this array.
        /// </summary>
        private void Free()
        {
            if (this.array != IntPtr.Zero && !Win32.VirtualFree(this.array, IntPtr.Zero, Win32.MEM_RELEASE))
            {
                if (!Environment.HasShutdownStarted)
                {
                    throw new Exception("Free allocation failed.", new Win32Exception(Marshal.GetLastWin32Error()));
                }
            }

            if (this.gcPressure)
            {
                GC.RemoveMemoryPressure(this.Length * this.size);
            }

            this.Length = 0;
        }
    }
}