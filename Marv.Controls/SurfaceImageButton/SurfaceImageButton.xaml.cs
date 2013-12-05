using Microsoft.Surface.Presentation.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Marv.Controls
{
    public partial class SurfaceImageButton : SurfaceButton
    {
        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(string), typeof(SurfaceImageButton), new PropertyMetadata("/Marv.Common;component/Resources/Icons/Close.png"));

        public SurfaceImageButton()
        {
            InitializeComponent();
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}