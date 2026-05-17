using Microsoft.UI.Text;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using System.Reflection;
using RichEditDocument = LeXtudio.UI.Text.RichEditTextDocument;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class RichEditTextDocumentTests
{
    [Test]
    public void BoldToggle_DoesNotReplaceSelectedText()
    {
        var document = new RichEditDocument();
        document.SetText(TextSetOptions.None, "Hi");
        document.Selection.SetRange(0, 2);

        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.GetText(TextGetOptions.None, out var text);
        Assert.That(text, Is.EqualTo("Hi"));

        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs, Has.Count.EqualTo(1));
        Assert.That(runs[0].Start, Is.EqualTo(0));
        Assert.That(runs[0].End, Is.EqualTo(2));
        Assert.That(runs[0].Format.Bold, Is.EqualTo(FormatEffect.On));
    }

    [Test]
    public void InsertInMiddleOfBoldRun_Inherits()
    {
        var document = CreateDocument("Hello");
        document.GetRange(1, 4).CharacterFormat.Bold = FormatEffect.On;

        InsertText(document, 2, "X");

        AssertText(document, "HeXllo");
        AssertBoldRuns(document, (1, 5));
    }

    [Test]
    public void InsertWithExplicitFormat_SplitsExistingRun()
    {
        var document = CreateDocument("Hello");
        var offFormat = (LeXtudio.UI.Text.TextCharacterFormat)document.GetDefaultCharacterFormat();
        offFormat.Bold = FormatEffect.Off;
        document.GetRange(0, 5).CharacterFormat = offFormat;

        var onFormat = (LeXtudio.UI.Text.TextCharacterFormat)document.GetDefaultCharacterFormat();
        onFormat.Bold = FormatEffect.On;
        InsertText(document, 2, "X", onFormat);

        AssertText(document, "HeXllo");
        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs, Has.Count.EqualTo(3));
        Assert.That((runs[0].Start, runs[0].End, runs[0].Format.Bold), Is.EqualTo((0, 2, FormatEffect.Off)));
        Assert.That((runs[1].Start, runs[1].End, runs[1].Format.Bold), Is.EqualTo((2, 3, FormatEffect.On)));
        Assert.That((runs[2].Start, runs[2].End, runs[2].Format.Bold), Is.EqualTo((3, 6, FormatEffect.Off)));
    }

    [Test]
    public void InsertAtRunStart_DoesNotInherit()
    {
        var document = CreateDocument("Hello");
        document.GetRange(2, 5).CharacterFormat.Bold = FormatEffect.On;

        InsertText(document, 2, "X");

        AssertText(document, "HeXllo");
        AssertBoldRuns(document, (3, 6));
    }

    [Test]
    public void InsertAtRunEnd_InheritsLeftRun()
    {
        var document = CreateDocument("Hello");
        document.GetRange(0, 2).CharacterFormat.Bold = FormatEffect.On;

        InsertText(document, 2, "X");

        AssertText(document, "HeXllo");
        AssertBoldRuns(document, (0, 3));
    }

    [Test]
    public void DeleteInsideRun_TrimsRun()
    {
        var document = CreateDocument("HHHH");
        document.GetRange(0, 4).CharacterFormat.Bold = FormatEffect.On;

        DeleteRange(document, 1, 3);

        AssertText(document, "HH");
        AssertBoldRuns(document, (0, 2));
    }

    [Test]
    public void DeleteAcrossTwoRuns_RemovesAllRuns()
    {
        var document = CreateDocument("AB");
        document.GetRange(0, 1).CharacterFormat.Bold = FormatEffect.On;
        document.GetRange(1, 2).CharacterFormat.Italic = FormatEffect.On;

        DeleteRange(document, 0, 2);

        AssertText(document, string.Empty);
        Assert.That(document.GetCharacterFormatRuns(), Is.Empty);
    }

    [Test]
    public void DeleteTrimsLeftAndShiftsRight()
    {
        var document = CreateDocument("abcdef");
        document.GetRange(2, 5).CharacterFormat.Bold = FormatEffect.On;

        DeleteRange(document, 0, 3);

        AssertText(document, "def");
        AssertBoldRuns(document, (0, 2));
    }

    [Test]
    public void ReplaceSelection_InheritsDeletedFormat()
    {
        var document = CreateDocument("BIG");
        document.GetRange(0, 3).CharacterFormat.Bold = FormatEffect.On;

        var format = GetCharacterFormat(document, 0, 3);
        DeleteRange(document, 0, 3);
        InsertText(document, 0, "j", format);

        AssertText(document, "j");
        AssertBoldRuns(document, (0, 1));
    }

    [Test]
    public void CollapsedSelectionToggle_AppliesToTypedText()
    {
        var document = CreateDocument(string.Empty);
        document.Selection.SetRange(0, 0);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Selection.TypeText("x");

        AssertText(document, "x");
        AssertBoldRuns(document, (0, 1));
    }

    [Test]
    public void CollapsedSelectionToggleAtRunEnd_RemovesFormatFromTypedText()
    {
        var document = CreateDocument("Hi");
        document.GetRange(0, 2).CharacterFormat.Bold = FormatEffect.On;
        document.Selection.SetRange(2, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Selection.TypeText("j");

        AssertText(document, "Hij");
        AssertBoldRuns(document, (0, 2));
    }

    [Test]
    public void CollapsedSelectionUnderline_AppliesToTypedText()
    {
        var document = CreateDocument(string.Empty);
        document.Selection.SetRange(0, 0);
        document.Selection.CharacterFormat.Underline = UnderlineType.Single;

        document.Selection.TypeText("x");

        AssertText(document, "x");
        AssertUnderlineRuns(document, (0, 1));
    }

    [Test]
    public void UndoRedo_Insert_RestoresAndReappliesRuns()
    {
        var document = CreateDocument("Hello");
        document.GetRange(1, 4).CharacterFormat.Bold = FormatEffect.On;

        InsertText(document, 2, "X");
        Assert.That(document.CanUndo(), Is.True);
        Assert.That(document.CanRedo(), Is.False);
        AssertText(document, "HeXllo");
        AssertBoldRuns(document, (1, 5));

        document.Undo();
        Assert.That(document.CanRedo(), Is.True);
        AssertText(document, "Hello");
        AssertBoldRuns(document, (1, 4));

        document.Redo();
        AssertText(document, "HeXllo");
        AssertBoldRuns(document, (1, 5));
    }

    [Test]
    public void UndoRedo_Delete_RestoresAndReappliesRuns()
    {
        var document = CreateDocument("abcdef");
        document.GetRange(2, 5).CharacterFormat.Bold = FormatEffect.On;

        DeleteRange(document, 0, 3);
        AssertText(document, "def");
        AssertBoldRuns(document, (0, 2));

        document.Undo();
        AssertText(document, "abcdef");
        AssertBoldRuns(document, (2, 5));

        document.Redo();
        AssertText(document, "def");
        AssertBoldRuns(document, (0, 2));
    }

    [Test]
    public void UndoThenNewEdit_ClearsRedoHistory()
    {
        var document = CreateDocument("abc");

        InsertText(document, 1, "X");
        document.Undo();
        Assert.That(document.CanRedo(), Is.True);

        InsertText(document, 1, "Y");
        Assert.That(document.CanRedo(), Is.False);
        AssertText(document, "aYbc");
    }

    [Test]
    public void UndoBoldToggle_RestoresPlainRunsAndPreservesText()
    {
        // Workflow: type "Hi", select "Hi", Ctrl+B, Ctrl+Z.
        // Expectation: text remains "Hi", bold run is gone, selection (0..2) is restored.
        var document = new RichEditDocument();
        InsertText(document, 0, "H");
        InsertText(document, 1, "i");
        document.Selection.SetRange(0, 2);

        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        AssertBoldRuns(document, (0, 2));
        Assert.That(document.CanUndo(), Is.True);

        document.Undo();

        AssertText(document, "Hi");
        AssertBoldRuns(document); // no bold runs left
        Assert.That(document.Selection.StartPosition, Is.EqualTo(0));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
    }

    [Test]
    public void RedoBoldToggle_ReappliesRunsAndSelection()
    {
        var document = CreateDocument("Hi");
        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Undo();
        Assert.That(document.CanRedo(), Is.True);

        document.Redo();

        AssertText(document, "Hi");
        AssertBoldRuns(document, (0, 2));
        Assert.That(document.Selection.StartPosition, Is.EqualTo(0));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
    }

    [Test]
    public void UndoInsert_RestoresPriorSelection()
    {
        // Selection bounds must travel with the snapshot, not be clamped to current state.
        var document = CreateDocument("ab");
        document.Selection.SetRange(2, 2);
        InsertText(document, 2, "c");
        document.Selection.SetRange(0, 3);

        document.Undo();

        AssertText(document, "ab");
        Assert.That(document.Selection.StartPosition, Is.EqualTo(2));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
    }

    [Test]
    public void RedoBoldToggle_AfterUndo_RestoresBoldAndSelection()
    {
        // Workflow: type "Hi", select "Hi", Ctrl+B, Ctrl+Z, Ctrl+Y.
        var document = new RichEditDocument();
        InsertText(document, 0, "H");
        InsertText(document, 1, "i");
        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Undo();
        Assert.That(document.CanRedo(), Is.True);

        document.Redo();

        AssertText(document, "Hi");
        AssertBoldRuns(document, (0, 2));
        Assert.That(document.Selection.StartPosition, Is.EqualTo(0));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
        Assert.That(document.CanRedo(), Is.False);
    }

    [Test]
    public void RedoTyping_RestoresInsertedTextAndCaret()
    {
        var document = CreateDocument("ab");
        document.Selection.SetRange(2, 2);
        InsertText(document, 2, "c");

        document.Undo();
        AssertText(document, "ab");

        document.Redo();
        AssertText(document, "abc");
        Assert.That(document.Selection.StartPosition, Is.EqualTo(2));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
    }

    [Test]
    public void RedoMultipleSteps_ReplaysInOriginalOrder()
    {
        var document = new RichEditDocument();
        InsertText(document, 0, "H");
        InsertText(document, 1, "i");
        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Undo(); // undo bold
        document.Undo(); // undo "i"
        AssertText(document, "H");
        AssertBoldRuns(document);

        document.Redo(); // redo "i"
        AssertText(document, "Hi");
        AssertBoldRuns(document);

        document.Redo(); // redo bold
        AssertText(document, "Hi");
        AssertBoldRuns(document, (0, 2));
        Assert.That(document.Selection.StartPosition, Is.EqualTo(0));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
        Assert.That(document.CanRedo(), Is.False);
    }

    [Test]
    public void RedoDelete_ReappliesDeletion()
    {
        var document = CreateDocument("abcdef");
        document.GetRange(2, 5).CharacterFormat.Bold = FormatEffect.On;
        DeleteRange(document, 0, 3);

        document.Undo();
        AssertText(document, "abcdef");
        AssertBoldRuns(document, (2, 5));

        document.Redo();
        AssertText(document, "def");
        AssertBoldRuns(document, (0, 2));
    }

    [Test]
    public void RetroApplyScope_ColorsJustTypedCharacterFromCollapsedCaret()
    {
        // WinUI parity: while the host is dispatching TextChanged after an
        // insertion, setting a delta format on the collapsed caret retro-colors
        // the just-typed text (so "type H -> set green" yields a green H).
        var document = CreateDocument(string.Empty);
        var green = global::Windows.UI.Color.FromArgb(255, 0, 128, 0);

        InsertText(document, 0, "H");
        document.Selection.SetRange(1, 1);

        var scope = (System.IDisposable)typeof(RichEditDocument)
            .GetMethod("EnterRetroApplyScope", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(document, null)!;
        try
        {
            document.Selection.CharacterFormat.ForegroundColor = green;
        }
        finally
        {
            scope.Dispose();
        }

        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs, Has.Count.EqualTo(1));
        Assert.That(runs[0].Format.ForegroundColor, Is.EqualTo(green));
        Assert.That((runs[0].Start, runs[0].End), Is.EqualTo((0, 1)));
    }

    [Test]
    public void RetroApply_OutsideScope_LeavesJustTypedTextUntouched()
    {
        // Without the retro-apply scope (i.e. programmatic edits), setting a
        // format on the collapsed caret must only affect future input. This
        // guards the bold/italic typing test below from regressing.
        var document = CreateDocument(string.Empty);
        var green = global::Windows.UI.Color.FromArgb(255, 0, 128, 0);

        InsertText(document, 0, "H");
        document.Selection.SetRange(1, 1);
        document.Selection.CharacterFormat.ForegroundColor = green;

        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs.Any(r => r.Start == 0 && r.End == 1 && r.Format.ForegroundColor == green), Is.False,
            "programmatic format set must not retro-color the prior insert");
    }

    [Test]
    public void BoldToggle_PreservesExistingForegroundColor()
    {
        // Repro: type green "Hi", select it, Ctrl+B. Bold must not wipe the
        // foreground color of the existing runs.
        var document = CreateDocument("Hi");
        var green = global::Windows.UI.Color.FromArgb(255, 0, 128, 0);
        document.GetRange(0, 2).CharacterFormat.ForegroundColor = green;

        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs, Has.Count.EqualTo(1));
        Assert.That(runs[0].Format.Bold, Is.EqualTo(FormatEffect.On));
        Assert.That(runs[0].Format.ForegroundColor, Is.EqualTo(green));
    }

    [Test]
    public void UndoBoldToggle_RestoresGreenNonBoldText()
    {
        // Full workflow: green "Hi", select, Ctrl+B, Ctrl+Z. Must end on
        // green non-bold "Hi" with selection (0..2) restored.
        var document = CreateDocument("Hi");
        var green = global::Windows.UI.Color.FromArgb(255, 0, 128, 0);
        document.GetRange(0, 2).CharacterFormat.ForegroundColor = green;

        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Undo();

        AssertText(document, "Hi");
        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs, Has.Count.EqualTo(1));
        Assert.That(runs[0].Format.Bold, Is.Not.EqualTo(FormatEffect.On));
        Assert.That(runs[0].Format.ForegroundColor, Is.EqualTo(green));
        Assert.That(document.Selection.StartPosition, Is.EqualTo(0));
        Assert.That(document.Selection.EndPosition, Is.EqualTo(2));
    }

    [Test]
    public void BoldToggle_OverMixedRuns_PreservesPerRunFormatting()
    {
        // Selection spans an uncolored "H" and a green "i". Bolding the
        // selection must preserve both per-run formatting attributes.
        var document = CreateDocument("Hi");
        var green = global::Windows.UI.Color.FromArgb(255, 0, 128, 0);
        document.GetRange(1, 2).CharacterFormat.ForegroundColor = green;

        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        var runs = document.GetCharacterFormatRuns().OrderBy(r => r.Start).ToList();
        Assert.That(runs.All(r => r.Format.Bold == FormatEffect.On), Is.True,
            "every run in the bolded range should be bold");
        var greenRun = runs.FirstOrDefault(r => r.Start == 1 && r.End == 2);
        Assert.That(greenRun, Is.Not.Null);
        Assert.That(greenRun!.Format.ForegroundColor, Is.EqualTo(green));
    }

    [Test]
    public void BoldToggle_IsIndependentUndoEntryFromTyping()
    {
        // The bug: ApplyCharacterFormat did not record an undo entry, so Ctrl+Z
        // would pop the previous typing entry instead.
        var document = new RichEditDocument();
        InsertText(document, 0, "H");
        InsertText(document, 1, "i");
        document.Selection.SetRange(0, 2);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;

        document.Undo(); // undoes the bold only

        AssertText(document, "Hi");
        AssertBoldRuns(document);
        Assert.That(document.CanUndo(), Is.True, "typing entries must still be on the undo stack");
    }

    [Test]
    public void CaretInputFormat_PrefersCaretOverDefault()
    {
        var document = CreateDocument(string.Empty);

        var defaultFormat = (LeXtudio.UI.Text.TextCharacterFormat)document.GetDefaultCharacterFormat();
        defaultFormat.Bold = FormatEffect.Off;
        document.SetDefaultCharacterFormat(defaultFormat);

        document.Selection.SetRange(0, 0);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        document.Selection.TypeText("x");

        AssertText(document, "x");
        AssertBoldRuns(document, (0, 1));
    }

    [Test]
    public void CaretInputFormat_ClearsWhenCaretMoves()
    {
        var document = CreateDocument(string.Empty);

        document.Selection.SetRange(0, 0);
        document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        document.Selection.TypeText("x");
        AssertBoldRuns(document, (0, 1));

        document.Selection.MoveLeft(TextRangeUnit.Character, 1, false);
        document.Selection.TypeText("y");

        AssertText(document, "yx");
        var runs = document.GetCharacterFormatRuns().Where(run => run.Format.Bold == FormatEffect.On).ToList();
        Assert.That(runs, Has.Count.EqualTo(1));
        Assert.That((runs[0].Start, runs[0].End), Is.EqualTo((1, 2)));
    }

    [Test]
    public void CollapsedSelectionAtRunEnd_ReportsLeftRunFormat()
    {
        var document = CreateDocument("Hi");
        document.GetRange(0, 2).CharacterFormat.Bold = FormatEffect.On;
        document.GetRange(0, 2).CharacterFormat.Italic = FormatEffect.On;

        document.Selection.SetRange(2, 2);

        Assert.That(document.Selection.CharacterFormat.Bold, Is.EqualTo(FormatEffect.On));
        Assert.That(document.Selection.CharacterFormat.Italic, Is.EqualTo(FormatEffect.On));
    }

    [Test]
    public void ParagraphRanges_NewlineCreatesSplit()
    {
        var document = CreateDocument("ab");

        InsertText(document, 1, "\n");

        AssertText(document, "a\nb");
        AssertParagraphRanges(document, (0, 1), (2, 3));
    }

    [Test]
    public void ApplyFormatAcrossNewline_DoesNotCreateCrossParagraphRun()
    {
        var document = CreateDocument("ab\ncd");
        document.GetRange(0, 5).CharacterFormat.Bold = FormatEffect.On;

        AssertBoldRuns(document, (0, 2), (3, 5));
        AssertParagraphRanges(document, (0, 2), (3, 5));
    }

    [Test]
    public void InsertNewlineInsideRun_SplitsRunAtBoundary()
    {
        var document = CreateDocument("abcd");
        document.GetRange(0, 4).CharacterFormat.Bold = FormatEffect.On;

        InsertText(document, 2, "\n");

        AssertText(document, "ab\ncd");
        AssertBoldRuns(document, (0, 2), (3, 5));
        AssertParagraphRanges(document, (0, 2), (3, 5));
    }

    [Test]
    public void DeleteNewline_MergesEquivalentAdjacentRuns()
    {
        var document = CreateDocument("ab\ncd");
        document.GetRange(0, 5).CharacterFormat.Bold = FormatEffect.On;

        DeleteRange(document, 2, 3);

        AssertText(document, "abcd");
        AssertBoldRuns(document, (0, 4));
        AssertParagraphRanges(document, (0, 4));
    }

    [Test]
    public void UndoRedo_NewlineInsert_PreservesParagraphBoundaries()
    {
        var document = CreateDocument("abcd");

        InsertText(document, 2, "\n");
        AssertParagraphRanges(document, (0, 2), (3, 5));

        document.Undo();
        AssertParagraphRanges(document, (0, 4));

        document.Redo();
        AssertParagraphRanges(document, (0, 2), (3, 5));
    }

    [Test]
    public void Typing_BoldThenItalicOnly_CreatesExpectedRuns()
    {
        var document = CreateDocument(string.Empty);
        document.Selection.SetRange(0, 0);

        document.Selection.CharacterFormat.Bold = FormatEffect.On;
        document.Selection.CharacterFormat.Italic = FormatEffect.Off;
        document.Selection.TypeText("Hello ");

        document.Selection.CharacterFormat.Bold = FormatEffect.Off;
        document.Selection.CharacterFormat.Italic = FormatEffect.On;
        document.Selection.TypeText("world!");

        AssertText(document, "Hello world!");

        var runs = document.GetCharacterFormatRuns();
        Assert.That(runs, Has.Count.EqualTo(2));

        Assert.That((runs[0].Start, runs[0].End), Is.EqualTo((0, 6)));
        Assert.That(runs[0].Format.Bold, Is.EqualTo(FormatEffect.On));
        Assert.That(runs[0].Format.Italic, Is.EqualTo(FormatEffect.Off));

        Assert.That((runs[1].Start, runs[1].End), Is.EqualTo((6, 12)));
        Assert.That(runs[1].Format.Bold, Is.EqualTo(FormatEffect.Off));
        Assert.That(runs[1].Format.Italic, Is.EqualTo(FormatEffect.On));
    }

    private static RichEditDocument CreateDocument(string text)
    {
        var document = new RichEditDocument();
        document.SetText(TextSetOptions.None, text);
        return document;
    }

    private static void AssertText(RichEditDocument document, string expected)
    {
        document.GetText(TextGetOptions.None, out var text);
        Assert.That(text, Is.EqualTo(expected));
    }

    private static void AssertBoldRuns(RichEditDocument document, params (int start, int end)[] expected)
    {
        var runs = document.GetCharacterFormatRuns().Where(run => run.Format.Bold == FormatEffect.On).ToList();
        Assert.That(runs, Has.Count.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
            Assert.That((runs[i].Start, runs[i].End), Is.EqualTo(expected[i]));
    }

    private static void AssertUnderlineRuns(RichEditDocument document, params (int start, int end)[] expected)
    {
        var runs = document.GetCharacterFormatRuns().Where(run => run.Format.Underline != UnderlineType.None).ToList();
        Assert.That(runs, Has.Count.EqualTo(expected.Length));

        for (int i = 0; i < expected.Length; i++)
            Assert.That((runs[i].Start, runs[i].End), Is.EqualTo(expected[i]));
    }

    private static void InsertText(RichEditDocument document, int offset, string text, LeXtudio.UI.Text.TextCharacterFormat? format = null)
    {
        var method = typeof(RichEditDocument).GetMethod("InsertText", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "InsertText internal method was not found.");
        method!.Invoke(document, new object?[] { offset, text, format });
    }

    private static void DeleteRange(RichEditDocument document, int start, int end)
    {
        var method = typeof(RichEditDocument).GetMethod("DeleteRange", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "DeleteRange internal method was not found.");
        method!.Invoke(document, new object?[] { start, end });
    }

    private static LeXtudio.UI.Text.TextCharacterFormat GetCharacterFormat(RichEditDocument document, int start, int end)
    {
        var method = typeof(RichEditDocument).GetMethod("GetCharacterFormat", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "GetCharacterFormat internal method was not found.");
        var result = method!.Invoke(document, new object?[] { start, end });
        Assert.That(result, Is.Not.Null, "GetCharacterFormat returned null.");
        return (LeXtudio.UI.Text.TextCharacterFormat)result!;
    }

    private static void AssertParagraphRanges(RichEditDocument document, params (int start, int end)[] expected)
    {
        var method = typeof(RichEditDocument).GetMethod("GetParagraphRanges", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, "GetParagraphRanges internal method was not found.");

        var result = method!.Invoke(document, null);
        Assert.That(result, Is.Not.Null, "GetParagraphRanges returned null.");

        var actual = ((IEnumerable)result!)
            .Cast<object>()
            .Select(item =>
            {
                var type = item.GetType();
                var start = (int)(type.GetProperty("Start")!.GetValue(item) ?? -1);
                var end = (int)(type.GetProperty("End")!.GetValue(item) ?? -1);
                return (start, end);
            })
            .ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }
}
