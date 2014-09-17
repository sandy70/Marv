using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AddinExpress.MSO;
using AddinExpress.XL;
using Marv;
using Microsoft.Office.Interop.Excel;
using Application = System.Windows.Forms.Application;
using System.Linq;

namespace Marv_Excel
{
    /// <summary>
    ///   Add-in Express Add-in Module
    /// </summary>
    [Guid("5F2F709C-668C-4327-A6C3-15ECA9CCCB49"), ProgId("Marv_Excel.AddinModule")]
    public class AddinModule : ADXAddinModule
    {
        private ADXExcelTaskPanesCollectionItem ExcelTaskPanesCollectionItem;
        private ADXExcelTaskPanesManager ExcelTaskPanesManager;
        private ADXRibbonGroup FileRibbonGrouop;
        private ImageList IconList;
        private ADXRibbonTab MarvRibbonTab;
        private ADXRibbonButton OpenButton;
        private ADXRibbonButton RunButton;

        #region Component Designer generated code

        /// <summary>
        /// Required by designer
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Required by designer support - do not modify
        /// the following method
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof (AddinModule));
            this.MarvRibbonTab = new AddinExpress.MSO.ADXRibbonTab(this.components);
            this.FileRibbonGrouop = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.OpenButton = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.IconList = new System.Windows.Forms.ImageList(this.components);
            this.ExcelTaskPanesManager = new AddinExpress.XL.ADXExcelTaskPanesManager(this.components);
            this.ExcelTaskPanesCollectionItem = new AddinExpress.XL.ADXExcelTaskPanesCollectionItem(this.components);
            this.RunButton = new AddinExpress.MSO.ADXRibbonButton(this.components);
            // 
            // MarvRibbonTab
            // 
            this.MarvRibbonTab.Caption = "MARV";
            this.MarvRibbonTab.Controls.Add(this.FileRibbonGrouop);
            this.MarvRibbonTab.Id = "adxRibbonTab_08f4a768cf7f44b39d16d8f5028106d2";
            this.MarvRibbonTab.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // FileRibbonGrouop
            // 
            this.FileRibbonGrouop.Caption = "File";
            this.FileRibbonGrouop.Controls.Add(this.OpenButton);
            this.FileRibbonGrouop.Controls.Add(this.RunButton);
            this.FileRibbonGrouop.Id = "adxRibbonGroup_85abbbe6b4e94c7baaf9b2fcef6db489";
            this.FileRibbonGrouop.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.FileRibbonGrouop.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // OpenButton
            // 
            this.OpenButton.Caption = "Open";
            this.OpenButton.Id = "adxRibbonButton_c2516ba8e35e485688dd882bdfeeae84";
            this.OpenButton.Image = 0;
            this.OpenButton.ImageList = this.IconList;
            this.OpenButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.OpenButton.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.OpenButton.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.OpenButton.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.OpenButton_Click);
            // 
            // IconList
            // 
            this.IconList.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("IconList.ImageStream")));
            this.IconList.TransparentColor = System.Drawing.Color.Transparent;
            this.IconList.Images.SetKeyName(0, "Open.png");
            this.IconList.Images.SetKeyName(1, "Run.png");
            // 
            // ExcelTaskPanesManager
            // 
            this.ExcelTaskPanesManager.Items.Add(this.ExcelTaskPanesCollectionItem);
            this.ExcelTaskPanesManager.SetOwner(this);
            // 
            // ExcelTaskPanesCollectionItem
            // 
            this.ExcelTaskPanesCollectionItem.AlwaysShowHeader = true;
            this.ExcelTaskPanesCollectionItem.CloseButton = true;
            this.ExcelTaskPanesCollectionItem.Position = AddinExpress.XL.ADXExcelTaskPanePosition.Right;
            this.ExcelTaskPanesCollectionItem.TaskPaneClassName = "Marv_Excel.TaskPane";
            this.ExcelTaskPanesCollectionItem.UseOfficeThemeForBackground = true;
            // 
            // RunButton
            // 
            this.RunButton.Caption = "Run";
            this.RunButton.Id = "adxRibbonButton_174592a089b64ebb85750adf9908bb3f";
            this.RunButton.Image = 1;
            this.RunButton.ImageList = this.IconList;
            this.RunButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.RunButton.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.RunButton.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.RunButton.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.RunButton_Click);
            // 
            // AddinModule
            // 
            this.AddinName = "Marv_Excel";
            this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaExcel;
        }

        #endregion

        #region Add-in Express automatic code

        // Required by Add-in Express - do not modify
        // the methods within this region

        [ComRegisterFunction]
        public static void AddinRegister(Type t)
        {
            ADXRegister(t);
        }

        [ComUnregisterFunction]
        public static void AddinUnregister(Type t)
        {
            ADXUnregister(t);
        }

        public override IContainer GetContainer()
        {
            if (components == null)
                components = new Container();
            return components;
        }

        #endregion

        public AddinModule()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            // Please add any initialization code to the AddinInitialize event handler
        }

        public new static AddinModule CurrentInstance
        {
            get
            {
                return ADXAddinModule.CurrentInstance as AddinModule;
            }
        }

        public _Application ExcelApp
        {
            get
            {
                return (HostApplication as _Application);
            }
        }

        public string FileName { get; set; }
        public Graph Graph { get; set; }

        public Worksheet InputSheet
        {
            get
            {
                return this.Workbook.GetWorksheetOrNew("Input");
            }
        }

        public Worksheet OutputSheet
        {
            get
            {
                return this.Workbook.GetWorksheetOrNew("Output");
            }
        }

        public TaskPane TaskPane
        {
            get
            {
                return this.ExcelTaskPanesCollectionItem.TaskPaneInstance as TaskPane;
            }
        }

        public Workbook Workbook
        {
            get
            {
                if (this.ExcelApp.ActiveWorkbook == null)
                {
                    this.ExcelApp.Workbooks.Add();
                }

                return this.ExcelApp.ActiveWorkbook;
            }
        }

        private void OpenButton_Click(object sender, IRibbonControl control, bool pressed)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "NetworkFile (*.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.TaskPane.Show();

                this.FileName = dialog.FileName;

                this.Graph = Graph.Read(this.FileName);

                if (this.TaskPane != null)
                {
                    this.TaskPane.Vertices = this.Graph.Vertices.Where(vertex => !vertex.Groups.Contains("hidden"));
                    this.TaskPane.DoneButtonClicked += taskPane_DoneButtonClicked;
                }
            }
        }

        private void RunButton_Click(object sender, IRibbonControl control, bool pressed)
        {
            this.OutputSheet.Cells.Clear();

            var sheetModel = SheetModel.Read(this.InputSheet);
            sheetModel.Run();
            sheetModel.Write(this.OutputSheet);

            this.OutputSheet.Activate();
        }

        private void taskPane_DoneButtonClicked(object sender, EventArgs e)
        {
            if (!this.TaskPane.SelectedVertices.Any())
            {
                MessageBox.Show("Select at least 1 node.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            this.TaskPane.Hide();

            this.InputSheet.Cells.Clear();
            this.InputSheet.Cells.NumberFormat = "@";

            var sheetModel = new SheetModel
            {
                SheetHeaders = new Dictionary<string, object>
                {
                    {"Network File", this.FileName},
                    {"Start Year", this.TaskPane.StartYear},
                    {"End Year", this.TaskPane.EndYear}
                },
                ColumnHeaders = new List<string>
                {
                    "Section Key",
                    "Latitude",
                    "Longitude"
                },

                StartYear = this.TaskPane.StartYear,
                EndYear = this.TaskPane.EndYear,
                Vertices = this.TaskPane.SelectedVertices
            };

            sheetModel.LineValue["One"] = new Dict<int, string, string, double>();
            sheetModel.Write(this.InputSheet);
        }
    }
}