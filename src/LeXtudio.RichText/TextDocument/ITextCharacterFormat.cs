using Windows.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;

namespace Microsoft.UI.Text;

public interface ITextCharacterFormat
{
    bool AllCaps { get; set; }
    Color BackgroundColor { get; set; }
    FormatEffect Bold { get; set; }
    FormatEffect Hidden { get; set; }
    FormatEffect Italic { get; set; }
    LinkType LinkType { get; }
    string Name { get; set; }
    FormatEffect Outline { get; set; }
    float Position { get; set; }
    FormatEffect ProtectedText { get; set; }
    int LanguageTag { get; set; }
    float Size { get; set; }
    FormatEffect SmallCaps { get; set; }
    int Spacing { get; set; }
    FormatEffect Strikethrough { get; set; }
    int Style { get; set; }
    FormatEffect Subscript { get; set; }
    FormatEffect Superscript { get; set; }
    FormatEffect TextScript { get; set; }
    Color ForegroundColor { get; set; }
    UnderlineType Underline { get; set; }
    int Weight { get; set; }

    ITextCharacterFormat GetClone();
    bool IsEqual(ITextCharacterFormat format);
    void SetClone(ITextCharacterFormat format);
}

public enum UnderlineType
{
    None = 0,
    Single = 1,
    Words = 2,
    Double = 3,
    Dotted = 4,
    Dash = 5,
    DashDot = 6,
    DashDotDot = 7,
    Wave = 8,
    Thick = 9,
    Thin = 10,
    DoubleWave = 11,
    HeavyWave = 12,
    LongDash = 13,
    ThickDash = 14,
    ThickDashDot = 15,
    ThickDashDotDot = 16,
    ThickDotted = 17,
    ThickLongDash = 18,
}
