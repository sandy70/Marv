using System.Collections.Generic;
using System.Windows;
using Marv.Common.Graph;

namespace Marv_Excel
{
    public partial class VertexSelectionControl
    {
        public static readonly DependencyProperty VerticesProperty =
            DependencyProperty.Register("Vertices", typeof (IEnumerable<Vertex>), typeof (VertexSelectionControl), new PropertyMetadata(null));

        public static readonly RoutedEvent DoneButtonClickedEvent =
            EventManager.RegisterRoutedEvent("DoneButtonClicked", RoutingStrategy.Bubble, typeof (RoutedEventHandler<RoutedEventArgs>), typeof (VertexSelectionControl));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (VertexSelectionControl), new PropertyMetadata(2000));

        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (VertexSelectionControl), new PropertyMetadata(2000));

        public VertexSelectionControl()
        {
            InitializeComponent();

            this.Loaded += VerticesSelectionControl_Loaded;
        }

        public int EndYear
        {
            get
            {
                return (int) GetValue(EndYearProperty);
            }
            set
            {
                SetValue(EndYearProperty, value);
            }
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

        public int StartYear
        {
            get
            {
                return (int) GetValue(StartYearProperty);
            }
            set
            {
                SetValue(StartYearProperty, value);
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
            this.StartYearUpDown.ValueChanged += StartYearUpDown_ValueChanged;
            this.DoneButton.Click += DoneButton_Click;
        }

        private void StartYearUpDown_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (this.EndYear < this.StartYear)
            {
                this.EndYear = this.StartYear;
            }
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