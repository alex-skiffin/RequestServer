using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;

namespace Server.DataBase
{
    public class DbProcessor
    {
        private static MongoCollection<AnonimusInfo> _anonimusCollection;
        private static MongoCollection<Phone> _phoneCollection;

        internal static MongoDatabase Database;
        
        public DbProcessor()
        {
            var settings = new MongoClientSettings
            {
                Servers = new[] {new MongoServerAddress("127.0.0.1")}
            };

            var mongoClient = new MongoClient(settings);
            if (mongoClient == null)
                throw new MongoAuthenticationException("Не удалось подключить MongoClient к серверу");

            var server = mongoClient.GetServer();
            Database = server.GetDatabase("anon");
            Database.CollectionExists("info");
            Database.CollectionExists("phone");
            _anonimusCollection = Database.GetCollection<AnonimusInfo>("info");
            _phoneCollection = Database.GetCollection<Phone>("phone");
        }

#region get
        public AnonimusInfo GetInfo()
        {
            return _anonimusCollection.FindOne();
        }
        public AnonimusInfo GetInfo(Guid id)
        {
            return _anonimusCollection.FindOneById(id);
        }
        public Phone GetPhone()
        {
            return _phoneCollection.FindOne();
        }
        public Phone GetPhone(Guid id)
        {
            return _phoneCollection.FindOneById(id);
        }
        public Phone GetPhone(string phoneName)
        {
            var query = Query.EQ("PhoneName", phoneName);
            return _phoneCollection.FindOne(query);
        }

        public AllInfo GetAllInfo()
        {
            return new AllInfo { AllInfos = _anonimusCollection.FindAll().ToList() };
        }

        public AllPhone GetAllPhone()
        {
            return new AllPhone { AllPhones= _phoneCollection.FindAll().ToList() };
        }
#endregion

        public void AddInfo(Guid infoId, string info)
        {
            _anonimusCollection.Save(info);
        }
        public void AddContactsInfo(string info)
        {
            var inff = JsonConvert.DeserializeObject<AnonimusInfo>(info.Replace("[", "").Replace("]", ""));
            _anonimusCollection.Save(inff);
        }
        public void AddPhoneInfo(string info)
        {
            var inff = JsonConvert.DeserializeObject<Phone>(info);
            _phoneCollection.Save(inff);
        }
    }
}
