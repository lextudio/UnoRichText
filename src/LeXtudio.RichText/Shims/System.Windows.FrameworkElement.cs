namespace System.Windows;

public class FrameworkElement : DependencyObject
{
    public static readonly DependencyProperty FlowDirectionProperty =
        DependencyProperty.Register(
            nameof(FlowDirection),
            typeof(FlowDirection),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(FlowDirection.LeftToRight));

    public FlowDirection FlowDirection
    {
        get => (FlowDirection)(GetValue(FlowDirectionProperty) ?? FlowDirection.LeftToRight);
        set => SetValue(FlowDirectionProperty, value);
    }
}
