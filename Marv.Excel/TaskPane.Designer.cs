namespace Marv_Excel
{
    partial class TaskPane
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
  
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
  
        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this.vertexSelectionControl = new VertexSelectionControl();
            this.SuspendLayout();
            // 
            // elementHost
            // 
            this.elementHost.Location = new System.Drawing.Point(10, 10);
            this.elementHost.Margin = new System.Windows.Forms.Padding(0);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(353, 501);
            this.elementHost.TabIndex = 0;
            this.elementHost.Child = this.vertexSelectionControl;
            // 
            // TaskPane
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(372, 520);
            this.Controls.Add(this.elementHost);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "TaskPane";
            this.Text = "MarvTaskPane";
            this.SizeChanged += new System.EventHandler(this.TaskPane_SizeChanged);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost;
        private VertexSelectionControl vertexSelectionControl;
    }
}
