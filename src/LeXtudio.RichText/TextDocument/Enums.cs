// Microsoft.UI.Text enum and constant support types (WinUI 3 shapes).
// These match the WinUI 3 surface for parity. Behavior is added in
// the corresponding concrete document/range/selection/format classes.

namespace Microsoft.UI.Text;

public enum TextGetOptions
{
    None = 0,
    AdjustCrlf = 1,
    UseCrlf = 2,
    UseObjectText = 4,
    AllowFinalEop = 8,
    NoHidden = 16,
    IncludeNumbering = 32,
    FormatRtf = 64,
}

public enum TextSetOptions
{
    None = 0,
    UnicodeBidi = 1,
    Unhide = 8,
    CheckTextLimit = 16,
    FormatRtf = 64,
    ApplyRtfDocumentDefaults = 128,
    Unlink = 256,
}

public enum TextRangeUnit
{
    Character = 1,
    Word = 2,
    Sentence = 3,
    Paragraph = 4,
    Line = 5,
    Story = 6,
    Screen = 7,
    Section = 8,
    Window = 9,
    CharacterFormat = 13,
    ParagraphFormat = 14,
    Object = 15,
    HardParagraph = 16,
    Cluster = 17,
    Cell = 18,
    Row = 19,
    Column = 20,
    Table = 21,
    Hidden = 22,
    Bold = 23,
    Italic = 24,
    Underline = 25,
    Strikethrough = 26,
    Protected = 27,
    Link = 28,
    SmallCaps = 29,
    AllCaps = 30,
    Disabled = 31,
    Revised = 32,
    Subscript = 33,
    Superscript = 34,
    FontBound = 35,
    LinkProtected = 36,
    ConflictBound = 37,
    Replace = 41,
    BookmarkRange = 42,
}

public enum SelectionType
{
    None = 0,
    InsertionPoint = 1,
    Normal = 2,
    Frame = 3,
    Column = 4,
    Row = 5,
    Block = 6,
    InlineShape = 7,
    Shape = 8,
}

public enum RichEditMathMode
{
    NoMath = 0,
    MathOnly = 1,
}

public enum PointOptions
{
    None = 0,
    IncludeInset = 1,
    Start = 32,
    ClientCoordinates = 256,
    AllowOffClient = 1024,
    Transform = 4096,
}

public enum FindOptions
{
    None = 0,
    Word = 2,
    Case = 4,
}

public enum CaretType
{
    Normal = 0,
    Null = 1,
    Korean = 64,
}

public enum LinkType
{
    Undefined = 0,
    NotALink = 1,
    ClientLink = 2,
    FriendlyLinkName = 3,
    FriendlyLinkAddress = 4,
    AutoLink = 5,
    AutoLinkEmail = 6,
    AutoLinkPhone = 7,
    AutoLinkPath = 8,
}

public enum ParagraphAlignment
{
    Undefined = 0,
    Left = 1,
    Center = 2,
    Right = 3,
    Justify = 4,
    FullInterword = 4,
    FullInterletter = 5,
    FullScaled = 6,
    FullGlyphs = 7,
    Snaketext = 8,
}

public enum ListStyle
{
    None = 0,
    Bullet = 1,
    Arabic = 2,
    LowercaseEnglishLetter = 3,
    UppercaseEnglishLetter = 4,
    LowercaseRoman = 5,
    UppercaseRoman = 6,
    Unicode = 7,
}

public enum LineSpacingRule
{
    Undefined = 0,
    Single = 1,
    OneAndHalf = 2,
    Double = 3,
    AtLeast = 4,
    Exactly = 5,
    Multiple = 6,
    Percent = 7,
}

public enum LetterCase
{
    Lower = 0,
    Upper = 1,
}

public enum FormatEffect
{
    Off = 0,
    On = 1,
    Toggle = 2,
    Undefined = 3,
}

public enum TextScript
{
    Undefined = 0,
    Ansi = 1,
    Default = 2,
    Symbol = 3,
    Invariant = 4,
    Mac = 5,
    Oem = 6,
    Shiftjis = 17,
    Hangul = 18,
    Johab = 19,
    Gb2312 = 20,
    Big5 = 21,
    Greek = 22,
    Turkish = 23,
    Vietnamese = 24,
    Hebrew = 25,
    Arabic = 26,
    ArabicTraditional = 27,
    ArabicUser = 28,
    HebrewUser = 29,
    Baltic = 30,
    Russian = 31,
    Thai = 32,
    EastEurope = 33,
    Pc437 = 34,
    Oem850 = 35,
}

public enum HorizontalCharacterAlignment
{
    Default = 0,
    Left = 1,
    Center = 2,
    Right = 3,
}

public enum VerticalCharacterAlignment
{
    Top = 0,
    Baseline = 1,
    Bottom = 2,
}
