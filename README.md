# Clipboard for C# 

`Clipboard` is a [C#](https://learn.microsoft.com/en-us/dotnet/csharp/) library to copying and pasting to the Clipboard. It is heavily inspired by the 
`Go` library [clipboard](https://github.com/atotto/clipboard).

Supported OS:
* OSX
* Windows 10 (probably work on other Windows)
* Linux/Unix (requires 'xclip' or 'xsel' command to be installed)


Notes:
* Text string only

## Getting Started

```c#
using Clipboard;

const string text = "test";
Console.WriteLine("Writing {0} to clipboard...", text);
SystemClipboard.Instance.Write(text);

Console.WriteLine("Reading from clipboard...");
Console.WriteLine(clipboard.Read());
```

## Commands:

paste shell command:
```bash
dotnet tool install -g dotpaste
dotpaste > document.txt
```
copy shell command:
```bash
dotnet tool install -g dotcopy
cat document.txt | dotcopy
```

## Authors
Rafael Andrade - Project Owner & creator
