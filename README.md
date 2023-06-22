# Clipboard for C# 

Provide copying and pasting to the Clipboard for C#.

Platforms:

* OSX
* Windows 10 (probably work on other Windows)
* Linux, Unix (requires 'xclip' or 'xsel' command to be installed)


Notes:
* Text string only

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