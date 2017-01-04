using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MUDBot
{
    public class ServerData
    {
        [JsonProperty("ServerID")]
        public ulong serverID { get; set; }
        [JsonProperty("AppToken")]
        public string Token { get; set; }

        public ServerData()
        {
            serverID = 0;
            Token = "";
        }

        public ServerData(ulong id, string tok)
        {
            serverID = id;
            Token = tok;
        }
    }
}
