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
            try
            {
                var settings = new MongoClientSettings
                {
                    Servers = new[] {new MongoServerAddress("127.0.0.1")}
                };

                var mongoClient = new MongoClient(settings);

                var server = mongoClient.GetServer();
                Database = server.GetDatabase("anon");
                Database.CollectionExists("info");
                Database.CollectionExists("phone");
                _anonimusCollection = Database.GetCollection<AnonimusInfo>("info");
                _phoneCollection = Database.GetCollection<Phone>("phone");
            }
            catch
            {
                Console.WriteLine("\r\nБаза данных недоступна!");
                throw;
            }
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
        public void AddContactInfo(string info)
        {
            var inff = JsonConvert.DeserializeObject<AnonimusInfo>(info.Replace("[", "").Replace("]", ""));
            _anonimusCollection.Save(inff);
        }
        public void AddAllContactsInfo(string info)
        {
            var inff = JsonConvert.DeserializeObject<AllInfo>(info);
            foreach (var inf in inff.AllInfos)
            {
                _anonimusCollection.Save(inf);
            }
        }
        public void AddPhoneInfo(string info)
        {
            var inff = JsonConvert.DeserializeObject<Phone>(info);
            _phoneCollection.Save(inff);
        }
    }
}