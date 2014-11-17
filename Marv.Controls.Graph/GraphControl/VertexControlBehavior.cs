using System.Windows.Input;
using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class VertexControlBehavior : Behavior<VertexControl>
    {
        public GraphControl GraphControl;
        public VertexControl VertexControl;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.VertexControl = this.AssociatedObject;
            this.GraphControl = this.VertexControl.GetParent<GraphControl>();

            this.VertexControl.CommandExecuted -= VertexControl_CommandExecuted;
            this.VertexControl.CommandExecuted += VertexControl_CommandExecuted;

            this.VertexControl.EvidenceEntered -= VertexControl_EvidenceEntered;
            this.VertexControl.EvidenceEntered += VertexControl_EvidenceEntered;

            this.VertexControl.MouseEnter -= VertexControl_MouseEnter;
            this.VertexControl.MouseEnter += VertexControl_MouseEnter;

            this.VertexControl.MouseLeave -= VertexControl_MouseLeave;
            this.VertexControl.MouseLeave += VertexControl_MouseLeave;
        }

        private void VertexControl_CommandExecuted(object sender, Command<VertexControl> command)
        {
            this.GraphControl.Graph.SelectedVertex = this.VertexControl.Vertex;

            if (command == VertexControlCommands.Expand)
            {
                this.GraphControl.UpdateLayout();
            }
            else if (command == VertexControlCommands.SubGraph)
            {
                this.GraphControl.SelectedGroup = (sender as VertexControl).Vertex.HeaderOfGroup;
            }
        }

        private void VertexControl_EvidenceEntered(object sender, VertexEvidence e)
        {
            this.GraphControl.RaiseEvidenceEntered(e);
        }

        private void VertexControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.VertexControl.IsToolbarVisible = true;
        }

        private void VertexControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.VertexControl.IsToolbarVisible = false;
        }
    }
}