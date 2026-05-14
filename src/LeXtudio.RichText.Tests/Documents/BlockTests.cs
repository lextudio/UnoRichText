using System.Windows.Documents;
using NUnit.Framework;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class BlockTests
{
    [Test]
    public void Section_OwnsBlockCollection()
    {
        var section = new Section();
        var paragraph = new Paragraph(new Run("hello"));

        section.Blocks.Add(paragraph);

        Assert.That(section.Blocks, Has.Count.EqualTo(1));
        Assert.That(section.Blocks[0], Is.SameAs(paragraph));
    }

    [Test]
    public void Section_TracksBlockRemoval()
    {
        var section = new Section();
        var first = new Paragraph(new Run("one"));
        var second = new Paragraph(new Run("two"));

        section.Blocks.Add(first);
        section.Blocks.Add(second);
        section.Blocks.Remove(first);

        Assert.That(section.Blocks, Has.Count.EqualTo(1));
        Assert.That(section.Blocks[0], Is.SameAs(second));
    }

    [Test]
    public void List_OwnsListItemCollection()
    {
        var list = new List();
        var item = new ListItem(new Paragraph(new Run("hello")));

        list.ListItems.Add(item);

        Assert.That(list.ListItems, Has.Count.EqualTo(1));
        Assert.That(list.ListItems[0], Is.SameAs(item));
        Assert.That(item.Blocks, Has.Count.EqualTo(1));
    }
}
