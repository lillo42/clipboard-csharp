using Clipboard;

var text = await SystemClipboard.Instance.ReadAsync().ConfigureAwait(false);
await Console.Out.WriteAsync(text).ConfigureAwait(false);

return 0;
