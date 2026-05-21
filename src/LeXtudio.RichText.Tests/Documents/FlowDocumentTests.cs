using NUnit.Framework;
using System.Linq;
using MS.Internal.Florence;
using FlowDocument = System.Windows.Documents.FlowDocument;
using LogicalDirection = System.Windows.Documents.LogicalDirection;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;
using TextPointer = System.Windows.Documents.TextPointer;
using TextRange = System.Windows.Documents.TextRange;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class FlowDocumentTests
{
    [Test]
    public void FlowDocument_OwnsBlockCollection()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void FlowDocument_CanAttachRichTextBlockLayoutHost()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public async Task TextContainer_IMECharCountIncludesRunText()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("abcdef")));

            Assert.That(document.TextContainer.IMECharCount, Is.GreaterThanOrEqualTo(6));
            Assert.That(new TextRange(document.ContentStart, document.ContentEnd).Text, Does.Contain("abcdef"));
        });
    }

    [Test]
    public async Task TextContainer_CanCreatePointersAtVisibleCharOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("abcdef")));

            TextPointer? pointer = document.TextContainer.CreatePointerAtCharOffset(4, LogicalDirection.Forward);

            Assert.That(pointer, Is.Not.Null);
            Assert.That(pointer!.CharOffset, Is.EqualTo(4));
        });
    }

    [Test]
    public async Task TextContainer_IMECharCountIncludesObjectInitializerRunText()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = "abcdef" });
            document.Blocks.Add(paragraph);

            Assert.That(document.TextContainer.IMECharCount, Is.GreaterThanOrEqualTo(6));
            Assert.That(new TextRange(document.ContentStart, document.ContentEnd).Text, Does.Contain("abcdef"));
        });
    }

    [Test]
    public async Task FlorenceLayout_UsesDocumentWideRunOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run { Text = "First paragraph with enough text to wrap at a narrow width." }));
            document.Blocks.Add(new Paragraph(new Run { Text = "Second paragraph should not restart run offsets at zero." }));

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(120, double.PositiveInfinity));

            foreach (var line in page.Lines)
            foreach (var run in line.Runs)
                Assert.That(run.StartOffset, Is.GreaterThanOrEqualTo(line.StartOffset));
        });
    }

    [Test]
    public async Task FlorenceLayout_DoesNotDoubleCountParagraphInlineOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = "This sample hosts a " });
            paragraph.Inlines.Add(new Run { Text = "FlowDocument" });
            paragraph.Inlines.Add(new Run { Text = " inside the ported " });
            paragraph.Inlines.Add(new Run { Text = "System.Windows.Controls.RichTextBox" });
            paragraph.Inlines.Add(new Run { Text = "." });
            document.Blocks.Add(paragraph);

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(600, double.PositiveInfinity));
            var plainText = new TextRange(document.ContentStart, document.ContentEnd).Text;
            int visibleTextLength = paragraph.Inlines
                .OfType<Run>()
                .Sum(run => new TextRange(run.ContentStart, run.ContentEnd).Text.Length);

            Assert.That(page.Lines, Is.Not.Empty);
            Assert.That(page.Lines[^1].EndOffset, Is.LessThanOrEqualTo(document.TextContainer.IMECharCount));
            Assert.That(page.Lines[^1].EndOffset, Is.EqualTo(visibleTextLength));
            Assert.That(plainText.Length, Is.GreaterThanOrEqualTo(visibleTextLength));

            foreach (var line in page.Lines)
            foreach (var run in line.Runs)
            {
                Assert.That(run.StartOffset, Is.GreaterThanOrEqualTo(0));
                Assert.That(run.EndOffset, Is.LessThanOrEqualTo(document.TextContainer.IMECharCount));
            }
        });
    }

    [Test]
    public async Task TextMeasurer_PreservesTrailingWhitespaceWidth()
    {
        // Regression for session 39: TextBlock.DesiredSize.Width strips trailing
        // whitespace, which collapsed the caret backwards across spaces. Our
        // sentinel-based MeasureWidth must report a wider value for "abc " than "abc".
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            const double FontSize = 14;
            double withoutSpace  = TextMeasurer.MeasureWidth("abc",   FontSize, bold: false, italic: false);
            double withOneSpace  = TextMeasurer.MeasureWidth("abc ",  FontSize, bold: false, italic: false);
            double withTwoSpaces = TextMeasurer.MeasureWidth("abc  ", FontSize, bold: false, italic: false);

            Assert.That(withOneSpace,  Is.GreaterThan(withoutSpace),
                "Trailing space must add width — TextBlock trim is not being compensated.");
            Assert.That(withTwoSpaces, Is.GreaterThan(withOneSpace),
                "Each additional trailing space must add width.");
        });
    }

    [Test]
    public async Task FlorenceLayout_PrefixWidthGrowsMonotonicallyAcrossSpaces()
    {
        // Regression for session 39: caret X for offsets that straddle a whitespace
        // boundary must move forward, not flatten or jump backward.
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run { Text = "abc def ghi" }));

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(600, double.PositiveInfinity));
            Assert.That(page.Lines, Is.Not.Empty);
            var line = page.Lines[0];
            Assert.That(line.Runs, Is.Not.Empty);
            var run = line.Runs[0];

            double prev = -1;
            for (int i = 0; i <= run.Length; i++)
            {
                double x = TextMeasurer.MeasurePrefixWidth(run, i);
                Assert.That(x, Is.GreaterThanOrEqualTo(prev),
                    $"Prefix width must be monotonic but went backwards at i={i} ('{run.Text[..i]}'): {x} < {prev}.");
                if (i > 0 && run.Text[i - 1] == ' ')
                {
                    // The space at position i-1 must contribute real width — otherwise
                    // the caret will not visibly move when stepped across a space.
                    double prior = TextMeasurer.MeasurePrefixWidth(run, i - 1);
                    Assert.That(x, Is.GreaterThan(prior),
                        $"Crossing space at i={i} produced no advance: x={x} prior={prior}.");
                }
                prev = x;
            }
        });
    }

    [Test]
    public async Task FlorenceLayout_TracksParagraphBoundariesOnSharedOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run { Text = "First paragraph." }));
            document.Blocks.Add(new Paragraph(new Run { Text = "Second paragraph." }));

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(1200, double.PositiveInfinity));
            Assert.That(page.Lines.Count, Is.GreaterThanOrEqualTo(2));

            var first = page.Lines[0];
            var second = page.Lines[1];

            Assert.That(second.StartOffset, Is.EqualTo(first.EndOffset + 1),
                "Florence should reserve one invisible paragraph-boundary slot between adjacent paragraphs, mirroring WPF logical caret positions.");
        });
    }

    [Test]
    public async Task FlorenceLayout_ParagraphBoundarySlotPrecedesNextParagraphFirstCharacter()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run { Text = "x." }));
            document.Blocks.Add(new Paragraph(new Run { Text = "First" }));

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(1200, double.PositiveInfinity));
            Assert.That(page.Lines.Count, Is.GreaterThanOrEqualTo(2));

            var first = page.Lines[0];
            var second = page.Lines[1];
            Assert.That(second.StartOffset, Is.EqualTo(first.EndOffset + 1));

            // Simulate caret stepping by logical char offsets:
            // before 'x' (0) -> between 'x' and '.' (1) -> after '.' (2)
            // -> paragraph boundary slot (3) -> before 'i' in "First" (4).
            int afterDot = first.EndOffset;
            int boundarySlot = afterDot + 1;
            int nextParagraphFirstCharLeftEdge = second.StartOffset;
            Assert.That(boundarySlot, Is.EqualTo(nextParagraphFirstCharLeftEdge),
                "One additional Right move after paragraph end should land at the left edge of the next paragraph first character, not skip over it.");
        });
    }
}
