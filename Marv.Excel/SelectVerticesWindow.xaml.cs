using System.Collections.Generic;
using System.Windows;
using Marv.Common.Graph;

namespace Marv.Excel
{
    public partial class SelectVerticesWindow : Window
    {
        public IEnumerable<Vertex> Vertices
        {
            get { return (IEnumerable<Vertex>)GetValue(VerticesProperty); }
            set { SetValue(VerticesProperty, value); }
        }

        public static readonly DependencyProperty VerticesProperty =
        DependencyProperty.Register("Vertices", typeof(IEnumerable<Vertex>), typeof(SelectVerticesWindow), new PropertyMetadata(null));

        public SelectVerticesWindow()
        {
            InitializeComponent();

            this.Loaded += SelectVerticesWindow_Loaded;
        }

        private void SelectVerticesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectAllButton.Click += SelectAllButton_Click;
            this.SelectNoneButton.Click += SelectNoneButton_Click;
            this.DoneButton.Click += DoneButton_Click;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticesListBox.UnselectAll();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticesListBox.SelectAll();
        }
    }
}