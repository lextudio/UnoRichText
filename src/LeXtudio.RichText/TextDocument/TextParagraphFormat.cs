// TextParagraphFormat — public concrete bag implementing Uno's ITextParagraphFormat.
// Property shapes follow Uno's ITextParagraphFormat exactly.

using System.Collections.Generic;
using ITextParagraphFormat = Microsoft.UI.Text.ITextParagraphFormat;
using FormatEffect = Microsoft.UI.Text.FormatEffect;
using LineSpacingRule = Microsoft.UI.Text.LineSpacingRule;
using ParagraphAlignment = Microsoft.UI.Text.ParagraphAlignment;
using ParagraphStyle = Microsoft.UI.Text.ParagraphStyle;
using MarkerAlignment = Microsoft.UI.Text.MarkerAlignment;
using MarkerStyle = Microsoft.UI.Text.MarkerStyle;
using MarkerType = Microsoft.UI.Text.MarkerType;
using TabAlignment = Microsoft.UI.Text.TabAlignment;
using TabLeader = Microsoft.UI.Text.TabLeader;

namespace LeXtudio.UI.Text;

public sealed class TextParagraphFormat : ITextParagraphFormat
{
    private readonly List<(float position, TabAlignment align, TabLeader leader)> _tabs = new();

    public ParagraphAlignment Alignment { get; set; } = ParagraphAlignment.Left;
    public float FirstLineIndent { get; internal set; }
    public FormatEffect KeepTogether { get; set; } = FormatEffect.Off;
    public FormatEffect KeepWithNext { get; set; } = FormatEffect.Off;
    public float LeftIndent { get; internal set; }
    public float LineSpacing { get; internal set; }
    public LineSpacingRule LineSpacingRule { get; internal set; } = LineSpacingRule.Single;
    public MarkerAlignment ListAlignment { get; set; } = MarkerAlignment.Left;
    public int ListLevelIndex { get; set; }
    public int ListStart { get; set; }
    public MarkerStyle ListStyle { get; set; } = MarkerStyle.Undefined;
    public float ListTab { get; set; }
    public MarkerType ListType { get; set; } = MarkerType.None;
    public FormatEffect NoLineNumber { get; set; } = FormatEffect.Off;
    public FormatEffect PageBreakBefore { get; set; } = FormatEffect.Off;
    public float RightIndent { get; set; }
    public FormatEffect RightToLeft { get; set; } = FormatEffect.Off;
    public ParagraphStyle Style { get; set; } = ParagraphStyle.Normal;
    public float SpaceAfter { get; set; }
    public float SpaceBefore { get; set; }
    public FormatEffect WidowControl { get; set; } = FormatEffect.Off;
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
            ListAlignment = ListAlignment,
            ListLevelIndex = ListLevelIndex,
            ListStart = ListStart,
            ListStyle = ListStyle,
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
            && LineSpacingRule == o.LineSpacingRule && ListAlignment == o.ListAlignment
            && ListLevelIndex == o.ListLevelIndex && ListStart == o.ListStart
            && ListStyle == o.ListStyle && ListTab == o.ListTab && ListType == o.ListType
            && NoLineNumber == o.NoLineNumber && PageBreakBefore == o.PageBreakBefore
            && RightIndent == o.RightIndent && RightToLeft == o.RightToLeft
            && Style == o.Style && SpaceAfter == o.SpaceAfter && SpaceBefore == o.SpaceBefore
            && WidowControl == o.WidowControl;
    }

    public void SetClone(ITextParagraphFormat format)
    {
        if (format is not TextParagraphFormat o) return;
        Alignment = o.Alignment; FirstLineIndent = o.FirstLineIndent;
        KeepTogether = o.KeepTogether; KeepWithNext = o.KeepWithNext;
        LeftIndent = o.LeftIndent; LineSpacing = o.LineSpacing;
        LineSpacingRule = o.LineSpacingRule; ListAlignment = o.ListAlignment;
        ListLevelIndex = o.ListLevelIndex; ListStart = o.ListStart;
        ListStyle = o.ListStyle; ListTab = o.ListTab; ListType = o.ListType;
        NoLineNumber = o.NoLineNumber; PageBreakBefore = o.PageBreakBefore;
        RightIndent = o.RightIndent; RightToLeft = o.RightToLeft;
        Style = o.Style; SpaceAfter = o.SpaceAfter; SpaceBefore = o.SpaceBefore;
        WidowControl = o.WidowControl;
        _tabs.Clear(); _tabs.AddRange(o._tabs);
    }

    public void SetIndents(float start, float left, float right)
    {
        FirstLineIndent = start; LeftIndent = left; RightIndent = right;
    }

    public void SetLineSpacing(LineSpacingRule rule, float spacing)
    {
        LineSpacingRule = rule; LineSpacing = spacing;
    }
}
