using System.Collections.Generic;
using System.Windows;
using Marv.Common.Graph;

namespace Marv_Excel
{
    public partial class VertexSelectionControl
    {
        public static readonly DependencyProperty VerticesProperty =
            DependencyProperty.Register("Vertices", typeof (IEnumerable<Vertex>), typeof (VertexSelectionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty nYearsProperty =
            DependencyProperty.Register("nYears", typeof (int), typeof (VertexSelectionControl), new PropertyMetadata(1));

        public static readonly RoutedEvent DoneButtonClickedEvent =
            EventManager.RegisterRoutedEvent("DoneButtonClicked", RoutingStrategy.Bubble, typeof (RoutedEventHandler<RoutedEventArgs>), typeof (VertexSelectionControl));

        public VertexSelectionControl()
        {
            InitializeComponent();

            this.Loaded += VerticesSelectionControl_Loaded;
        }

        public IEnumerable<Vertex> SelectedVertices
        {
            get
            {
                var selectedVertices = new List<Vertex>();

                foreach (var item in this.VerticesListBox.SelectedItems)
                {
                    selectedVertices.Add(item as Vertex);
                }

                return selectedVertices;
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                return (IEnumerable<Vertex>) GetValue(VerticesProperty);
            }
            set
            {
                SetValue(VerticesProperty, value);
            }
        }

        public int nYears
        {
            get
            {
                return (int) GetValue(nYearsProperty);
            }
            set
            {
                SetValue(nYearsProperty, value);
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs
            {
                RoutedEvent = DoneButtonClickedEvent,
            });
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticesListBox.SelectAll();
        }

        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticesListBox.UnselectAll();
        }

        private void VerticesSelectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectAllButton.Click += SelectAllButton_Click;
            this.SelectNoneButton.Click += SelectNoneButton_Click;
            this.DoneButton.Click += DoneButton_Click;
        }

        public event RoutedEventHandler<RoutedEventArgs> DoneButtonClicked
        {
            add
            {
                AddHandler(DoneButtonClickedEvent, value);
            }
            remove
            {
                RemoveHandler(DoneButtonClickedEvent, value);
            }
        }
    }
}