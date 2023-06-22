using System.Diagnostics;

namespace Clipboard;

/// <summary>
/// The MacOS implementation of the <see cref="IClipboard"/>.
/// </summary>
public class MacClipboard : IClipboard
{
    /// <inheritdoc cref="IClipboard.Read"/>
    public string Read()
    {
        using var process = new Process();
        process.StartInfo.FileName = "pbpaste";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.WaitForExit();
        return process.StandardOutput.ReadToEnd();
    }

    /// <inheritdoc cref="IClipboard.ReadAsync"/>
    public async Task<string> ReadAsync(CancellationToken cancellationToken = default)
    {
        using var process = new Process();
        process.StartInfo.FileName = "pbpaste";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        return await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IClipboard.Write"/>
    public void Write(string text)
    {
        using var process = new Process();
        process.StartInfo.FileName = "pbcopy";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.Start();
        process.StandardInput.Write(text);
        process.StandardInput.Flush();
        process.Close();
    }

    /// <inheritdoc cref="IClipboard.WriteAsync"/>
    public async Task WriteAsync(string text, CancellationToken cancellationToken = default)
    {
        using var process = new Process();
        process.StartInfo.FileName = "pbcopy";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.Start();
        await process.StandardInput.WriteAsync(text).ConfigureAwait(false);
        await process.StandardInput.FlushAsync().ConfigureAwait(false);
        process.Close();
    }
}
