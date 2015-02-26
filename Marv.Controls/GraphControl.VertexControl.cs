using System.Windows.Input;
using Marv.Common;

namespace Marv.Controls
{
    public partial class GraphControl
    {
        private void VertexControl_CommandExecuted(object sender, Command<VertexControl> command)
        {
            this.Graph.SelectedVertex = (sender as VertexControl).Vertex;

            if (command == VertexControlCommands.Expand)
            {
                this.UpdateLayout();
            }
            else if (command == VertexControlCommands.SubGraph)
            {
                this.SelectedGroup = (sender as VertexControl).Vertex.HeaderOfGroup;
            }
        }

        private void VertexControl_EvidenceEntered(object sender, NodeEvidence e)
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
    }
}