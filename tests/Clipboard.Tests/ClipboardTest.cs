using FluentAssertions;

namespace Clipboard.Tests;

public class ClipboardTest
{
    private readonly IClipboard _clipboard;

    public ClipboardTest()
    {
        _clipboard = SystemClipboard.Instance;
    }

    public static IEnumerable<object[]> Texts => new List<object[]>
    {
        new object[] { "日本語" },
        new object[] { "test" },
        new object[] { "hello world" },
        new object[] { "French: éèêëàùœç" },
        new object[] { "Weird UTF-8: 💩☃" },
    };

    [Theory]
    [MemberData(nameof(Texts))]
    public void CopyAndPaste(string value)
    {
        _clipboard.Write(value);
        _clipboard.Read().Should().Be(value);
    }

    [Theory]
    [MemberData(nameof(Texts))]
    public async Task CopyAndPasteAsync(string value)
    {
        await _clipboard.WriteAsync(value);
        (await _clipboard.ReadAsync()).Should().Be(value);
    }
}
