using System.Text;
using Clipboard;

Console.OutputEncoding = Encoding.UTF8;
var clipboard = SystemClipboard.Instance;

var text = "test";
Console.WriteLine("Writing {0} to clipboard...", text);
clipboard.Write(text);

Console.WriteLine("Reading from clipboard...");
Console.WriteLine(clipboard.Read());

text = "日本語";
Console.WriteLine("Writing {0} to clipboard...", text); 
await clipboard.WriteAsync(text);
Console.WriteLine("Reading from clipboard...");
Console.WriteLine(await clipboard.ReadAsync());
