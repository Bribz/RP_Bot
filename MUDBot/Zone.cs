using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord;
using Discord.WebSocket;

namespace MUDBot
{
    public class Zone
    {
        //[JsonProperty("NPCs")]
        //public List<int> NPC_IDs;

        [JsonIgnore]
        public List<GameUser> Players;

        [JsonProperty("Objects")]
        public List<int> OBJECT_IDS;
        

        [JsonProperty("Description")]
        public string Description;

        [JsonIgnore]
        public SocketGuildChannel ZoneChannel;

        [JsonProperty("Location")]
        public Vector2 ZoneID;
        
    

        [JsonIgnore]
        public List<MUDObject> objects;

        public Zone()
        {
            OBJECT_IDS = new List<int>();
            objects = new List<MUDObject>();
            Players = new List<GameUser>();
        }

        [JsonConstructor]
        public Zone(List<int> object_ids)
        {
            Players = new List<GameUser>();

            OBJECT_IDS = object_ids;

            foreach (var objID in OBJECT_IDS)
            {
                MUDObject mObj = DataManager.GetObjectData(objID);
                if (mObj != null)
                {
                    objects.Add(mObj);
                }
            }
        }

        public void SetChannel(IGuildChannel zoneChannel)
        {
            ZoneChannel = zoneChannel as SocketGuildChannel;
        }

        public void RemoveChannel()
        {
            ZoneChannel = null;
        }

        public void AddPlayer(GameUser user)
        {
            Players.Add(user);
        }

        public void RemovePlayer(GameUser user)
        {
            Players.Remove(user);
        }
    }
}
