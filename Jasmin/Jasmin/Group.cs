using FireSharp.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jasmin
{
    class Group
    {
        public Group() { }

        public Group(System.Collections.Generic.KeyValuePair<string,Newtonsoft.Json.Linq.JToken> JSON)
        {
            string ceva=" ";
            this.Name = JSON.Key;
            
            foreach (var kid in JSON.Value)
            {
                messages.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<MessageData>(kid.ToString()));

            }
            //var current = JObject.Parse(JSON);
        }
        public string Name { get; set; }
        public List<MessageData> messages = new List<MessageData>();

    }
}
