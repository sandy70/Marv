using LibConfig;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LibPipeline
{
    public class MongoInstance
    {
        private MongoDatabase Database;
        private MongoInfo Info;
        private MongoServer Server;

        public MongoInstance(MongoInfo info)
        {
            this.Info = info;
            this.Start();
            this.Server = MongoServer.Create(this.Info.Connection);
            this.Database = this.Server.GetDatabase(this.Info.Database);
        }

        public MongoCollection<T> GetCollection<T>()
        {
            return this.Database.GetCollection<T>(this.Info.Collection);
        }

        public void ShutDown()
        {
            this.Server.Shutdown();
        }

        private void Start()
        {
            // If mongod.lock exists from previous unclean shutdown, delete it
            string lockFile = Path.Combine(this.Info.DbPath, "mongod.lock");

            if (File.Exists(lockFile))
            {
                try
                {
                    File.Delete(lockFile);
                }
                catch (IOException e)
                {
                    Console.WriteLine("lock file could not be deleted: " + e.Message);
                }
            }

            Process[] processList = Process.GetProcessesByName("mongod");

            if (processList.Count() == 0)
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = this.Info.DExec;
                process.StartInfo.Arguments = @"--nojournal --dbpath " + this.Info.DbPath;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }

            while (processList.Count() == 0)
            {
                processList = Process.GetProcessesByName("mongod");
            }
        }
    }
}