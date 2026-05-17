using LeXtudio.UI.Xaml.Controls;
using NUnit.Framework;
using FlowDocument = System.Windows.Documents.FlowDocument;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;

namespace LeXtudio.RichText.Tests.Controls;

[TestFixture]
public sealed class RichTextBoxTests
{
    [Test]
    [Ignore("Requires an active Uno dispatcher host.")]
    public void Constructor_CreatesFlowDocument()
    {
        var control = new RichTextBox();

        Assert.That(control.Document, Is.Not.Null);
    }

    [Test]
    [Ignore("Requires an active Uno dispatcher host.")]
    public void AssignedDocument_IsRetained()
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
    }
}
