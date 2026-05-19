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
    public async Task Text_ReturnsPlainTextAcrossParagraphs()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("Alpha")));
            document.Blocks.Add(new Paragraph(new Run("Beta")));

            var range = new TextRange(document.ContentStart, document.ContentEnd);

            Assert.That(range.Text, Does.Contain("Alpha"));
            Assert.That(range.Text, Does.Contain("Beta"));
            Assert.That(range.Text, Does.Contain("\r\n"));
        });
    }

    [Test]
    public async Task Text_ReturnsSlicedPlainTextAcrossParagraphs()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var firstParagraph = new Paragraph(new Run("Alpha"));
            var secondParagraph = new Paragraph(new Run("Beta"));
            document.Blocks.Add(firstParagraph);
            document.Blocks.Add(secondParagraph);

            // paragraph.ContentStart sits one symbol before the Run's
            // element-start; +1 lands inside the run's text content.
            // firstParagraph.ContentStart + 4 = between 'p' and 'h' of "Alpha".
            // secondParagraph.ContentStart + 3 = between 'e' and 't' of "Beta".
            var start = firstParagraph.ContentStart.GetPositionAtOffset(4)
                        ?? document.ContentStart;
            var end = secondParagraph.ContentStart.GetPositionAtOffset(3)
                      ?? document.ContentEnd;

            var range = new TextRange(start, end);

            Assert.That(range.Text, Is.EqualTo("ha\r\nBe"));
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
