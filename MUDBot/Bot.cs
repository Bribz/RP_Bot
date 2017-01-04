using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions;
using Discord.Net;
using Discord.Logging;
using Discord.Legacy;
using Discord.ETF;
using Discord.API;

namespace MUDBot
{
    public class Bot
    {
        private DiscordClient _client;
        private Server _server;
        private MUDWorld GameWorld;
        private ServerData SERVER_DATA;
        
        public Bot()
        {
            _client = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
            
            InitializeBot();
        }

        /// <summary>
        /// Handle all initialization of the bot.
        /// </summary>
        private void InitializeBot()
        {
            SERVER_DATA = DataManager.ReadServerDetails();
            
            _client.ExecuteAndWait( async () =>
            {
                await _client.Connect(SERVER_DATA.Token, TokenType.Bot);
                
                await Task.Delay(500);
                _server = _client.GetServer(SERVER_DATA.serverID);

                SetUpWorld();
                SetUpCommands();
            });


            GameWorld.Shutdown();
        }

        private void SetUpWorld()
        {
            GameWorld = new MUDWorld(_server);
            
            _client.UserUpdated += GameWorld.UserUpdatedCallback;


            var embed = new EmbedBuilder();
        }

        private void SetUpCommands()
        {
            _client.UsingCommands(x =>
            {
                x.PrefixChar = '/';
                x.HelpMode = HelpMode.Public;
            });

            var cService = _client.GetService<CommandService>();

            cService.CreateCommand("ping")
                .Do(PongEvent);

            cService.CreateCommand("shutdown")
                .Hide()
                .Description("Shut down server")
                .Do(async e =>
                {
                    if(e.User.HasRole(_server.FindRoles("Moderator", true).FirstOrDefault()))
                    {
                        await e.Channel.SendMessage($"Shutting down {_server.Name}...");
                        await Task.Delay(2000);
                        ShutDown();
                    }
                });

            cService.CreateGroup("Attack", cgb =>
            {
                cService.CreateCommand("Melee")

                    .Alias(new string[] { "m" })
                    .Description("Make a close combat attack at an enemy.")
                    .Parameter("Target", ParameterType.Required)
                    .Do(async e => { await e.Channel.SendMessage(""); });

            });

            cService.CreateCommand("Party")
                .Alias(new string[] { "Group" })
                .Description("Enter a party with the character")
                .Parameter("Character", ParameterType.Required)
                .Do(GameWorld.CreateParty);

            cService.CreateCommand("LeaveParty")
                .Alias(new string[] { "LeaveGroup" })
                .Description("Leave current party")
                .Do(GameWorld.LeaveParty);

            cService.CreateCommand("Move")
                    .Alias(new string[] { "Head" })
                    .Description("Move in a direction given")
                    .Parameter("Direction", ParameterType.Required)
                    .Do(GameWorld.MoveZoneSpace);

            cService.CreateCommand("roll")
                .Alias(new string[] { "r" })
                .Description("Roll a die")
                .Parameter("Die type (1d10)", ParameterType.Required)
                .Do(RollDie);

            cService.CreateCommand("charstats")
                .Alias(new string[] { "character", "char", "stats", "inv", "inventory" })
                .Do(GameWorld.GetCharacterStats);
        }

        private async Task RollDie(CommandEventArgs e)
        {
            string result = "  Failed to roll!";
            int numberDice = 1;
            int dieType = 1;
            string[] input = e.GetArg(0).Split(new char[] { 'd' });

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

                    for(int i = 0; i < dice.Length; i++)
                    {
                        result += $"<**{dice[i]}**> ";
                    }
                }
            }

            await e.Message.Delete();
            await e.Channel.SendMessage(e.User.Mention + result);
        }

        private async Task PongEvent(CommandEventArgs e)
        {
            
            await e.Channel.SendMessage("Pong!");
                //sends a message to channel with the given text
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"<{DateTime.Now}> : [{e.Severity}] [{e.Source}] : {e.Message}");
        }

        /// <summary>
        /// Shut down the bot and go offline. TODO: Make this private when commands finalized
        /// </summary>
        public void ShutDown()
        {
            GameWorld.Shutdown();

            _client.Disconnect();
            _client.Dispose();

            _client = null;
        }
    }
}
