using System.Windows;
using System.Windows.Input;

namespace Marv.Controls
{
    public partial class EditableTextBox
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (EditableTextBox), new PropertyMetadata(false));

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof (Thickness), typeof (EditableTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (EditableTextBox), new PropertyMetadata(null));

        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Thickness TextMargin
        {
            get { return (Thickness) GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public EditableTextBox()
        {
            InitializeComponent();
        }

        private void EditableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                this.IsEditable = false;
                this.ReleaseMouseCapture();
            }
        }

        private void EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsEditable = false;
            this.ReleaseMouseCapture();
        }

        private void EditableTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsEditable && e.ChangedButton == MouseButton.Right)
            {
                this.IsEditable = true;
                Keyboard.Focus(this.TextBox);
                Mouse.Capture(this);
            }
            else
            {
                var position = e.GetPosition(this);

                if (position.X < 0 || position.X > this.ActualWidth ||
                    position.Y < 0 || position.Y > this.ActualHeight)
                {
                    this.IsEditable = false;
                    this.ReleaseMouseCapture();
                }
                else
                {
                    if (this.IsEditable)
                    {
                        this.TextBox.RaiseEvent(e);
                    }
                    else
                    {
                        this.GetParent<FrameworkElement>().RaiseEvent(e);
                    }
                }
            }
        }
    }
}