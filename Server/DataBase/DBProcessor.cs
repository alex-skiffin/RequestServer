using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Server.DataBase
{
    public class DBProcessor
    {
        private static MongoCollection<AnonimusInfo> _anonimusCollection;

        internal static MongoDatabase Database;
        private MongoServer _server;
        public DBProcessor()
        {
            var accConn = new MongoDbAccessorConnection(MongoDbSettings.MacroscopDb, login, password);
            MongoClientSettings settings = accConn.GetSettings();
            settings.Servers = new[] { new MongoServerAddress("127.0.0.1") };
            settings.ConnectTimeout = TimeSpan.FromSeconds(24);

            var mongoClient = new MongoClient(settings);
            if (mongoClient == null)
                throw new MongoAuthenticationException("Не удалось подключить MongoClient к серверу");

            _server = mongoClient.GetServer();
            Database = _server.GetDatabase("anon");
            Database.CollectionExists(MongoDbSettings.ClustersCollection);
            _anonimusCollection = Database.GetCollection<AnonimusInfo>(MongoDbSettings.ClustersCollection);
        }

        public AnonimusInfo GetInfo()
        {
			return _anonimusCollection.FindAll().First();
        }
        public AnonimusInfo GetInfo(Guid id)
        {
            return _anonimusCollection.FindOneById(id);
        }

        public void AddInfo(Guid infoId, string info)
        {
        }
        public void AddInfo(string info)
        {
        }
    }
}
