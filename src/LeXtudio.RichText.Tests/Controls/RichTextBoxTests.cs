using LeXtudio.UI.Xaml.Controls;
using LeXtudio.RichText.Tests.Runtime;
using NUnit.Framework;
using FlowDocument = System.Windows.Documents.FlowDocument;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;

namespace LeXtudio.RichText.Tests.Controls;
#if RICHTEXTBOX
[TestFixture]
public sealed class RichTextBoxTests
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
    public async Task Constructor_CreatesFlowDocument()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var control = new RichTextBox();

            Assert.That(control.Document, Is.Not.Null);
        });
    }

    [Test]
    public async Task AssignedDocument_IsRetained()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run("Hello"));
            document.Blocks.Add(paragraph);

            var control = new RichTextBox
            {
                Document = document,
            };

            Assert.That(control.Document, Is.SameAs(document));
            Assert.That(control.Document.Blocks, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task ReplacingDocument_ReattachesTextLayoutHost()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var control = new RichTextBox();
            var firstDocument = control.Document;
            var secondDocument = new FlowDocument();

            Assert.That(firstDocument.TextLayoutHost, Is.Not.Null);
            Assert.That(secondDocument.TextLayoutHost, Is.Null);

            control.Document = secondDocument;

            Assert.That(firstDocument.TextLayoutHost, Is.Null);
            Assert.That(secondDocument.TextLayoutHost, Is.Not.Null);
            Assert.That(control.Document, Is.SameAs(secondDocument));
        });
    }

    [Test]
    public async Task MutatingAssignedDocumentBlocks_KeepsSelectionApisCoherent()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("Alpha")));

            var control = new RichTextBox
            {
                Document = document,
            };

            document.Blocks.Add(new Paragraph(new Run("Beta")));

            Assert.That(control.Document.Blocks, Has.Count.EqualTo(2));

            control.SelectAll();

            Assert.That(control.SelectionStart.Offset, Is.LessThanOrEqualTo(control.SelectionEnd.Offset));
            Assert.That(control.SelectedText, Does.Contain("Alpha"));
            Assert.That(control.SelectedText, Does.Contain("Beta"));
        });
    }
}
#endif
