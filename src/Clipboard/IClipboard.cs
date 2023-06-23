namespace Clipboard;

/// <summary>
/// The interface for the clipboard.
/// </summary>
public interface IClipboard
{
    /// <summary>
    /// Read the clipboard text.
    /// </summary>
    /// <returns>The clipboard text.</returns>
    string Read();

    /// <summary>
    /// Read the clipboard text.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The clipboard text.</returns>
    Task<string> ReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Write the clipboard text.
    /// </summary>
    /// <param name="text">The text to be write.</param>
    void Write(string text);

    /// <summary>
    /// Write the clipboard text.
    /// </summary>
    /// <param name="text">The text to be write.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    Task WriteAsync(string text, CancellationToken cancellationToken = default);
}
