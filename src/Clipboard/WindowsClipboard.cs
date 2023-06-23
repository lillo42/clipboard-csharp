using System.Runtime.InteropServices;
using System.Text;
using NodaTime;

namespace Clipboard;

/// <summary>
/// Windows implementation of the <see cref="IClipboard"/>.
/// </summary>
public partial class WindowsClipboard : IClipboard
{
    private const uint CfUnicodeText = 13;
    private const uint GmemMoveable = 0x0002;
    private const string Kernel32 = "kernel32.dll";
    private const string User32 = "user32.dll";

    private readonly IClock _clock;

    /// <summary>
    /// Initialize the <see cref="WindowsClipboard"/> class.
    /// </summary>
    /// <param name="clock">The <see cref="IClock"/>.</param>
    public WindowsClipboard(IClock clock)
    {
        _clock = clock;
    }

    /// <inheritdoc cref="IClipboard.Read"/>
    public string Read()
    {
        if (!IsClipboardFormatAvailable(CfUnicodeText))
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
        }

        WaitOpenClipboard();

        try
        {
            var handler = GetClipboardData(CfUnicodeText);
            if (handler == nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            var locker = GlobalLock(handler);
            if (locker == nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            var text = Marshal.PtrToStringUni(locker) ?? string.Empty;

            if (!GlobalUnlock(handler))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            return text;
        }
        finally
        {
            CloseClipboard();
        }
    }

    /// <inheritdoc cref="IClipboard.ReadAsync"/>
    public Task<string> ReadAsync(CancellationToken cancellationToken = default) => Task.FromResult(Read());

    /// <inheritdoc cref="IClipboard.Write"/>
    public void Write(string text)
    {
        if (!IsClipboardFormatAvailable(CfUnicodeText))
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
        }

        WaitOpenClipboard();
        if (!EmptyClipboard())
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
        }

        var data = Encoding.Unicode.GetBytes(text);

        // "If the hMem parameter identifies a memory object, the object must have
        // been allocated using the function with the GMEM_MOVEABLE flag."
        var alloc = GlobalAlloc(GmemMoveable, (ulong)(data.Length + 2));
        if (alloc == nint.Zero)
        {
            var ex = Marshal.GetExceptionForHR(Marshal.GetLastPInvokeError());
            CloseClipboard();
            throw ex!;
        }

        try
        {
            // Lock the handle and copy the text to the buffer
            var locker = GlobalLock(alloc);
            if (locker == nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            Marshal.Copy(data, 0, locker, data.Length);

            if (!GlobalUnlock(alloc))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            // Place the handle on the clipboard. 
            var clip = SetClipboardData(CfUnicodeText, alloc);
            if (clip == nint.Zero)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }

            alloc = nint.Zero;
        }
        finally
        {
            CloseClipboard();
            if (alloc != nint.Zero)
            {
                GlobalFree(alloc);
            }
        }
    }

    /// <inheritdoc cref="IClipboard.WriteAsync"/>
    public Task WriteAsync(string text, CancellationToken cancellationToken = default)
    {
        Write(text);
        return Task.CompletedTask;
    }

    private void WaitOpenClipboard()
    {
        var startedAt = _clock.GetCurrentInstant();
        var limit = startedAt + Duration.FromSeconds(1);

        while (_clock.GetCurrentInstant() < limit)
        {
            if (OpenClipboard(0))
            {
                return;
            }

            Thread.Sleep(1);
        }

        Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
    }

    #region Clipboard

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseClipboard();

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EmptyClipboard();

    [LibraryImport(User32, SetLastError = true)]
    private static partial nint GetClipboardData(uint format);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsClipboardFormatAvailable(uint format);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenClipboard(nint hWndNewOwner);

    [LibraryImport(User32, SetLastError = true)]
    private static partial nint SetClipboardData(uint uFormat, nint hMem);

    #endregion

    #region Kernel

    [LibraryImport(Kernel32, SetLastError = true)]
    private static partial nint GlobalAlloc(uint uFlags, ulong dwBytes);

    [LibraryImport(Kernel32, SetLastError = true)]
    private static partial nint GlobalFree(nint hMem);

    [LibraryImport(Kernel32, SetLastError = true)]
    private static partial nint GlobalLock(nint hMem);

    [LibraryImport(Kernel32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GlobalUnlock(nint hMem);

    #endregion
}
