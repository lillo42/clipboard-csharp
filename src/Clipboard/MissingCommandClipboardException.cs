namespace Clipboard;

/// <summary>
/// The exception that is thrown when the command is not found.
/// </summary>
public class MissingCommandClipboardException : Exception
{
    /// <summary>
    /// Initialize a new instance of <see cref="MissingCommandClipboardException"/>.
    /// </summary>
    /// <param name="message">The error message.</param>
    public MissingCommandClipboardException(string? message) : base(message)
    {
    }
}
