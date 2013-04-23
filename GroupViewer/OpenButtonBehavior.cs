using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace GroupViewer
{
    internal class OpenButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".net",
                Filter = "Hugin Network Files (.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                Properties.Settings.Default.FileName = dialog.FileName;
            }
        }
    }
}