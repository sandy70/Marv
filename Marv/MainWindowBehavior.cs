using LibNetwork;
using LibPipeline;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows;

namespace Marv
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closing += AssociatedObject_Closing;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.AssociatedObject.SourceGraph.Write(this.AssociatedObject.SourceGraph.FileName);
            Properties.Settings.Default.Save();
        }

        private async void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            var window = this.AssociatedObject;

            if (e.KeyboardDevice.IsKeyDown(Key.R) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {

            }
            else if (e.KeyboardDevice.IsKeyDown(Key.M) &&
               (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
                window.IsMenuVisible = !window.IsMenuVisible;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.O) &&
                   (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)))
            {
            }
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.MultiLocations = new SelectableCollection<MultiLocation>();

            window.MultiLocations = AdcoInput.Read();

            window.SourceGraph = await BnGraph.ReadAsync<BnVertexViewModel>(@"D:\Data\ADCO02\ADCO_06.net");

            window.DisplayGraph = window.SourceGraph.GetSubGraph(window.SourceGraph.DefaultGroup);
            
            window.RetractAllButton.Click += RetractAllButton_Click;
            window.EditNetworkFilesMenuItem.Click += EditNetworkFilesMenuItem_Click;
            window.EditSettingsMenuItem.Click += EditSettingsMenuItem_Click;
            window.RunModelMenuItem.Click += RunModelMenuItem_Click;
        }

        private async void RunModelMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var graph = BnGraph.Read<BnVertexViewModel>(@"D:\Data\ADCO02\ADCO_06.net");
            int startIndex = 0;
            var window = this.AssociatedObject;

            var endYear = window.EndYear;
            var inputFileName = window.InputFileName;

            var multiLocation = window.MultiLocations.SelectedItem;

            await Task.Run(() =>
            {
                var nCompleted = startIndex;
                var nLocations = multiLocation.Count();
                var startYear = (int)multiLocation["StartYear"];

                foreach (var location in multiLocation.Skip(startIndex))
                {
                    var graphEvidence = AdcoInput.GetGraphEvidence(graph, inputFileName, multiLocation.Name, location.Name);

                    var modelValue = graph.Run(graphEvidence, startYear, endYear);

                    Console.WriteLine("Ran " + ++nCompleted + " of " + nLocations);

                    var fileName = Path.Combine(multiLocation.Name, location.Name + ".db");

                    ObjectDataBase.Write(fileName, modelValue);
                }
            });
        }

        private void EditNetworkFilesMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.TransitionControl.SelectElement("EditNetworkFilesControl");
        }

        private void EditSettingsMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.TransitionControl.SelectElement("SettingsControl");
        }

        private void RetractAllButton_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;
            window.SourceGraph.Value = window.SourceGraph.ClearEvidence();
        }
    }
}