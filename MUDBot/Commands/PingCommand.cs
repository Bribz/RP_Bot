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
    public class PingCommand : ModuleBase
    {
        // ~say hello -> hello
        [Command("ping"), Summary("Echos a message.")]
        public async Task Ping()
        {
            var embed = new EmbedBuilder();
            embed.Color = new Color(104, 128, 173);
            //embed.ThumbnailUrl = "http://i104.photobucket.com/albums/m161/kai7735/MortalWorldICON_zpst6sya4so.png";
            //embed.Description = "Pong!";
            embed.Footer = new EmbedFooterBuilder() { Text = "Pong!" };
            embed.Author = new EmbedAuthorBuilder() { IconUrl = "http://i104.photobucket.com/albums/m161/kai7735/MortalWorldICON_zpst6sya4so.png", Name = "Mudbot" };
            embed.Build();


            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
