using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MUDBot
{
    public class DataManager
    {
        /*
        public static void WriteServerDetails(ServerData sd)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\ServerConfig.txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(sd, Formatting.Indented));
        }
        */

        public static ServerData ReadServerDetails()
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\ServerConfig.txt";
            string data = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<ServerData>(data);
        }

        public static void WriteJson(ulong userID, Character ch)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\Characters\\" + userID + ".txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(ch, Formatting.Indented));
        }

        public static void ReadJson(ulong userID, out Character c)
        {
            string path = Directory.GetCurrentDirectory();

            if (!File.Exists(path + "\\Data\\Users\\" + userID + ".txt"))
            {
                c = null;
                Console.WriteLine($"Error: Failed to find <Character> JSON file for userID<{userID}>");
                return;
            }

            string data = File.ReadAllText(path + "\\Data\\Characters\\" + userID + ".txt");


            c =  JsonConvert.DeserializeObject<Character>(data);
        }

        public static void WriteJson(GameUser user)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\Users\\" + user.DiscordUserID + ".txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();
            }

             File.WriteAllText(path, JsonConvert.SerializeObject(user, Formatting.Indented));
        }

        public static void ReadJson(ulong discordUserID, out GameUser user)
        {
            string path = Directory.GetCurrentDirectory();

            if(!File.Exists(path + "\\Data\\Users\\" + discordUserID + ".txt"))
            {
                user = null;
                Console.WriteLine($"Error: Failed to find <GameUser> JSON file for userID<{discordUserID}>");
                return;
            }

            string data = File.ReadAllText(path + "\\Data\\Users\\" + discordUserID + ".txt");

            user = JsonConvert.DeserializeObject<GameUser>(data);
        }

        

        public static MUDObject GetObjectData(int objectID)
        {
            string path = Directory.GetCurrentDirectory();

            if (!File.Exists(path + "\\Data\\Objects\\" + objectID + ".txt"))
            {
                Console.WriteLine($"Error: Failed to find <MUDObject> JSON file for objectID<{objectID}>");
                return null;
            }

            string data = File.ReadAllText(path + "\\Data\\Objects\\" + objectID + ".txt");

            return JsonConvert.DeserializeObject<MUDObject>(data);
        }

        public static void WriteJson(MUDObject _object, int objectID)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\Objects\\" + objectID + ".txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(_object, Formatting.Indented));
        }

        public static void WriteJson(Party party, int partyID)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\Parties\\" + partyID + ".txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(party, Formatting.Indented));
        }

        public static void DeletePartyJson(int partyID)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\Parties\\" + partyID + ".txt";

            if (File.Exists(path))
                File.Delete(path);
        }

        public static List<Party> ReadAllParties()
        {
            List<Party> partybase = new List<Party>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Data\\Parties\\");
            Party nParty;
            foreach (string s in files)
            {
                nParty = JsonConvert.DeserializeObject<Party>(File.ReadAllText(s));
                if (nParty != null)
                {
                    partybase.Add(nParty);
                }

            }
            return partybase;
        }

        public static List<GameUser> ReadAllUsers()
        {
            List<GameUser> userbase = new List<GameUser>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Data\\Users\\");
            GameUser ngUser;
            foreach(string s in files)
            {
                ngUser = JsonConvert.DeserializeObject<GameUser>(File.ReadAllText(s));
                if(ngUser != null)
                {
                    userbase.Add(ngUser);
                }
                
            }
            return userbase;
        }

        public static void WriteJson(Zone zone)
        {
            string path = Directory.GetCurrentDirectory() + $"\\Data\\Zones\\"+zone.ZoneID.x+"_"+zone.ZoneID.y+".txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();

            }

            File.WriteAllText(path, JsonConvert.SerializeObject(zone, Formatting.Indented));
            
        }

        public static List<Zone> ReadAllZones(int size)
        {
            Zone[] zones = new Zone[size*size];
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Data\\Zones\\");
            Zone nZone;
            foreach (string s in files)
            {
                nZone = JsonConvert.DeserializeObject<Zone>(File.ReadAllText(s));
                if (nZone != null)
                {
                    zones[nZone.ZoneID.x + (nZone.ZoneID.y * size)] = nZone;
                }

            }
            return zones.ToList();
        }

        public static void WriteJson(MUDObject obj)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data\\Objects\\" + obj.Name + ".txt";

            if (!File.Exists(path))
            {
                File.CreateText(path).Close();
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        public static List<MUDObject> ReadAllObjects()
        {
            List<MUDObject> objectList = new List<MUDObject>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Data\\Objects\\");
            MUDObject nObj;
            foreach (string s in files)
            {
                nObj = JsonConvert.DeserializeObject<MUDObject>(File.ReadAllText(s));
                if (nObj != null)
                {
                    objectList.Add(nObj);
                }

            }
            return objectList;
        }
    }
}
