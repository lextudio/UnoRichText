// Skeleton TextParagraphFormat backing ITextParagraphFormat.

using System.Collections.Generic;

namespace Microsoft.UI.Text;

internal sealed class TextParagraphFormat : ITextParagraphFormat
{
    private readonly List<(float position, TabAlignment align, TabLeader leader)> _tabs = new();

    public ParagraphAlignment Alignment { get; set; } = ParagraphAlignment.Left;
    public float FirstLineIndent { get; internal set; }
    public bool KeepTogether { get; set; }
    public bool KeepWithNext { get; set; }
    public float LeftIndent { get; internal set; }
    public float LineSpacing { get; internal set; }
    public LineSpacingRule LineSpacingRule { get; internal set; } = LineSpacingRule.Single;
    public ListStyle ListStart { get; set; }
    public float ListLevelIndex { get; set; }
    public int ListTab { get; set; }
    public ListStyle ListType { get; set; }
    public bool NoLineNumber { get; set; }
    public bool PageBreakBefore { get; set; }
    public float RightIndent { get; set; }
    public bool RightToLeft { get; set; }
    public int Style { get; set; }
    public float SpaceAfter { get; set; }
    public float SpaceBefore { get; set; }
    public bool WidowControl { get; set; }
    public int TabCount => _tabs.Count;

    public void AddTab(float position, TabAlignment align, TabLeader leader)
        => _tabs.Add((position, align, leader));

    public void ClearAllTabs() => _tabs.Clear();

    public void DeleteTab(float position)
    {
        for (int i = 0; i < _tabs.Count; i++)
        {
            if (System.Math.Abs(_tabs[i].position - position) < 0.001f)
            {
                _tabs.RemoveAt(i);
                return;
            }
        }
    }

    public ITextParagraphFormat GetClone()
    {
        var clone = new TextParagraphFormat
        {
            Alignment = Alignment,
            FirstLineIndent = FirstLineIndent,
            KeepTogether = KeepTogether,
            KeepWithNext = KeepWithNext,
            LeftIndent = LeftIndent,
            LineSpacing = LineSpacing,
            LineSpacingRule = LineSpacingRule,
            ListStart = ListStart,
            ListLevelIndex = ListLevelIndex,
            ListTab = ListTab,
            ListType = ListType,
            NoLineNumber = NoLineNumber,
            PageBreakBefore = PageBreakBefore,
            RightIndent = RightIndent,
            RightToLeft = RightToLeft,
            Style = Style,
            SpaceAfter = SpaceAfter,
            SpaceBefore = SpaceBefore,
            WidowControl = WidowControl,
        };
        clone._tabs.AddRange(_tabs);
        return clone;
    }

    public void GetTab(int index, out float position, out TabAlignment align, out TabLeader leader)
    {
        if (index < 0 || index >= _tabs.Count)
        {
            position = 0; align = TabAlignment.Left; leader = TabLeader.Spaces; return;
        }
        var tab = _tabs[index];
        position = tab.position; align = tab.align; leader = tab.leader;
    }

    public bool IsEqual(ITextParagraphFormat format)
    {
        if (format is not TextParagraphFormat o) return false;
        return Alignment == o.Alignment && FirstLineIndent == o.FirstLineIndent
            && KeepTogether == o.KeepTogether && KeepWithNext == o.KeepWithNext
            && LeftIndent == o.LeftIndent && LineSpacing == o.LineSpacing
            && LineSpacingRule == o.LineSpacingRule && ListStart == o.ListStart
            && ListLevelIndex == o.ListLevelIndex && ListTab == o.ListTab
            && ListType == o.ListType && NoLineNumber == o.NoLineNumber
            && PageBreakBefore == o.PageBreakBefore && RightIndent == o.RightIndent
            && RightToLeft == o.RightToLeft && Style == o.Style
            && SpaceAfter == o.SpaceAfter && SpaceBefore == o.SpaceBefore
            && WidowControl == o.WidowControl;
    }

    public void SetClone(ITextParagraphFormat format)
    {
        if (format is not TextParagraphFormat o) return;
        Alignment = o.Alignment;
        FirstLineIndent = o.FirstLineIndent;
        KeepTogether = o.KeepTogether;
        KeepWithNext = o.KeepWithNext;
        LeftIndent = o.LeftIndent;
        LineSpacing = o.LineSpacing;
        LineSpacingRule = o.LineSpacingRule;
        ListStart = o.ListStart;
        ListLevelIndex = o.ListLevelIndex;
        ListTab = o.ListTab;
        ListType = o.ListType;
        NoLineNumber = o.NoLineNumber;
        PageBreakBefore = o.PageBreakBefore;
        RightIndent = o.RightIndent;
        RightToLeft = o.RightToLeft;
        Style = o.Style;
        SpaceAfter = o.SpaceAfter;
        SpaceBefore = o.SpaceBefore;
        WidowControl = o.WidowControl;
        _tabs.Clear();
        _tabs.AddRange(o._tabs);
    }

    public void SetIndents(float start, float left, float right)
    {
        FirstLineIndent = start;
        LeftIndent = left;
        RightIndent = right;
    }

    public void SetLineSpacing(LineSpacingRule rule, float spacing)
    {
        LineSpacingRule = rule;
        LineSpacing = spacing;
    }
}
