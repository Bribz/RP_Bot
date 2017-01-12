using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MUDBot
{
    public class CombatInstance
    {
        private MUDWorld world;
        private SocketTextChannel c;
        private List<CombatMember> participants;
        private int currentTurn;
        bool combatStarted;

        public CombatInstance(MUDWorld _world)
        {
            participants = new List<CombatMember>();
            world = _world;
            currentTurn = 0;
            combatStarted = false;
        }

        public async void StartCombat()
        {
            foreach(CombatMember participant in participants)
            {
                participant.initiative = world.RollDice(10) + participant.stats.Dexterity;
            }
            participants.Sort(new InitiativeComparer());

            combatStarted = true;
        }

        public void JoinCombatInstance(CombatMember participant)
        {
            if (participant.stats == null)
                return;

            if(combatStarted)
            {
                participants.Insert(currentTurn + 1, participant);
            }
            else
            {
                participants.Add(participant);
            }
        }

        public bool AttemptEscape(GameUser user)
        {
            CombatMember participant = participants.Find(x => x.playerID == user.DiscordUserID);
            if(participant == null)
            {
                Console.WriteLine($"Error: Did not find user {user.DiscordUserID} in combat instance.");
                return false;
            }

            int escapeScore = world.RollDice(10) + participant.stats.CurrentDexterity;
            bool success = true;

            for(int i = 0; i < participants.Count-1; i++)
            {
                if (participants[i] == participant) continue;

                if (escapeScore < (world.RollDice(10) + participants[i].stats.CurrentDexterity))
                {
                    success = false;
                    break;
                }
            }
            
            return success;
        }

        public bool IsTurn(GameUser user)
        {
            CombatMember participant = participants.Find(x => x.playerID == user.DiscordUserID);
            if (participant == null)
            {
                Console.WriteLine($"Error: Did not find user {user.DiscordUserID} in combat instance.");
                return false;
            }

            if (participants[currentTurn].playerID == user.DiscordUserID)
            {
                return true;
            }
            else
                return false;
        }

        public async void NextTurn(SocketTextChannel channel)
        {
            currentTurn++;
            if(currentTurn >= participants.Count)
            {
                currentTurn = 0;
            }

            if(participants[currentTurn].playerID != 0)
            {
                await channel.SendMessageAsync($"{channel.GetUser(participants[currentTurn].playerID).Nickname}, it is now your turn!");
            }
            else
            {
                HandleNPCTurn(channel);
            }
        }

        private async void HandleNPCTurn(SocketTextChannel channel)
        {
            await channel.SendMessageAsync("ERROR: not yet implemented.");
            NextTurn(channel);
        }

        public async void LeaveCombatInstance(Character c, SocketTextChannel channel)
        {
            CombatMember mb = participants.Find(x => x.stats == c);
            if(mb == null)
            {
                Console.WriteLine($"Error: Did not find character {c} in combat instance.");
                return;
            }
            if(participants[currentTurn]==mb)
            {
                await Task.Delay(100);
                NextTurn(channel);
            }
            participants.Remove(mb);
        }
    }
}
