using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace MUDBot
{
    public class PartyManager
    {
        private MUDWorld world;
        private List<Party> Parties;
        private int nextPartyID;

        public PartyManager(MUDWorld _world)
        {
            world = _world;
            Parties = DataManager.ReadAllParties();
            if(Parties == null)
            {
                Parties = new List<Party>();
            }

            Parties.Sort(new PartyComparer());
            nextPartyID = 0;
            for (int i = 0; i < Parties.Count; i++)
            {
                if(Parties[i].PartyID == nextPartyID)
                {
                    nextPartyID++;
                }
                else
                {
                    break;
                }
            }
        }

        public void ShutDown()
        {
            foreach(var v in Parties)
            {
                DataManager.WriteJson(v, v.PartyID);
            }
        }

        public async void JoinParty(CombatMember member, Channel channel, int partyID)
        {
            Party party = GetParty(partyID);
            if(party == null)
            {
                Console.WriteLine($"Error: Null party retrieving at {partyID}");
            }

            party.AddToParty(member);

            await channel.SendMessage($"{member.stats.Name} has joined the Party!");
        }

        public Party GetParty(int partyID)
        {
            try
            {
                return Parties.Find(x => x.PartyID == partyID);
            }
            catch(Exception e)
            {
                return null;
            }
            
        }

        public async Task CreateParty(CombatMember p1, CombatMember p2, Channel channel)
        {
            List<CombatMember> memberData = new List<CombatMember>();
            memberData.Add(p1);
            memberData.Add(p2);
            Party party = null;
            for (int i = 0; i < Parties.Count; i++)
            {
                if (Parties[i].PartyID == nextPartyID)
                {
                    nextPartyID++;
                }
                else
                {
                    break;
                }
            }
            party = new Party(memberData, nextPartyID);

            if (nextPartyID < Parties.Count)
            {

                Parties.Insert(nextPartyID, party);
            }
            else
            {
                Parties.Add(party);
            }

            foreach(var v in memberData)
            {
                GameUser g = world.FindGameUser(v.playerID);
                if (g != null)
                {
                    g.PartyID = nextPartyID;
                }
            }

            await (channel.SendMessage($"{p1.stats.Name} and {p2.stats.Name} are now in a party!"));

            DataManager.WriteJson(party, nextPartyID);
        }

        public async void RemoveFromParty(CombatMember member, int partyID, Channel channel)
        {
            Party party = GetParty(partyID);
            party.RemoveFromParty(member);

            if(member.playerID != 0)
            {
                world.FindGameUser(member.playerID).PartyID = Party.NotInParty;
            }

            await channel.SendMessage($"{member.stats.Name} has left the party.");

            if(party.GetPartyCount() <= 1)
            {
                DisbandParty(partyID, channel);
            }
        }   

        public async void DisbandParty(int partyID, Channel channel)
        {
            nextPartyID = partyID;

            foreach(var pm in GetParty(partyID).GetPartyMembers())
            {
                if(pm.playerID != 0)
                {
                    world.FindGameUser(pm.playerID).PartyID = 0;
                }
            }
            

            await channel.SendMessage("The party has been disbanded.");

            DataManager.DeletePartyJson(partyID);

        }
    }
}
