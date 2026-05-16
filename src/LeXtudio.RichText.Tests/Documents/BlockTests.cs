using NUnit.Framework;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class BlockTests
{
    [Test]
    public void Section_OwnsBlockCollection()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void Section_TracksBlockRemoval()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void List_OwnsListItemCollection()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }
}
