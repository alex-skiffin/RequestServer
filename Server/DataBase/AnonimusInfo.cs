using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class AnonimusInfo
    {
        [BsonId]
        public Guid Id = Guid.Empty;

        public DateTime ChangeTime = DateTime.UtcNow;

        public string VerySecretInfo = "O_o";

        /// <summary>
        /// Поля класса, которые есть в монге, но у класса как такового их нет
        /// </summary>
        public BsonDocument ExtraElements;
    }

    public class AllInfo
    {
        public List<AnonimusInfo> AllInfos = new List<AnonimusInfo>(); 
    }
}
