namespace Marv.ExcelNew
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.MarvTab = this.Factory.CreateRibbonTab();
            this.FileGroup = this.Factory.CreateRibbonGroup();
            this.OpenButton = this.Factory.CreateRibbonButton();
            this.ToggleMarvPaneButton = this.Factory.CreateRibbonToggleButton();
            this.MarvTab.SuspendLayout();
            this.FileGroup.SuspendLayout();
            // 
            // MarvTab
            // 
            this.MarvTab.Groups.Add(this.FileGroup);
            this.MarvTab.Label = "MARV";
            this.MarvTab.Name = "MarvTab";
            // 
            // FileGroup
            // 
            this.FileGroup.Items.Add(this.OpenButton);
            this.FileGroup.Items.Add(this.ToggleMarvPaneButton);
            this.FileGroup.Label = "File";
            this.FileGroup.Name = "FileGroup";
            // 
            // OpenButton
            // 
            this.OpenButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.OpenButton.Image = global::Marv.ExcelNew.Properties.Resources.Open;
            this.OpenButton.Label = "Open Network";
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.ShowImage = true;
            this.OpenButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.OpenButton_Click);
            // 
            // ToggleMarvPaneButton
            // 
            this.ToggleMarvPaneButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.ToggleMarvPaneButton.Image = global::Marv.ExcelNew.Properties.Resources.SubGraph;
            this.ToggleMarvPaneButton.Label = "MARV Pane";
            this.ToggleMarvPaneButton.Name = "ToggleMarvPaneButton";
            this.ToggleMarvPaneButton.ShowImage = true;
            this.ToggleMarvPaneButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ToggleMarvPaneButton_Click);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.MarvTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.MarvTab.ResumeLayout(false);
            this.MarvTab.PerformLayout();
            this.FileGroup.ResumeLayout(false);
            this.FileGroup.PerformLayout();

        }

        #endregion

        private Microsoft.Office.Tools.Ribbon.RibbonTab MarvTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup FileGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton OpenButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton ToggleMarvPaneButton;

    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
