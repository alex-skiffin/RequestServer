using System;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Server.DataBase;
using Server.Requester;

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
            if (commandData[1] == Requests.AllPhones)
                info = jsonSerialiser.Serialize(_dbProcessor.GetAllPhone());
            if (commandData[1] == Requests.AllContacts)
                info = jsonSerialiser.Serialize(_dbProcessor.GetAllInfo(commandData[2]));
            if (commandData[1] == Requests.Phone)
                info = jsonSerialiser.Serialize(_dbProcessor.GetPhone(commandData[2]));
            if (commandData[1] == Requests.OneContact)
                info = JsonConvert.SerializeObject(_dbProcessor.GetInfo(commandData[2]));
            if (commandData[1] == Requests.Profile)
                info = JsonConvert.SerializeObject(_dbProcessor.GetProfile(commandData[2]));
            if (commandData[1] == String.Empty)
                info = JsonConvert.SerializeObject(_dbProcessor.GetPhone());
            return info;
        }

        public string Post(string[] commandData, string requestBody)
        {
            if (commandData[1] == Requests.Phone)
                _dbProcessor.AddPhoneInfo(requestBody);
            if (commandData[1] == Requests.Contacts)
                _dbProcessor.AddAllContactsInfo(requestBody);
            if (commandData[1] == Requests.Profile)
                _dbProcessor.AddProfile(requestBody);
            if (commandData[1] == Requests.Register)
               return AddReg(requestBody);
            if (commandData[1] == String.Empty)
                return "Unkown request";
            return "Записано!";
        }

        private string AddReg(string authInfo)
        {
            var result = string.Empty;
            var auth = JsonConvert.DeserializeObject<AuthInfo>(authInfo);
            if (_dbProcessor.NameFree(auth.UserName))
            {
                _dbProcessor.AddRegisterInfo(authInfo);
                result = "ok";
            }
            return result;
        }
    }
}