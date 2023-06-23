using System.Runtime.InteropServices;
using NodaTime;

namespace Clipboard;

/// <summary>
/// The system clipboard.
/// </summary>
public static class SystemClipboard
{
    /// <summary>
    /// The instance of the <see cref="IClipboard"/>.
    /// </summary>
    public static IClipboard Instance { get; }

    /// <summary>
    /// Initialize the <see cref="SystemClipboard"/> class.
    /// </summary>
    static SystemClipboard()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Instance = new WindowsClipboard(SystemClock.Instance);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Instance = new MacClipboard();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                 || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            Instance = new UnixClipboard();
        }
        else
        {
            Instance = new EmptyClipboard();
        }
    }
}
