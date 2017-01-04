using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MUDBot
{
    
    public class CombatMember
    {
        public static ulong NotAPlayer = 0;
        [JsonProperty("PlayerID")]
        public ulong playerID;
        [JsonProperty("Character")]
        public Character stats;

        [JsonIgnore]
        public Vector2 Location;
        [JsonIgnore]
        public int initiative;

        [JsonConstructor]
        public CombatMember()
        {
            stats = null;
            playerID = NotAPlayer;
            Location = Vector2.Zero;
            initiative = 0;
        }

        public CombatMember(Character character, GameUser user = null)
        {
            stats = character;
            if(user != null)
            {
                playerID = user.DiscordUserID;
            }

            Location = Vector2.Zero;
            initiative = 0;
        }

        /// This could be unsafe. 
        /// I doubt anything will ever cause this issue, but its possible for players to cheat this 
        /// if they hash their discordID and name to be the same as somebody elses.
        /// For our purposes. It's fine. I don't think it will ever even be called.
        public override int GetHashCode()
        {
            return stats.Name.GetHashCode() + playerID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            CombatMember member = obj as CombatMember;

            if (member == null)
                return false;

            if(this.playerID != NotAPlayer && member.playerID != NotAPlayer)
            {
                if (this.playerID == member.playerID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if(this.stats == member.stats)
            {
                return true;
            }

            return false;
        }
    }

    public class InitiativeComparer: IComparer<CombatMember>
    {
        public int Compare(CombatMember x, CombatMember y)
        {
            if (x.initiative < y.initiative)
            {
                return -1;
            }
            else if (x.initiative > y.initiative)
            {
                return 1;
            }
            else
                return 0;
        }
    }
}
