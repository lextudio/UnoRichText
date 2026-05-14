namespace System.Windows.Documents;

public interface ITextPointer
{
    int CompareTo(ITextPointer position);
    TextPointerContext GetPointerContext(LogicalDirection direction);
    void MoveToNextContextPosition(LogicalDirection direction);
    System.Type? ParentType { get; }
}
