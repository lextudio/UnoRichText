using Microsoft.UI.Xaml;
using NUnit.Framework;
using System.Windows.Documents;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class FlowDocumentTests
{
    [Test]
    public void FlowDocument_OwnsBlockCollection()
    {
        var document = new FlowDocument();
        var paragraph = new Paragraph(new Run("hello"));

        document.Blocks.Add(paragraph);

        Assert.That(document.Blocks, Has.Count.EqualTo(1));
        Assert.That(document.Blocks[0], Is.SameAs(paragraph));
    }

    [Test]
    public void FlowDocument_CanAttachRichTextBlockLayoutHost()
    {
        var document = new FlowDocument();
        var host = new FakeTextLayoutHost();

        document.TextLayoutHost = host;

        Assert.That(document.TextLayoutHost, Is.SameAs(host));
    }

    private sealed class FakeTextLayoutHost : ITextLayoutHost
    {
        public UIElement RenderScope => throw new NotSupportedException();
        public bool IsLayoutValid => true;
        public double ViewportWidth => 0;
        public double ViewportHeight => 0;
        public double ExtentHeight => 0;
        public void InvalidateLayout()
        {
        }
    }
}
