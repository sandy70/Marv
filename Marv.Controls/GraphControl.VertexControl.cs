using System;
using System.Windows.Input;
using Marv.Common;

namespace Marv.Controls
{
    public partial class GraphControl
    {
        private void VertexControl_EvidenceEntered(object sender, VertexEvidence e)
        {
            this.Run();
            this.RaiseEvidenceEntered(e);
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
        }
    }
}