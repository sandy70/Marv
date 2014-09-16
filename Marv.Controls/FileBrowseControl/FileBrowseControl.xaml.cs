using Marv;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Marv.Controls
{
    public partial class FileBrowseControl : UserControl
    {
        public static readonly DependencyProperty FileNameProperty =
        DependencyProperty.Register("FileName", typeof(string), typeof(FileBrowseControl), new PropertyMetadata("", ChangedFileName));

        private static void ChangedFileName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileBrowseControl;
            control.RaiseEvent(new ValueEventArgs<string> { Value = control.FileName });
        }

        public FileBrowseControl()
        {
            InitializeComponent();

            this.BrowseButton.Click += BrowseButton_Click;
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false
            };

            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                this.FileName = openFileDialog.FileName;
            }
        }
    }
}