﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AddinExpress.MSO;
using AddinExpress.XL;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;
using Application = System.Windows.Forms.Application;

namespace Marv_Excel
{
    [Guid("5522A6E6-9554-4599-8EFF-54594B5329A8"), ProgId("Marv_Excel.AddinModule")]
    public class AddinModule : ADXAddinModule
    {
        private ADXExcelTaskPanesCollectionItem ExcelTaskPanesCollectionItem;
        private ADXExcelTaskPanesManager ExcelTaskPanesManager;
        private ADXRibbonGroup FileRibbonGroup;
        private ImageList IconList;
        private ADXRibbonTab MarvRibbonTab;
        private ADXRibbonButton OpenButton;
        private ADXTaskPane TaskPane;

        private string fileName;

        public AddinModule()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
        }

        public new static AddinModule CurrentInstance
        {
            get
            {
                return ADXAddinModule.CurrentInstance as AddinModule;
            }
        }

        public static _Application ExcelApp
        {
            get
            {
                return (CurrentInstance.HostApplication as _Application);
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

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
            this.FileRibbonGroup = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.OpenButton = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.IconList = new System.Windows.Forms.ImageList(this.components);
            this.TaskPane = new AddinExpress.MSO.ADXTaskPane(this.components);
            this.ExcelTaskPanesManager = new AddinExpress.XL.ADXExcelTaskPanesManager(this.components);
            this.ExcelTaskPanesCollectionItem = new AddinExpress.XL.ADXExcelTaskPanesCollectionItem(this.components);
            // 
            // MarvRibbonTab
            // 
            this.MarvRibbonTab.Caption = "MARV";
            this.MarvRibbonTab.Controls.Add(this.FileRibbonGroup);
            this.MarvRibbonTab.Id = "adxRibbonTab_0f94d5995d624255b03a659f43dbbfb2";
            this.MarvRibbonTab.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // FileRibbonGroup
            // 
            this.FileRibbonGroup.Caption = "File";
            this.FileRibbonGroup.Controls.Add(this.OpenButton);
            this.FileRibbonGroup.Id = "adxRibbonGroup_ad04fb6e0687428cb72fffa876b3e9e5";
            this.FileRibbonGroup.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.FileRibbonGroup.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // OpenButton
            // 
            this.OpenButton.Caption = "Open";
            this.OpenButton.Id = "adxRibbonButton_91b23395c4af4f13b553c8a51d86c6c8";
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
            // TaskPane
            // 
            this.TaskPane.ControlProgID = "";
            this.TaskPane.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaExcel;
            this.TaskPane.Visible = false;
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
            // AddinModule
            // 
            this.AddinName = "Marv_Excel";
            this.LoadBehavior = ((AddinExpress.MSO.ADXLoadBehavior) (((AddinExpress.MSO.ADXLoadBehavior.lbConnected | AddinExpress.MSO.ADXLoadBehavior.lbLoadAtStartup)
                                                                      | AddinExpress.MSO.ADXLoadBehavior.lbLoadOnDemand)));
            this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaExcel;
            this.TaskPanes.Add(this.TaskPane);
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

        public override void UninstallControls()
        {
            base.UninstallControls();
        }

        #endregion

        private void OpenButton_Click(object sender, IRibbonControl control, bool pressed)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "NetworkFile (*.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.ExcelTaskPanesCollectionItem.TaskPaneInstance.Show();

                this.FileName = dialog.FileName;

                var graph = Graph.Read(this.FileName);

                var taskPane = this.ExcelTaskPanesCollectionItem.TaskPaneInstance as TaskPane;
                taskPane.SetVertices(graph.Vertices);
            }
        }
    }
}