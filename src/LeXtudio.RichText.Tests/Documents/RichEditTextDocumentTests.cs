using Microsoft.UI.Text;
using NUnit.Framework;
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
}
