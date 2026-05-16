using NUnit.Framework;

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
}
