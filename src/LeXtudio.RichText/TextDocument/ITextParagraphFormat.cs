namespace Microsoft.UI.Text;

public interface ITextParagraphFormat
{
    ParagraphAlignment Alignment { get; set; }
    float FirstLineIndent { get; }
    bool KeepTogether { get; set; }
    bool KeepWithNext { get; set; }
    float LeftIndent { get; }
    float LineSpacing { get; }
    LineSpacingRule LineSpacingRule { get; }
    ListStyle ListStart { get; set; }
    float ListLevelIndex { get; set; }
    int ListTab { get; set; }
    ListStyle ListType { get; set; }
    bool NoLineNumber { get; set; }
    bool PageBreakBefore { get; set; }
    float RightIndent { get; set; }
    bool RightToLeft { get; set; }
    int Style { get; set; }
    float SpaceAfter { get; set; }
    float SpaceBefore { get; set; }
    bool WidowControl { get; set; }
    int TabCount { get; }

    void AddTab(float position, TabAlignment align, TabLeader leader);
    void ClearAllTabs();
    void DeleteTab(float position);
    ITextParagraphFormat GetClone();
    void GetTab(int index, out float position, out TabAlignment align, out TabLeader leader);
    bool IsEqual(ITextParagraphFormat format);
    void SetClone(ITextParagraphFormat format);
    void SetIndents(float start, float left, float right);
    void SetLineSpacing(LineSpacingRule rule, float spacing);
}

public enum TabAlignment
{
    Left = 0,
    Center = 1,
    Right = 2,
    Decimal = 3,
    Bar = 4,
}

public enum TabLeader
{
    Spaces = 0,
    Dots = 1,
    Dashes = 2,
    Lines = 3,
    ThickLines = 4,
    Equals = 5,
}
