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
