using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AddinExpress.XL;
using Marv.Common.Graph;

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
                return this.vertexSelectionControl.StartYear;
            }
        }

        public int EndYear
        {
            get
            {
                return this.vertexSelectionControl.EndYear;
            }
        }

        public IEnumerable<Vertex> SelectedVertices
        {
            get
            {
                return this.vertexSelectionControl.SelectedVertices.ToList();
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            set
            {
                this.vertexSelectionControl.Vertices = value;
            }
        }

        public TaskPane()
        {
            InitializeComponent();
            this.Activated += TaskPane_Activated;

            this.vertexSelectionControl.DoneButtonClicked += vertexSelectionControl_DoneButtonClicked;
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
            this.elementHost.Height = this.Height - 20;
            this.elementHost.Width = this.Width - 20;
        }

        private void vertexSelectionControl_DoneButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Hide();

            if (this.DoneButtonClicked != null)
            {
                this.DoneButtonClicked(this, new EventArgs());
            }
        }
    }
}