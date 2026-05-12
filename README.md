# UnoRichText

UnoRichText is a rich text layout library for [Uno Platform](https://platform.uno), providing `RichTextBlock` and the associated document model types that are missing or incomplete in Uno's current implementation.

It is backed by [PretextSharp](https://github.com/wieslawsoltes/PretextSharp) for cross-platform text measurement and layout, and mirrors the WinUI 3 / WPF namespace conventions so it can be used as a drop-in replacement with minimal code changes.

## Why

Uno Platform's built-in `RichTextBlock` has several gaps:

- Styling properties (`FontFamily`, `FontSize`, `Foreground`, etc.) are not applied.
- `InlineUIContainer` — used to embed arbitrary UI elements inline with text — is not implemented.
- Nested `RichTextBlock` inside containers (borders, grids, etc.) often renders no text at all.

UnoRichText replaces these types with a fully custom implementation that works correctly on all Uno Skia Desktop targets.

## Supported Platforms

- Windows 11 (Uno Skia Desktop)
- macOS, 3 most recent versions
- Ubuntu latest LTS (other Linux distributions may work but are not primary targets)

> For support of additional platforms or business sponsorship enquiries, please reach out at [lextudio.com](https://lextudio.com).

## Get Started

Install the NuGet package:

[![NuGet](https://img.shields.io/nuget/v/LeXtudio.RichText.svg?label=LeXtudio.RichText)](https://www.nuget.org/packages/LeXtudio.RichText)

```shell
dotnet add package LeXtudio.RichText
```

### Provided types

| Type | Namespace |
|------|-----------|
| `RichTextBlock` | `LeXtudio.UI.Xaml.Controls` |
| `Paragraph` | `LeXtudio.UI.Xaml.Documents` |
| `InlineCollection` | `LeXtudio.UI.Xaml.Documents` |
| `Run` | `LeXtudio.UI.Xaml.Documents` |
| `Bold` | `LeXtudio.UI.Xaml.Documents` |
| `Italic` | `LeXtudio.UI.Xaml.Documents` |
| `Span` | `LeXtudio.UI.Xaml.Documents` |
| `Hyperlink` | `LeXtudio.UI.Xaml.Documents` |
| `LineBreak` | `LeXtudio.UI.Xaml.Documents` |
| `InlineUIContainer` | `LeXtudio.UI.Xaml.Documents` |

### Usage

The API mirrors WinUI 3's `RichTextBlock`:

```csharp
using LeXtudio.UI.Xaml.Controls;
using LeXtudio.UI.Xaml.Documents;

var block = new RichTextBlock
{
    FontSize = 14,
    TextWrapping = TextWrapping.WrapWholeWords
};

var paragraph = new Paragraph();
paragraph.Inlines.Add(new Run { Text = "Hello, " });
paragraph.Inlines.Add(new Bold { Inlines = { new Run { Text = "world" } } });
block.Blocks.Add(paragraph);
```

### Bridging from WinUI 3 code

If you have existing WinUI 3 code that uses the native `RichTextBlock` and related types, you can redirect unqualified names to UnoRichText on Uno targets using `global using` aliases in a single file:

```csharp
#if !WINDOWS_APP_SDK
global using RichTextBlock     = LeXtudio.UI.Xaml.Controls.RichTextBlock;
global using InlineCollection  = LeXtudio.UI.Xaml.Documents.InlineCollection;
global using Paragraph         = LeXtudio.UI.Xaml.Documents.Paragraph;
global using Run               = LeXtudio.UI.Xaml.Documents.Run;
global using Bold              = LeXtudio.UI.Xaml.Documents.Bold;
global using Italic            = LeXtudio.UI.Xaml.Documents.Italic;
global using Span              = LeXtudio.UI.Xaml.Documents.Span;
global using LineBreak         = LeXtudio.UI.Xaml.Documents.LineBreak;
global using InlineUIContainer = LeXtudio.UI.Xaml.Documents.InlineUIContainer;
global using Hyperlink         = LeXtudio.UI.Xaml.Documents.Hyperlink;
#endif
```

No other changes to the consuming code are needed.

## License

MIT — see [LICENSE](LICENSE).
