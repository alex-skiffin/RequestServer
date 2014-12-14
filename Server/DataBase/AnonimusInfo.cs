using System;
using System.Collections.Generic;
using System.Globalization;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.DataBase
{
    public class AnonimusInfo
    {
        [BsonId]
        public Guid Id = Guid.Empty;

        public string ChangeTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

        public string VerySecretInfo = "O_o";

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
