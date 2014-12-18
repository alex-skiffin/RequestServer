using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class Phone
    {
        [BsonId]
        public Guid Id = Guid.NewGuid();

        public string PhoneName = String.Empty;
    }
    public class AllPhone
    {
        public List<Phone> AllPhones = new List<Phone>();
    }
}
