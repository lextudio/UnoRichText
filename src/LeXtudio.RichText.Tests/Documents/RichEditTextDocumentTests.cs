using Microsoft.UI.Text;
using NUnit.Framework;
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
}
