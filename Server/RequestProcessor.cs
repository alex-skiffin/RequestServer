using System;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Server.DataBase;

namespace Server
{
    class RequestProcessor
    {
        private readonly DbProcessor _dbProcessor = new DbProcessor();

        public string GetResponseData(string httpMethod, string[] urlParts, string requestBody = null)
        {
            string data = string.Empty;
            if (httpMethod.ToUpper() == "GET")
            {
                data = Get(urlParts);
            }
            if (httpMethod.ToUpper() == "POST")
            {
                data = Post(urlParts, requestBody);
            }
            if (httpMethod.ToUpper() == "PUT")
            {
            }
            if (httpMethod.ToUpper() == "DELETE")
            {
            }
            return data;
        }

        public string Get(string[] commandData)
        {
            if (commandData.Length == 0)
                return "Unkown request";
            string info = string.Empty;
            var jsonSerialiser = new JavaScriptSerializer();
            Console.WriteLine("Запрос на получение информации");
            if (commandData[1] == "all_phones")
                info = jsonSerialiser.Serialize(_dbProcessor.GetAllPhone());
            if (commandData[1] == "all_contacts")
                info = jsonSerialiser.Serialize(_dbProcessor.GetAllInfo(commandData[2]));
            if (commandData[1] == "phone")
                info = jsonSerialiser.Serialize(_dbProcessor.GetPhone(commandData[2]));
            if (commandData[1] == "contact")
                info = JsonConvert.SerializeObject(_dbProcessor.GetInfo(commandData[2]));
            if (commandData[1] == "profile")
                info = JsonConvert.SerializeObject(_dbProcessor.GetProfile(commandData[2]));
            if (commandData[1] == "")
                info = JsonConvert.SerializeObject(_dbProcessor.GetPhone());
            return info;
        }

        public string Post(string[] commandData, string requestBody)
        {
            if (commandData[1] == "phone")
                _dbProcessor.AddPhoneInfo(requestBody);
            if (commandData[1] == "contacts")
                _dbProcessor.AddAllContactsInfo(requestBody);
            if (commandData[1] == "profile")
                _dbProcessor.AddProfile(requestBody);
            if (commandData[1] == "")
                return "Unkown request";
            return "Записано!";
        }
    }
}