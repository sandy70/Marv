﻿namespace Marv.ExcelNew
{
    partial class WpfHost
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ElementHost = new System.Windows.Forms.Integration.ElementHost();
            this.vertexListBox1 = new Marv.ExcelNew.VertexListBox();
            this.SuspendLayout();
            // 
            // ElementHost
            // 
            this.ElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ElementHost.Location = new System.Drawing.Point(0, 0);
            this.ElementHost.Name = "ElementHost";
            this.ElementHost.Size = new System.Drawing.Size(512, 256);
            this.ElementHost.TabIndex = 0;
            this.ElementHost.Child = this.vertexListBox1;
            // 
            // WpfHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ElementHost);
            this.Name = "WpfHost";
            this.Size = new System.Drawing.Size(512, 256);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Integration.ElementHost ElementHost;
        public VertexListBox vertexListBox1;

    }
}
