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
            this.ElementHost = new System.Windows.Forms.Integration.ElementHost();
            this.VertexSelectionControl = new Marv_Excel.VertexSelectionControl();
            this.SuspendLayout();
            // 
            // ElementHost
            // 
            this.ElementHost.Location = new System.Drawing.Point(0, 0);
            this.ElementHost.Margin = new System.Windows.Forms.Padding(10);
            this.ElementHost.Name = "ElementHost";
            this.ElementHost.Size = new System.Drawing.Size(345, 626);
            this.ElementHost.TabIndex = 0;
            this.ElementHost.Text = "elementHost1";
            this.ElementHost.Child = this.VertexSelectionControl;
            // 
            // TaskPane
            // 
            this.ClientSize = new System.Drawing.Size(369, 650);
            this.Controls.Add(this.ElementHost);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "TaskPane";
            this.Text = "TaskPane";
            this.SizeChanged += new System.EventHandler(this.TaskPane_SizeChanged);
            this.ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.Integration.ElementHost ElementHost;
        public VertexSelectionControl VertexSelectionControl;

    }
}
