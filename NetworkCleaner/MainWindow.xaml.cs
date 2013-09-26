using LibNetwork;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace NetworkCleaner
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty InputFileNameProperty =
        DependencyProperty.Register("InputFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty OutputFileNameProperty =
        DependencyProperty.Register("OutputFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register("StatusText", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public MainWindow()
        {
            InitializeComponent();

            this.CleanButton.Click += CleanButton_Click;
            this.InputBrowseButton.Click += InputBrowseButton_Click;
            this.OutputBrowseButton.Click += OutputBrowseButton_Click;
        }

        public string InputFileName
        {
            get { return (string)GetValue(InputFileNameProperty); }
            set { SetValue(InputFileNameProperty, value); }
        }

        public string OutputFileName
        {
            get { return (string)GetValue(OutputFileNameProperty); }
            set { SetValue(OutputFileNameProperty, value); }
        }

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            var structure = NetworkStructure.Read(this.InputFileName);
            // structure.FixStates();
            structure.Write(this.OutputFileName);
            this.StatusText = "Done!";
        }

        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".net",
                Filter = "Hugin network Files (.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.InputFileName = dialog.FileName;
                this.OutputFileName = Path.Combine(Path.GetDirectoryName(this.InputFileName), Path.GetFileNameWithoutExtension(this.InputFileName) + " Clean.net");
            }
        }

        private void OutputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".net",
                Filter = "Hugin network Files (.net)|*.net",
            };

            if (dialog.ShowDialog() == true)
            {
                this.OutputFileName = dialog.FileName;
            }
        }
    }
}