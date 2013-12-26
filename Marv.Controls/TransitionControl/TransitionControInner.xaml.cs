using System.Windows;
using System.Windows.Controls;

namespace Marv.Controls
{
    public partial class TransitionControlInner : Grid
    {
        public static readonly DependencyProperty SelectedElementProperty =
        DependencyProperty.Register("SelectedElement", typeof(FrameworkElement), typeof(TransitionControlInner), new PropertyMetadata(null));

        public TransitionControlInner()
        {
            InitializeComponent();
        }

        public FrameworkElement SelectedElement
        {
            get { return (FrameworkElement)GetValue(SelectedElementProperty); }
            set { SetValue(SelectedElementProperty, value); }
        }
    }
}