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
    private FormatEffect _allCaps = FormatEffect.Off;
    private Color _backgroundColor = Color.FromArgb(0, 0, 0, 0);
    private FormatEffect _bold = FormatEffect.Off;
    private FontStretch _fontStretch = FontStretch.Normal;
    private FontStyle _fontStyle = FontStyle.Normal;
    private Color _foregroundColor = Color.FromArgb(255, 0, 0, 0);
    private FormatEffect _hidden = FormatEffect.Off;
    private FormatEffect _italic = FormatEffect.Off;
    private float _kerning;
    private string _languageTag = string.Empty;
    private LinkType _linkType = LinkType.NotALink;
    private string _name = "Segoe UI";
    private FormatEffect _outline = FormatEffect.Off;
    private float _position;
    private FormatEffect _protectedText = FormatEffect.Off;
    private float _size = 14f;
    private FormatEffect _smallCaps = FormatEffect.Off;
    private float _spacing;
    private FormatEffect _strikethrough = FormatEffect.Off;
    private FormatEffect _subscript = FormatEffect.Off;
    private FormatEffect _superscript = FormatEffect.Off;
    private TextScript _textScript = TextScript.Default;
    private UnderlineType _underline = UnderlineType.None;
    private int _weight = 400;

    internal event System.EventHandler? Changed;

    public FormatEffect AllCaps { get => _allCaps; set => Set(ref _allCaps, value); }
    public Color BackgroundColor { get => _backgroundColor; set => Set(ref _backgroundColor, value); }
    public FormatEffect Bold { get => _bold; set => Set(ref _bold, value); }
    public FontStretch FontStretch { get => _fontStretch; set => Set(ref _fontStretch, value); }
    public FontStyle FontStyle { get => _fontStyle; set => Set(ref _fontStyle, value); }
    public Color ForegroundColor { get => _foregroundColor; set => Set(ref _foregroundColor, value); }
    public FormatEffect Hidden { get => _hidden; set => Set(ref _hidden, value); }
    public FormatEffect Italic { get => _italic; set => Set(ref _italic, value); }
    public float Kerning { get => _kerning; set => Set(ref _kerning, value); }
    public string LanguageTag { get => _languageTag; set => Set(ref _languageTag, value ?? string.Empty); }
    public LinkType LinkType { get => _linkType; internal set => Set(ref _linkType, value); }
    public string Name { get => _name; set => Set(ref _name, value ?? string.Empty); }
    public FormatEffect Outline { get => _outline; set => Set(ref _outline, value); }
    public float Position { get => _position; set => Set(ref _position, value); }
    public FormatEffect ProtectedText { get => _protectedText; set => Set(ref _protectedText, value); }
    public float Size { get => _size; set => Set(ref _size, value); }
    public FormatEffect SmallCaps { get => _smallCaps; set => Set(ref _smallCaps, value); }
    public float Spacing { get => _spacing; set => Set(ref _spacing, value); }
    public FormatEffect Strikethrough { get => _strikethrough; set => Set(ref _strikethrough, value); }
    public FormatEffect Subscript { get => _subscript; set => Set(ref _subscript, value); }
    public FormatEffect Superscript { get => _superscript; set => Set(ref _superscript, value); }
    public TextScript TextScript { get => _textScript; set => Set(ref _textScript, value); }
    public UnderlineType Underline { get => _underline; set => Set(ref _underline, value); }
    public int Weight { get => _weight; set => Set(ref _weight, value); }

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

    private void Set<T>(ref T field, T value)
    {
        if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        Changed?.Invoke(this, System.EventArgs.Empty);
    }
}
