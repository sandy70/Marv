using RamGecTools;
using System.Windows.Forms;

namespace LibPipeline
{
    public static class ConfigManager
    {
        private static string configFile = null;
        private static Settings4Net settings4Net = new Settings4Net();

        public static object Read(string key)
        {
            if (ConfigManager.settings4Net.IsLoaded)
            {
                return settings4Net.Settings[key];
            }
            else
            {
                return null;
            }
        }

        public static void SetConfigFile(string fileName)
        {
            ConfigManager.settings4Net.Open(fileName);
            ConfigManager.configFile = fileName;
        }

        public static void Write(string key, object value)
        {
            if (ConfigManager.settings4Net.IsLoaded)
            {
                ConfigManager.settings4Net.Settings[key] = value;
                ConfigManager.settings4Net.Save(ConfigManager.configFile);
            }
        }

        private static void ShowConfigFileDialog()
        {
            MessageBox.Show("Please specify a config file!", "Select Config File", MessageBoxButtons.OK);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML Config Files|*.xml"; // Filter files by extension
            dlg.Multiselect = false;

            DialogResult inputResult = dlg.ShowDialog();

            if (inputResult == DialogResult.OK)
            {
                ConfigManager.SetConfigFile(dlg.FileName);
            }
        }
    }
}