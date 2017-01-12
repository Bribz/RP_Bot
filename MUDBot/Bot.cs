using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.InteractiveCommands;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;
using System.Reflection;

namespace MUDBot
{
    public class Bot
    {
        #region Singleton_Stuff
        private static Bot Instance;

        public static Bot instance
        {
            get
            {
                if(Instance == null)
                {
                    Instance = new Bot();
                }
                return Instance;
            }
        }
        #endregion
        public DiscordSocketClient _client;
        public SocketGuild _server;
        public MUDWorld GameWorld;
        private CommandService commands;
        private DependencyMap map;
        public ServerData SERVER_DATA;
        public bool running;
        
        public Bot()
        {   
            //Legacy Code
            /*
            _client = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
            */
            Task.Run(InitializeBot).GetAwaiter().GetResult();
            
        }

        private async Task _client_Log(LogMessage e)
        {
            Console.WriteLine($"<{DateTime.Now}> : [{e.Severity}] [{e.Source}] : {e.Message}");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Handle all initialization of the bot.
        /// </summary>
        private async Task InitializeBot()
        {
            running = true;

            var clientConfig = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                DefaultRetryMode = RetryMode.Retry502 | RetryMode.RetryTimeouts,
                ConnectionTimeout = 20000,
                MessageCacheSize = 10
            };

            _client = new DiscordSocketClient(clientConfig);

            _client.Log += _client_Log;

            SERVER_DATA = DataManager.ReadServerDetails();

            ServicePointManager.ServerCertificateValidationCallback = Validator;
            
            await _client.LoginAsync(TokenType.Bot, SERVER_DATA.Token);

            await _client.ConnectAsync(true);

            await Task.Delay(1000);

            SetUpWorld();
            await SetUpCommands();

            await Task.Delay(-1);
        }

        private void SetUpWorld()
        {
            _server = _client.GetGuild(SERVER_DATA.serverID);
            GameWorld = new MUDWorld(_server);

            _client.UserUpdated += GameWorld.UserUpdatedCallback; 
        }

        private async Task SetUpCommands()
        {
            commands = new CommandService();
            map = new DependencyMap();

            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            /*
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

            */
        }

        public async Task ChangeNickname(IUser user, string proposedName)
        {
            await (user as SocketGuildUser).ModifyAsync(x => { x.Nickname = proposedName; });
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('>', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            // Create a Command Context
            var context = new CommandContext(_client, message);

            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed succesfully)
            var result = await commands.ExecuteAsync(context, argPos, map);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
        

        /// <summary>
        /// Shut down the bot and go offline. TODO: Make this private when commands finalized
        /// </summary>
        public void ShutDown()
        {
            GameWorld.Shutdown();

            _client.DisconnectAsync();
            _client.Dispose();

            _client = null;
            running = false;
        }

        public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain,
                                      SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
