using Marv.Common;
using Marv.Common.Graph;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Vertex = new Vertex
            {
                Key = "Vertex",
                Name = "My Vertex",
                IsExpanded = true,
                Units = "My Units",
                Description = "This is some random long description for this vertex. This will contain long sentences even running in paragraphs.",

                States = new ModelCollection<State>
                {
                    new State
                    {
                        Key = "State1",
                        Name = "State One"
                    },

                    new State
                    {
                        Key = "State2;lkajd;lkjfa;lkd",
                        Name = "State Two"
                    },

                    new State
                    {
                        Key = "State3",
                        Name = "State Three"
                    },
                }
            };

            this.Vertex.UpdateMostProbableState();
        }

        public Vertex Vertex
        {
            get
            {
                return (Vertex) GetValue(VertexProperty);
            }
            set
            {
                SetValue(VertexProperty, value);
            }
        }

        internal void SelectState(State state)
        {
            this.Vertex.SelectState(state);
        }
    }
}