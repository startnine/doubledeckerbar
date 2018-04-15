using System.Windows;

namespace DoubleDeckerBar
{
    public class DoubleContentWindow : Window
    {
        public DoubleContentWindow()
        {

        }

        public object SecondContent
        {
            get => GetValue(SecondContentProperty);
            set => SetValue(SecondContentProperty, (value));
        }

        public static readonly DependencyProperty SecondContentProperty =
            DependencyProperty.Register("SecondContent", typeof(object), typeof(DoubleContentWindow), new PropertyMetadata());
    }
}
