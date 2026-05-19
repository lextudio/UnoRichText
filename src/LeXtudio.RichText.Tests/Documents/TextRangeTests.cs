using LeXtudio.RichText.Tests.Runtime;
using NUnit.Framework;
using FlowDocument = System.Windows.Documents.FlowDocument;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;
using TextRange = System.Windows.Documents.TextRange;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class TextRangeTests
{
    [OneTimeSetUp]
    public async Task RequireUnoRuntimeHost()
    {
        try
        {
            await UnoRuntimeTestHost.EnsureAvailableAsync();
        }
        catch (Exception ex)
        {
            Assert.Ignore($"Uno runtime test host is not available: {ex.Message}");
        }
    }

    [Test]
    public async Task Text_ReturnsRunPlainText()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var run = new Run("Alpha");
            _ = new Paragraph(run);

            var range = new TextRange(run.ContentStart, run.ContentEnd);

            Assert.That(range.Text, Is.EqualTo("Alpha"));
        });
    }

    [Test]
    public async Task Text_ReturnsSlicedPlainTextWithinRun()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var run = new Run("Alpha");
            _ = new Paragraph(run); // attach so ContentStart is in a tree

            var start = run.ContentStart.GetPositionAtOffset(1) ?? run.ContentStart;
            var end = run.ContentStart.GetPositionAtOffset(4) ?? run.ContentEnd;

            var range = new TextRange(start, end);

            Assert.That(range.Text, Is.EqualTo("lph"));
        });
    }

    [Test]
    public async Task Diagnose_ParagraphPointerWalk()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var run = new Run("Alpha");
            var paragraph = new Paragraph(run);

            var paraStart = ((System.Windows.Documents.ITextPointer)paragraph.ContentStart).CreatePointer();
            var paraEnd = (System.Windows.Documents.ITextPointer)paragraph.ContentEnd;
            var compare = paraStart.CompareTo(paraEnd);
            var contextFwd = paraStart.GetPointerContext(System.Windows.Documents.LogicalDirection.Forward);

            TestContext.WriteLine($"paragraph.ContentStart.CompareTo(ContentEnd)={compare}");
            TestContext.WriteLine($"paragraph.ContentStart.GetPointerContext(Forward)={contextFwd}");

            var runStart = (System.Windows.Documents.ITextPointer)run.ContentStart;
            TestContext.WriteLine($"paragraph.ContentStart.CompareTo(run.ContentStart)={paraStart.CompareTo(runStart)}");

            Assert.Pass();
        });
    }

    [Test]
    public async Task Text_EmptyRangeReturnsEmptyString()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("Alpha")));

            var pointer = document.ContentStart;
            var range = new TextRange(pointer, pointer);

            Assert.That(range.Text, Is.EqualTo(string.Empty));
        });
    }
}
