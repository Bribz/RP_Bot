using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MUDBot
{
    public class GameUser
    {
        [JsonProperty("DiscordID")]
        public ulong DiscordUserID;
        [JsonProperty("CharacterStats", IsReference =true, NullValueHandling = NullValueHandling.Include)]
        public Character Character;

        [JsonProperty("CreationStatus")]
        public int CharacterCreation;
        [JsonIgnore]
        public bool inCombat;
        [JsonIgnore]
        public bool traveling;
        [JsonIgnore]
        public bool Online;
        [JsonProperty("PartyID")]
        public int PartyID;

        [JsonConstructor]
        public GameUser()
        {
            CharacterCreation = 0;
            inCombat = false;
            traveling = false;
            Online = false;
            PartyID = Party.NotInParty;
        }

        public GameUser(Discord.User user)
        {
            DiscordUserID = user.Id;
            traveling = false;
            PartyID = Party.NotInParty;
        }

        public GameUser(Discord.User user, Character character)
        {
            DiscordUserID = user.Id;
            Character = character;
            traveling = false;
            PartyID = Party.NotInParty;
        }

        public bool isDead()
        {
            return Character.CurrentHealth <= 0;
        }
    }
}
