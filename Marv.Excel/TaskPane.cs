using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AddinExpress.XL;
using Marv;

namespace Marv_Excel
{
    public partial class TaskPane : ADXExcelTaskPane
    {
        public event EventHandler DoneButtonClicked;

        private bool isHiddenBefore;

        public int StartYear
        {
            get
            {
                return this.VertexSelectionControl.StartYear;
            }
        }

        public int EndYear
        {
            get
            {
                return this.VertexSelectionControl.EndYear;
            }
        }

        public IEnumerable<Vertex> SelectedVertices
        {
            get
            {
                return this.VertexSelectionControl.SelectedVertices.ToList();
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            set
            {
                this.VertexSelectionControl.Vertices = value;
            }
        }

        public TaskPane()
        {
            InitializeComponent();
            this.Activated += TaskPane_Activated;

            this.VertexSelectionControl.DoneButtonClicked += vertexSelectionControl_DoneButtonClicked;
        }

        private void TaskPane_Activated(object sender, EventArgs e)
        {
            if (!isHiddenBefore)
            {
                this.Hide();
                isHiddenBefore = true;
            }
        }

        private void TaskPane_SizeChanged(object sender, EventArgs e)
        {
            this.ElementHost.Height = this.Height - 20;
            this.ElementHost.Width = this.Width - 20;
        }

        private void vertexSelectionControl_DoneButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.DoneButtonClicked != null)
            {
                this.DoneButtonClicked(this, new EventArgs());
            }
        }
    }
}