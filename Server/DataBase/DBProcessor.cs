using System;
using System.IO;
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
        private static MongoCollection<Profile> _profileCollection;
        private static MongoCollection<AuthInfo> _authCollection;
        private string _login = string.Empty;
        private string _password = string.Empty;

        internal static MongoDatabase Database;

        public DbProcessor()
        {
            ReadCredentionals();
            try
            {
                var settings = new MongoClientSettings
                {
                    Servers = new[] { new MongoServerAddress("127.0.0.1") },
                    Credentials = new[] { new MongoCredential(new MongoInternalIdentity("anon", _login), new PasswordEvidence(_password)) }
                };

                var mongoClient = new MongoClient(settings);

                var server = mongoClient.GetServer();
                Database = server.GetDatabase("anon");
                Database.CollectionExists("info");
                Database.CollectionExists("phone");
                Database.CollectionExists("profile");
                _anonimusCollection = Database.GetCollection<AnonimusInfo>("info");
                _profileCollection = Database.GetCollection<Profile>("profile");
                _phoneCollection = Database.GetCollection<Phone>("phone");
                _authCollection = Database.GetCollection<AuthInfo>("auth");
            }
            catch
            {
                Console.WriteLine("\r\nБаза данных недоступна!");
                throw;
            }
        }

        private void ReadCredentionals()
        {
            try
            {
                var read = File.ReadAllText("creds.cfg");
                var creds = read.Split(':');
                if (creds.Length == 0)
                    throw new Exception("Логин-пароль отсутствуют");
                _login = creds[0];
                _password = creds[1];

            }
            catch (FileNotFoundException)
            {
                throw new Exception("Нет файла с логином-паролем");
            }
            catch (Exception)
            {
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

        public Profile GetProfile(string profileId)
        {
            var id = Guid.Parse(profileId);
            return _profileCollection.FindOneById(id);
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

        public void AddProfile(string requestBody)
        {
            var inff = JsonConvert.DeserializeObject<Profile>(requestBody);
            _phoneCollection.Save(inff);
        }

        public static void CheckAuth(string userName, string passHash)
        {
            var query = Query.EQ("UserName", userName);
            if (_authCollection.FindOne(query).PassHash != passHash)
                throw new Exception("Access denied!!!");
        }

        public bool NameFree(string userName)
        {
            var query = Query.EQ("UserName", userName);
            return _authCollection.FindOne(query) != null;
        }

        public void AddRegisterInfo(string authInfo)
        {
            var auth = JsonConvert.DeserializeObject<AuthInfo>(authInfo);
            _authCollection.Save(auth);
        }
    }
}