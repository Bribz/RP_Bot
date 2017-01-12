using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using Discord.WebSocket;

namespace MUDBot
{
    public class Party
    {
        
        public const int NotInParty = -1;

        [JsonProperty("PartyData")]
        private List<CombatMember> PartyMembers;
        [JsonProperty("PartyID")]
        public int PartyID;

        [JsonConstructor]
        public Party()
        {
            PartyMembers = new List<CombatMember>();
        }

        
        public Party(List<CombatMember> data, int partyID)
        {
            PartyMembers = data;
            PartyID = partyID;
        }


        public void AddToParty(CombatMember member)
        {
            PartyMembers.Add(member);
        }

        public void RemoveFromParty(CombatMember member)
        {
            PartyMembers.Remove(member);
        }

        public async void PartyDetails(SocketTextChannel channel)
        {
            string List = "Members in party: \n";

            foreach (var v in PartyMembers)
            {
                List += $"{v.stats.Name}";
            }

            await channel.SendMessageAsync(List);
        }

        public List<CombatMember> GetPartyMembers()
        {
            return PartyMembers;
        }

        public int GetPartyCount()
        {
            return PartyMembers.Count;
        }

        public async void JoinBattle(CombatInstance instance, SocketTextChannel channel, MUDWorld world)
        {
            foreach(var pm in PartyMembers)
            {
                if (world.FindGameUser(pm.playerID).inCombat)
                {
                    await channel.SendMessageAsync("Your party is already in combat!");
                    return;
                }
            }

            foreach(var pm in PartyMembers)
            {
                
                instance.JoinCombatInstance(new CombatMember(pm.stats,world.FindGameUser(pm.playerID)));
                await channel.SendMessageAsync($"{pm.stats.Name} has joined the fight!");
                world.FindGameUser(pm.playerID).inCombat = true;
            }
        }

        public async void LeaveBattle(CombatInstance instance, SocketTextChannel channel, MUDWorld world)
        {
            foreach (var pm in PartyMembers)
            {
                instance.LeaveCombatInstance(pm.stats, channel);
                world.FindGameUser(pm.playerID).inCombat = false;
                await channel.SendMessageAsync($"{pm.stats.Name} has left the fight!");
            }
        }
    }

    public class PartyComparer : IComparer<Party>
    {
        public int Compare(Party x, Party y)
        {
            if (x.PartyID < y.PartyID)
            {
                return -1;
            }
            else if (x.PartyID > y.PartyID)
            {
                return 1;
            }
            else
                return 0;
        }
    }
}
