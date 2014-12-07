using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class AnonimusInfo
    {
        [BsonId]
        public Guid Id = Guid.Empty;

        public DateTime ChangeTime = DateTime.UtcNow;

        public string VerySecretInfo = "O_o";
    }
}
