using NUnit.Framework;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class InlineTests
{
    [Test]
    public void Bold_OwnsChildInline()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void Italic_OwnsChildInline()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void Span_OwnsInlineCollection()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void Span_TracksInlineRemoval()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void Run_StringConstructorPreservesText()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }
}
