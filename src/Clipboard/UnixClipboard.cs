using System.Diagnostics;

namespace Clipboard;

/// <summary>
/// The Unix implementation of the <see cref="IClipboard"/>. 
/// </summary>
public class UnixClipboard : IClipboard
{
    private const string MissingCommands =
        "No clipboard utilities available. Please install xsel, xclip, wl-clipboard or Termux:API add-on for termux-clipboard-get/set.";

    private static readonly List<(string name, string args)> s_paste = new()
    {
        ("wl-paste", "--no-newline"),
        ("xsel", "--output --clipboard"),
        ("xclip", "-out -selection clipboard"),
        ("powershell.exe", "Get-Clipboard"),
        ("termux-clipboard-get", string.Empty)
    };

    private static readonly List<(string name, string args)> s_copy = new()
    {
        ("wl-copy", string.Empty),
        ("xsel", "--input --clipboard"),
        ("xclip", "-int -selection clipboard"),
        ("clip.exe", string.Empty),
        ("termux-clipboard-set", string.Empty)
    };


    private readonly (string command, string args) _paste;
    private readonly (string command, string args) _copy;

    private readonly bool _trim;
    private readonly bool _unsupported;

    /// <summary>
    /// Initialize the <see cref="UnixClipboard"/> class.
    /// </summary>
    public UnixClipboard()
    {
        _paste = s_paste.FirstOrDefault(x => ExistOnPath(x.name));
        _copy = s_copy.FirstOrDefault(x => ExistOnPath(x.name));

        _unsupported = _paste.command == null || _copy.command == null;
        _trim = _paste.command != _copy.command;

        static bool ExistOnPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                return true;
            }

            var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var path in pathEnv.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Initialize the <see cref="UnixClipboard"/> class.
    /// </summary>
    /// <param name="paste">The paste command.</param>
    /// <param name="pasteArgs">The paste args.</param>
    /// <param name="copy">The copy command.</param>
    /// <param name="copyArgs">The copy args.</param>
    /// <param name="trim">If it should trim.</param>
    public UnixClipboard(string paste, string pasteArgs, 
        string copy, string copyArgs,
        bool trim = false)
    {
        _paste = (paste, pasteArgs);
        _copy = (copy, copyArgs);
        _trim = trim;
        _unsupported = false;
    }

    /// <inheritdoc cref="IClipboard.Read"/>
    public string Read()
    {
        if (_unsupported)
        {
            throw new MissingCommandClipboardException(MissingCommands);
        }

        using var process = new Process();
        process.StartInfo.FileName = _paste.command;
        process.StartInfo.Arguments = _paste.args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.WaitForExit();
        var text = process.StandardOutput.ReadToEnd();
        if (_trim && text.Length > 0)
        {
            return text[..^2];
        }

        return text;
    }

    /// <inheritdoc cref="IClipboard.ReadAsync"/>
    public async Task<string> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (_unsupported)
        {
            throw new MissingCommandClipboardException(MissingCommands);
        }

        using var process = new Process();
        process.StartInfo.FileName = _paste.command;
        process.StartInfo.Arguments = _paste.args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        var text = await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        if (_trim && text.Length > 0)
        {
            return text[..^2];
        }

        return text;
    }

    /// <inheritdoc cref="IClipboard.Write"/>
    public void Write(string text)
    {
        if (_unsupported)
        {
            throw new MissingCommandClipboardException(MissingCommands);
        }

        using var process = new Process();
        process.StartInfo.FileName = _copy.command;
        process.StartInfo.Arguments = _copy.args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;

        process.Start();
        process.StandardInput.WriteLine(text);
        process.StandardInput.Flush();
        process.StandardInput.Close();
        process.WaitForExit();
    }

    /// <inheritdoc cref="IClipboard.WriteAsync"/>
    public async Task WriteAsync(string text, CancellationToken cancellationToken = default)
    {
        if (_unsupported)
        {
            throw new MissingCommandClipboardException(MissingCommands);
        }

        using var process = new Process();
        process.StartInfo.FileName = _copy.command;
        process.StartInfo.Arguments = _copy.args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;

        process.Start();
        await process.StandardInput.WriteAsync(text.AsMemory(), cancellationToken).ConfigureAwait(false);
        await process.StandardInput.FlushAsync().ConfigureAwait(false);
        process.StandardInput.Close();
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
    }
}
