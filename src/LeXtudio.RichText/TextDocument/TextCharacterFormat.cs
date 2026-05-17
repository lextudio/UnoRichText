// TextCharacterFormat — public concrete bag implementing Uno's ITextCharacterFormat.
// Uno does not ship a public concrete class, so consumers need ours when they
// want to construct one (e.g. for SetDefaultCharacterFormat).
//
// Property shapes follow Uno's ITextCharacterFormat exactly; see audit notes
// in docs/DESIGN.md.

using ITextCharacterFormat = Microsoft.UI.Text.ITextCharacterFormat;
using FormatEffect = Microsoft.UI.Text.FormatEffect;
using LinkType = Microsoft.UI.Text.LinkType;
using TextScript = Microsoft.UI.Text.TextScript;
using UnderlineType = Microsoft.UI.Text.UnderlineType;
using global::Windows.UI;
using FontStretch = global::Windows.UI.Text.FontStretch;
using FontStyle = global::Windows.UI.Text.FontStyle;

namespace LeXtudio.UI.Text;

public sealed class TextCharacterFormat : ITextCharacterFormat
{
    public FormatEffect AllCaps { get; set; } = FormatEffect.Off;
    public Color BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
    public FormatEffect Bold { get; set; } = FormatEffect.Off;
    public FontStretch FontStretch { get; set; } = FontStretch.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public Color ForegroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
    public FormatEffect Hidden { get; set; } = FormatEffect.Off;
    public FormatEffect Italic { get; set; } = FormatEffect.Off;
    public float Kerning { get; set; } = 0f;
    public string LanguageTag { get; set; } = string.Empty;
    public LinkType LinkType { get; internal set; } = LinkType.NotALink;
    public string Name { get; set; } = "Segoe UI";
    public FormatEffect Outline { get; set; } = FormatEffect.Off;
    public float Position { get; set; } = 0f;
    public FormatEffect ProtectedText { get; set; } = FormatEffect.Off;
    public float Size { get; set; } = 14f;
    public FormatEffect SmallCaps { get; set; } = FormatEffect.Off;
    public float Spacing { get; set; } = 0f;
    public FormatEffect Strikethrough { get; set; } = FormatEffect.Off;
    public FormatEffect Subscript { get; set; } = FormatEffect.Off;
    public FormatEffect Superscript { get; set; } = FormatEffect.Off;
    public TextScript TextScript { get; set; } = TextScript.Default;
    public UnderlineType Underline { get; set; } = UnderlineType.None;
    public int Weight { get; set; } = 400;

    public ITextCharacterFormat GetClone() => new TextCharacterFormat
    {
        AllCaps = AllCaps,
        BackgroundColor = BackgroundColor,
        Bold = Bold,
        FontStretch = FontStretch,
        FontStyle = FontStyle,
        ForegroundColor = ForegroundColor,
        Hidden = Hidden,
        Italic = Italic,
        Kerning = Kerning,
        LanguageTag = LanguageTag,
        LinkType = LinkType,
        Name = Name,
        Outline = Outline,
        Position = Position,
        ProtectedText = ProtectedText,
        Size = Size,
        SmallCaps = SmallCaps,
        Spacing = Spacing,
        Strikethrough = Strikethrough,
        Subscript = Subscript,
        Superscript = Superscript,
        TextScript = TextScript,
        Underline = Underline,
        Weight = Weight,
    };

    public bool IsEqual(ITextCharacterFormat format)
    {
        if (format is not TextCharacterFormat o) return false;
        return AllCaps == o.AllCaps && BackgroundColor.Equals(o.BackgroundColor)
            && Bold == o.Bold && FontStretch == o.FontStretch && FontStyle == o.FontStyle
            && ForegroundColor.Equals(o.ForegroundColor) && Hidden == o.Hidden && Italic == o.Italic
            && Kerning == o.Kerning && LanguageTag == o.LanguageTag && LinkType == o.LinkType
            && Name == o.Name && Outline == o.Outline && Position == o.Position
            && ProtectedText == o.ProtectedText && Size == o.Size && SmallCaps == o.SmallCaps
            && Spacing == o.Spacing && Strikethrough == o.Strikethrough
            && Subscript == o.Subscript && Superscript == o.Superscript
            && TextScript == o.TextScript && Underline == o.Underline && Weight == o.Weight;
    }

    public void SetClone(ITextCharacterFormat format)
    {
        if (format is not TextCharacterFormat o) return;
        AllCaps = o.AllCaps; BackgroundColor = o.BackgroundColor; Bold = o.Bold;
        FontStretch = o.FontStretch; FontStyle = o.FontStyle; ForegroundColor = o.ForegroundColor;
        Hidden = o.Hidden; Italic = o.Italic; Kerning = o.Kerning; LanguageTag = o.LanguageTag;
        LinkType = o.LinkType; Name = o.Name; Outline = o.Outline; Position = o.Position;
        ProtectedText = o.ProtectedText; Size = o.Size; SmallCaps = o.SmallCaps; Spacing = o.Spacing;
        Strikethrough = o.Strikethrough; Subscript = o.Subscript; Superscript = o.Superscript;
        TextScript = o.TextScript; Underline = o.Underline; Weight = o.Weight;
    }
}
