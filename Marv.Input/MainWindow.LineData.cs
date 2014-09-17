using System.Windows;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (MainWindow), new PropertyMetadata(null, ChangedSelectedCell));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (MainWindow), new PropertyMetadata(int.MinValue));

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

        private static void ChangedSelectedCell(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MainWindow;

            control.Graph.NetworkStructure.Run(control.LineData.Sections[control.SelectedSectionId]);
            control.LineDataControl.UpdateCurrentGraphData(control.SelectedSectionId, control.SelectedYear);
        }
    }
}