using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using LeXtudio.RichText.Tests.Runtime;
using MS.Internal.Commands;
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
            Assert.That(new TextRange(run.ContentStart, run.ContentEnd).Text, Is.EqualTo("Hello"));
            Assert.That(box.ShouldSerializeDocument(), Is.True);
        });
    }

    [Test]
    public async Task SelectionFormatting_CanBeAppliedAtCaret()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var box = new WpfRichTextBox();

            box.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            box.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);

            Assert.That(box.Selection.GetPropertyValue(TextElement.FontWeightProperty), Is.EqualTo(FontWeights.Bold));
            Assert.That(box.Selection.GetPropertyValue(TextElement.FontStyleProperty), Is.EqualTo(FontStyles.Italic));
        });
    }

    [Test]
    public async Task RoutedCommand_ClassHandlers_RespectRegisteredControlType()
    {
        await UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var command = new RoutedCommand("TestCommand", typeof(WpfRichTextBox));
            int richTextBoxExecutions = 0;
            int flowDocumentExecutions = 0;

            CommandHelpers.RegisterCommandHandler(
                typeof(WpfRichTextBox),
                command,
                (_, e) =>
                {
                    richTextBoxExecutions++;
                    e.Handled = true;
                });

            CommandHelpers.RegisterCommandHandler(
                typeof(FlowDocument),
                command,
                (_, e) =>
                {
                    flowDocumentExecutions++;
                    e.Handled = true;
                });

            var box = new WpfRichTextBox();
            var document = new FlowDocument();

            command.Execute(null, box);
            Assert.That(richTextBoxExecutions, Is.EqualTo(1));
            Assert.That(flowDocumentExecutions, Is.EqualTo(0));

            command.Execute(null, document);
            Assert.That(richTextBoxExecutions, Is.EqualTo(1));
            Assert.That(flowDocumentExecutions, Is.EqualTo(1));
        });
    }
}
