using Marv.Common;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Marv.Controls
{
    public partial class FileBrowseControl : UserControl
    {
        public static readonly RoutedEvent FileNameChangedEvent =
        EventManager.RegisterRoutedEvent("FileNameChanged", RoutingStrategy.Bubble, typeof(ValueEventHandler<string>), typeof(FileBrowseControl));

        public static readonly DependencyProperty FileNameProperty =
        DependencyProperty.Register("FileName", typeof(string), typeof(FileBrowseControl), new PropertyMetadata(""));

        public FileBrowseControl()
        {
            InitializeComponent();

            this.BrowseButton.Click += BrowseButton_Click;
        }

        public event ValueEventHandler<string> FileNameChanged
        {
            add { AddHandler(FileNameChangedEvent, value); }
            remove { RemoveHandler(FileNameChangedEvent, value); }
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