using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using LeXtudio.RichText.Tests.Runtime;
using NUnit.Framework;
using WpfRichTextBox = System.Windows.Controls.RichTextBox;

namespace LeXtudio.RichText.Tests.Controls;

[TestFixture]
public sealed class WpfRichTextBoxShellTests
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
    public async Task Constructor_CreatesImplicitEmptyFlowDocument()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var box = new WpfRichTextBox();

            Assert.That(box.Document, Is.Not.Null);
            Assert.That(box.Document.Blocks, Has.Count.EqualTo(1));
            Assert.That(box.Document.Blocks.FirstBlock, Is.TypeOf<Paragraph>());
            Assert.That(box.ShouldSerializeDocument(), Is.False);
        });
    }

    [Test]
    public async Task AssignedDocument_IsOwnedAndSerializable()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run("Hello"));
            document.Blocks.Add(paragraph);

            var box = new WpfRichTextBox(document);

            Assert.That(box.Document, Is.SameAs(document));
            Assert.That(document.Parent, Is.SameAs(box));
            Assert.That(box.ShouldSerializeDocument(), Is.True);
        });
    }

    [Test]
    public async Task Document_CannotBeSharedAcrossRichTextBoxes()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var first = new WpfRichTextBox(document);
            _ = first;

            Assert.Throws<ArgumentException>(() => _ = new WpfRichTextBox(document));
        });
    }

    [Test]
    public async Task AddChild_ReplacesOnlyImplicitDocument()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var box = new WpfRichTextBox();
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("Child document")));

            ((IAddChild)box).AddChild(document);

            Assert.That(box.Document, Is.SameAs(document));
            Assert.Throws<ArgumentException>(() => ((IAddChild)box).AddChild(new FlowDocument()));
        });
    }

    [Test]
    public async Task AppendText_CreatesRunInLastParagraph()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var box = new WpfRichTextBox();

            box.AppendText("Hello");

            var paragraph = (Paragraph)box.Document.Blocks.LastBlock!;
            var run = (Run)paragraph.Inlines.LastInline!;
            Assert.That(run.Text, Is.EqualTo("Hello"));
            Assert.That(box.ShouldSerializeDocument(), Is.True);
        });
    }

    [Test]
    public async Task AppendText_UsesCurrentTypingFormat()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var box = new WpfRichTextBox();

            ApplyTypingProperty(box, TextElement.FontWeightProperty, FontWeights.Bold);
            ApplyTypingProperty(box, TextElement.FontStyleProperty, FontStyles.Italic);
            box.AppendText("Formatted");

            var paragraph = (Paragraph)box.Document.Blocks.LastBlock!;
            var run = (Run)paragraph.Inlines.LastInline!;
            Assert.That(run.FontWeight, Is.EqualTo(FontWeights.Bold));
            Assert.That(run.FontStyle, Is.EqualTo(FontStyles.Italic));
        });
    }

    private static void ApplyTypingProperty(WpfRichTextBox box, DependencyProperty property, object value)
    {
        var method = typeof(WpfRichTextBox).GetMethod("ApplyTypingProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null);
        method!.Invoke(box, [property, value]);
    }
}
