using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Marv;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (MainWindow), new PropertyMetadata(int.MinValue));

        private readonly List<GridViewCellClipboardEventArgs> cellClipboardEventArgs = new List<GridViewCellClipboardEventArgs>();
        private readonly Dictionary<GridViewCellClipboardEventArgs, object> oldValues = new Dictionary<GridViewCellClipboardEventArgs, object>();

        public string SelectedSectionId
        {
            get
            {
                return (string) GetValue(SelectedSectionIdProperty);
            }
            set
            {
                SetValue(SelectedSectionIdProperty, value);
            }
        }

        public int SelectedYear
        {
            get
            {
                return (int) GetValue(SelectedYearProperty);
            }
            set
            {
                SetValue(SelectedYearProperty, value);
            }
        }

        private void InputGridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var cellClipboardEventArg in this.cellClipboardEventArgs)
            {
                this.SetCell(cellClipboardEventArg.Cell.ToModel(), cellClipboardEventArg.Value as string, oldValues[cellClipboardEventArg] as string);
            }

            cellClipboardEventArgs.Clear();
        }

        private void InputGridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.cellClipboardEventArgs.Add(e);
            this.oldValues[e] = e.Cell.ToModel().Data;
        }

        public void SetCell(CellModel cellModel, string newStr, string oldStr = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                if (!string.IsNullOrWhiteSpace(newStr))
                {
                    if (!string.IsNullOrWhiteSpace(oldStr))
                    {
                        this.LineData.Sections.ChangeKey(oldStr, newStr);
                    }
                    else
                    {
                        this.LineData.Sections[newStr] = new Dict<int, string, VertexData>();
                    }
                }

                cellModel.Data = newStr;
                return;
            }

            var selectedVertex = this.Graph.SelectedVertex;

            if (selectedVertex == null)
            {
                return;
            }

            var values = selectedVertex.States.Parse(newStr);

            var evidence = values == null ? null : new VertexData
            {
                Evidence = values.ToArray(), String = newStr
            };

            cellModel.Data = evidence;

            this.LineData.Sections[cellModel.SectionId][cellModel.Year][selectedVertex.Key] = evidence;

            // this.Graph.Run(LineData.SectionEvidences[cellModel.SectionId]);
        }
    }
}