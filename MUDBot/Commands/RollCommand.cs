using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MUDBot.Commands
{
    public class RollCommand : ModuleBase
    {
        // ~say hello -> hello
        [Command("say"), Summary("Echos a message.")]
        public async Task Roll([Summary("Roll command, Example: 2d10")] string e)
        {
            string result = "  Failed to roll!";
            int numberDice = 1;
            int dieType = 1;
            string[] input = e.Split(new char[] { 'd' });

            if (Int32.TryParse(input[0], out numberDice) && Int32.TryParse(input[1], out dieType))
            {
                Random r = new Random();
                int[] dice = new int[numberDice];

                int total = 0;
                for (int i = 0; i < numberDice; i++)
                {
                    dice[i] = r.Next(1, dieType);
                    total += dice[i];
                }

                result = $"  **You rolled a** ***{total}*** **!**\n";

                if (numberDice > 1)
                {
                    result += "**Breakdown of rolls:** ";

                    for (int i = 0; i < dice.Length; i++)
                    {
                        result += $"<**{dice[i]}**> ";
                    }
                }
            }

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(Context.User.Mention + result);
        }
    }
}
