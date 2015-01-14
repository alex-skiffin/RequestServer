using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class Contact
    {
        [BsonId]
        public Guid Id = new Guid();

        public string Prefix;
        public string Name;
        public string Patronymic;
        public string LastName;

        public string HomePhone;
        public string MobilePhone;
        public string WorkPhone;
        
        public string HomeEmail;
        public string MobileEmail;
        public string WorkEmail;

        public string WorkPlace;

        public DateTime Birthday;

        public Guid PhotoId;
    }
}
