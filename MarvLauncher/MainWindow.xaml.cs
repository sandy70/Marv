using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MarvLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string directory = Directory.GetCurrentDirectory();
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            string root = directoryInfo.Root.ToString();

            StreamWriter streamWriter = new StreamWriter(root + @"MarvApplication\config.xml");
            streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            streamWriter.WriteLine("<settings>");
            streamWriter.WriteLine(XmlString("data.networkdir", "string", root + @"MarvApplication\Data\Networks"));

            streamWriter.WriteLine(XmlString("mongodb.bindir", "string", root + @"MarvApplication\MongoDb"));
            streamWriter.WriteLine(XmlString("mongodb.connection", "string", "mongodb://localhost/?safe=true"));
            streamWriter.WriteLine(XmlString("mongodb.collection", "string", "pipelines"));
            streamWriter.WriteLine(XmlString("mongodb.db", "string", "test"));
            streamWriter.WriteLine(XmlString("mongodb.dbpath", "string", root + @"MarvApplication\MongoDb\db"));

            streamWriter.WriteLine(XmlString("psql.bin", "string", root + @"MarvApplication\Psql\9.0\bin\psql.exe"));
            streamWriter.WriteLine(XmlString("psql.database", "string", "postgis"));
            streamWriter.WriteLine(XmlString("psql.db", "string", "postgis"));
            streamWriter.WriteLine(XmlString("psql.dbpath", "string", root + @"MarvApplication\Psql\9.0\data"));
            streamWriter.WriteLine(XmlString("psql.dexec", "string", root + @"MarvApplication\Psql\9.0\bin\pg_ctl.exe"));
            streamWriter.WriteLine(XmlString("psql.password", "string", "ncc1701d"));
            streamWriter.WriteLine(XmlString("psql.port", "string", "5432"));
            streamWriter.WriteLine(XmlString("psql.server", "string", "127.0.0.1"));
            streamWriter.WriteLine(XmlString("psql.userid", "string", "postgres"));
            streamWriter.WriteLine(XmlString("psql.user", "string", "postgres"));

            streamWriter.WriteLine(XmlString("marv.autoload", "bool", "True"));
            streamWriter.WriteLine(XmlString("marv.demopipeline", "string", root + @"MarvApplication\Data\Demo.pln"));
            streamWriter.WriteLine("</settings>");
            streamWriter.Close();

            Process marvProcess = new Process();
            marvProcess.StartInfo.FileName = root + @"MarvApplication\Marv\Marv.exe";
            marvProcess.StartInfo.WorkingDirectory = root;
            marvProcess.Start();
            marvProcess.WaitForExit();

            this.Close();
        }

        private string XmlString(string key, string type, string value)
        {
            return String.Format("\t<item name=\"{0}\" type=\"{1}\" default=\"\">{2}</item>", key, type, value);
        }
    }
}