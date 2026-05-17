// Skeleton TextCharacterFormat backing ITextCharacterFormat.

using Windows.UI;

namespace Microsoft.UI.Text;

internal sealed class TextCharacterFormat : ITextCharacterFormat
{
    public bool AllCaps { get; set; }
    public Color BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
    public FormatEffect Bold { get; set; } = FormatEffect.Off;
    public FormatEffect Hidden { get; set; } = FormatEffect.Off;
    public FormatEffect Italic { get; set; } = FormatEffect.Off;
    public LinkType LinkType { get; internal set; } = LinkType.NotALink;
    public string Name { get; set; } = "Segoe UI";
    public FormatEffect Outline { get; set; } = FormatEffect.Off;
    public float Position { get; set; } = 0f;
    public FormatEffect ProtectedText { get; set; } = FormatEffect.Off;
    public int LanguageTag { get; set; } = 0;
    public float Size { get; set; } = 14f;
    public FormatEffect SmallCaps { get; set; } = FormatEffect.Off;
    public int Spacing { get; set; } = 0;
    public FormatEffect Strikethrough { get; set; } = FormatEffect.Off;
    public int Style { get; set; } = 0;
    public FormatEffect Subscript { get; set; } = FormatEffect.Off;
    public FormatEffect Superscript { get; set; } = FormatEffect.Off;
    public FormatEffect TextScript { get; set; } = FormatEffect.Off;
    public Color ForegroundColor { get; set; } = Color.FromArgb(255, 0, 0, 0);
    public UnderlineType Underline { get; set; } = UnderlineType.None;
    public int Weight { get; set; } = 400;

    public ITextCharacterFormat GetClone() => new TextCharacterFormat
    {
        AllCaps = AllCaps,
        BackgroundColor = BackgroundColor,
        Bold = Bold,
        Hidden = Hidden,
        Italic = Italic,
        LinkType = LinkType,
        Name = Name,
        Outline = Outline,
        Position = Position,
        ProtectedText = ProtectedText,
        LanguageTag = LanguageTag,
        Size = Size,
        SmallCaps = SmallCaps,
        Spacing = Spacing,
        Strikethrough = Strikethrough,
        Style = Style,
        Subscript = Subscript,
        Superscript = Superscript,
        TextScript = TextScript,
        ForegroundColor = ForegroundColor,
        Underline = Underline,
        Weight = Weight,
    };

    public bool IsEqual(ITextCharacterFormat format)
    {
        if (format is not TextCharacterFormat o) return false;
        return AllCaps == o.AllCaps && BackgroundColor.Equals(o.BackgroundColor)
            && Bold == o.Bold && Hidden == o.Hidden && Italic == o.Italic
            && LinkType == o.LinkType && Name == o.Name && Outline == o.Outline
            && Position == o.Position && ProtectedText == o.ProtectedText
            && LanguageTag == o.LanguageTag && Size == o.Size && SmallCaps == o.SmallCaps
            && Spacing == o.Spacing && Strikethrough == o.Strikethrough && Style == o.Style
            && Subscript == o.Subscript && Superscript == o.Superscript
            && TextScript == o.TextScript && ForegroundColor.Equals(o.ForegroundColor)
            && Underline == o.Underline && Weight == o.Weight;
    }

    public void SetClone(ITextCharacterFormat format)
    {
        if (format is not TextCharacterFormat o) return;
        AllCaps = o.AllCaps;
        BackgroundColor = o.BackgroundColor;
        Bold = o.Bold;
        Hidden = o.Hidden;
        Italic = o.Italic;
        LinkType = o.LinkType;
        Name = o.Name;
        Outline = o.Outline;
        Position = o.Position;
        ProtectedText = o.ProtectedText;
        LanguageTag = o.LanguageTag;
        Size = o.Size;
        SmallCaps = o.SmallCaps;
        Spacing = o.Spacing;
        Strikethrough = o.Strikethrough;
        Style = o.Style;
        Subscript = o.Subscript;
        Superscript = o.Superscript;
        TextScript = o.TextScript;
        ForegroundColor = o.ForegroundColor;
        Underline = o.Underline;
        Weight = o.Weight;
    }
}
