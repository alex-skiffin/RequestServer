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
                Servers = new[] { new MongoServerAddress("127.0.0.1") }
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
        public AnonimusInfo GetInfo(string contactInfo)
        {
            Guid tempGuid;
            if (Guid.TryParse(contactInfo, out tempGuid))
                return _anonimusCollection.FindOneById(tempGuid);
            var query = Query.EQ("ContactName", contactInfo);
            return _anonimusCollection.FindOne(query);
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

        public AllInfo GetAllInfo(string phoneName)
        {
            if (string.IsNullOrEmpty(phoneName))
                return new AllInfo();

            var phone = GetPhone(phoneName);
            if (phone == null)
                return new AllInfo();
            var query = Query.EQ("PhoneId", phone.Id);
            return new AllInfo { AllInfos = _anonimusCollection.Find(query).ToList() };
        }

        public AllPhone GetAllPhone()
        {
            return new AllPhone { AllPhones = _phoneCollection.FindAll().ToList() };
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
