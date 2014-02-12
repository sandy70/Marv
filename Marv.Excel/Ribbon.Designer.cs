namespace Marv.Excel
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
            this.OpenFileButton = this.Factory.CreateRibbonButton();
            this.MarvTab.SuspendLayout();
            this.FileGroup.SuspendLayout();
            // 
            // MarvTab
            // 
            this.MarvTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.MarvTab.Groups.Add(this.FileGroup);
            this.MarvTab.Label = "MARV";
            this.MarvTab.Name = "MarvTab";
            // 
            // FileGroup
            // 
            this.FileGroup.Items.Add(this.OpenFileButton);
            this.FileGroup.Label = "File";
            this.FileGroup.Name = "FileGroup";
            // 
            // OpenFileButton
            // 
            this.OpenFileButton.Label = "Open File";
            this.OpenFileButton.Name = "OpenFileButton";
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

        internal Microsoft.Office.Tools.Ribbon.RibbonTab MarvTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup FileGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton OpenFileButton;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
