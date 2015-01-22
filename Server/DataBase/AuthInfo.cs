using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class AuthInfo
    {
        [BsonId]
        public Guid Id = new Guid();

        public string UserName;

        public string PassHash;

        public Guid ProfileId;
    }
}
