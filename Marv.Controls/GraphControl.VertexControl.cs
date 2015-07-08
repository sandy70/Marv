using System;
using System.Linq;
using System.Windows.Input;
using Marv.Common;

namespace Marv.Controls
{
    public partial class GraphControl
    {
        private void VertexControl_EvidenceEntered(object sender, VertexEvidence e)
        {
            this.RaiseEvidenceEntered(e);
        }

        private void VertexControl_ExpandButtonClicked(object sender, EventArgs e)
        {
            this.UpdateLayout();
        }

        private void VertexControl_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as VertexControl).IsToolbarVisible = true;
        }

        private void VertexControl_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as VertexControl).IsToolbarVisible = false;
        }

        private void VertexControl_ShowGroupButtonClicked(object sender, EventArgs e)
        {
            this.SelectedGroup = (sender as VertexControl).Vertex.HeaderOfGroup;

            var selectedVertex = this.SelectedVertex ?? this.Graph.Vertices.FirstOrDefault(vertex => vertex.HeaderOfGroup == this.SelectedGroup);

            this.UpdateDisplayGraph(selectedVertex == null ? null : selectedVertex.Key);

            this.SelectedVertex = this.DisplayGraph.GetSink();
        }
    }
}