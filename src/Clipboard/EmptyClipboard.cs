namespace Clipboard;

/// <summary>
/// A empty clipboard.
/// </summary>
public class EmptyClipboard : IClipboard
{
    /// <inheritdoc cref="IClipboard.Read"/>
    public string Read()
    {
        return string.Empty;
    }

    /// <inheritdoc cref="IClipboard.ReadAsync"/>
    public Task<string> ReadAsync(CancellationToken cancellationToken = default) => Task.FromResult(string.Empty);

    /// <inheritdoc cref="IClipboard.Write"/>
    public void Write(string text)
    {
    }

    /// <inheritdoc cref="IClipboard.WriteAsync"/>
    public Task WriteAsync(string text, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
