using NUnit.Framework;

namespace LeXtudio.RichText.Tests.Controls;

[TestFixture]
public sealed class OverflowLayoutTests : TestBase
{
    [Test]
    public void Overflow_StartsAtWholeLineBoundary()
    {
        SampleDiagnostics.AssertPassMarker("OVERFLOW: PASS");
    }

    [Test]
    public void GalleryOverflow_UsesLinkedColumns()
    {
        SampleDiagnostics.AssertPassMarker("GALLERY_OVERFLOW: PASS");
    }
}
