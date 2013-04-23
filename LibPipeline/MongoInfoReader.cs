using LibConfig;

namespace LibPipeline
{
    public static class MongoInfoReader
    {
        public static MongoInfo Read()
        {
            MongoInfo mongoInfo = new MongoInfo();
            mongoInfo.BinDir = (string)ConfigManager.Read("mongodb.bindir");
            mongoInfo.Collection = (string)ConfigManager.Read("mongodb.collection");
            mongoInfo.Connection = (string)ConfigManager.Read("mongodb.connection");
            mongoInfo.Database = (string)ConfigManager.Read("mongodb.db");
            mongoInfo.DbPath = (string)ConfigManager.Read("mongodb.dbpath");
            return mongoInfo;
        }
    }
}