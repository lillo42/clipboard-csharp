using Clipboard;


var @out = await Console.In.ReadToEndAsync().ConfigureAwait(false);
await SystemClipboard.Instance.WriteAsync(@out).ConfigureAwait(false);

return 0;


