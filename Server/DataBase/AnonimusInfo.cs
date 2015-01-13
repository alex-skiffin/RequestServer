using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class AnonimusInfo
    {
        [BsonId]
        public Guid Id = Guid.Empty;

        public Guid PhoneId = Guid.Empty;

        public string ContactName = "";

        public Dictionary<string, string> VerySecretInfo = new Dictionary<string, string>();

        /*/// <summary>
        /// Поля класса, которые есть в монге, но у класса как такового их нет
        /// </summary>
        public BsonDocument ExtraElements;*/
    }

    public class AllInfo
    {
        public List<AnonimusInfo> AllInfos = new List<AnonimusInfo>();
    }
}
