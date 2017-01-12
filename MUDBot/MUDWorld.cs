using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net;
using Discord.API;
using Discord.Rest;
using Discord.Addons.InteractiveCommands;

namespace MUDBot
{
    public class MUDWorld
    {
        private List<GameUser> Users;
        private SocketGuild server;
        public ChannelManager ChannelManager;
        public CombatManager CombatManager;
        public PartyManager PartyManager;
        public ZoneManager ZoneManager;
        public ObjectManager ObjectManager;
        private bool ready;
        internal int MUDSize = 5;

        public MUDWorld(SocketGuild _server)
        {
            Users = DataManager.ReadAllUsers();
            server = _server;
            ObjectManager = new ObjectManager();
            ZoneManager = new ZoneManager(_server, this);
            ChannelManager = new ChannelManager(_server, this);
            CombatManager = new CombatManager(this);
            PartyManager = new PartyManager(this);
            ready = true;
        }

        public GameUser FindGameUser(ulong userID)
        {
            return Users.Find(x => x.DiscordUserID == userID);
        }

        public void Shutdown()
        {
            Console.WriteLine("Saving Data...");

            ChannelManager.Shutdown();

            foreach (GameUser g in Users)
            {
                DataManager.WriteJson(g);
            }

            ZoneManager.ShutDown();

            PartyManager.ShutDown();

            Console.WriteLine("Done!");
        }

        /*
        public async Task GetCharacterStats(CommandEventArgs e)
        {
            Message post = null;
            GameUser user = Users.Find(u => u.DiscordUserID == e.User.Id);
            if (user != null)
            {
                if (user.CharacterCreation == -1 && user.Character != null)
                {
                    await e.User.SendMessage(user.Character.ToString());
                }
                else
                {
                    post = await e.Channel.SendMessage(e.User.NicknameMention + " You do not have an alive character!");
                }
            }
            else
            {
                Console.WriteLine("Error: Discord User does not have matching GameUser!");
            }
            await e.Message.Delete();
            if (post != null)
            {
                await Task.Delay(4000);
                await post.Delete();
            }
        }
        */

        public int RollDice(string inputValue)
        {
            int numberDice = 1;
            int dieType = 1;
            string[] input = inputValue.Split(new char[] { 'd' });

            int total = 0;

            if (Int32.TryParse(input[0], out numberDice) && Int32.TryParse(input[1], out dieType))
            {
                Random r = new Random();
                int[] dice = new int[numberDice];

                total = 0;
                for (int i = 0; i < numberDice; i++)
                {
                    dice[i] = r.Next(1, dieType);
                    total += dice[i];
                }
            }
            return total;
        }

        /*
        public async Task CreateParty(CommandEventArgs e)
        {
            GameUser p1 = FindGameUser(e.User.Id);
            if(p1.inCombat)
            {
                await e.Channel.SendMessage("You cant party with somebody in battle!");
                return;
            }

            CombatMember m1 = new CombatMember(p1.Character, p1);
            CombatMember m2 = null;
            GameUser p2 = null;

            //Console.WriteLine(e.Args[0]);
            string parameter = e.Args[0];
            
            //Player
            if (e.Args[0].Contains("@"))
            {
                parameter = parameter.Replace("@", "").Replace("!","").Replace("<", "").Replace(">", "");

                ulong ID;
                ulong.TryParse(parameter, out ID);
                p2 = FindGameUser(ID);

                if (p2.inCombat)
                {
                    await e.Channel.SendMessage("You cant party with somebody in battle!");
                    return;
                }

                m2 = new CombatMember(p2.Character, p2);
            }
            //NPC
            else
            {
                await e.Channel.SendMessage($"ERROR: not yet implemented");
            }

            if (m1 != null && m2 != null)
            {
                if (p1.PartyID != Party.NotInParty)
                {
                    //add p2 to p1 party
                    PartyManager.JoinParty(m2, e.Channel, p1.PartyID);
                }
                else if (p2 != null)
                {
                    if (p2.PartyID != Party.NotInParty)
                    {
                        //add p1 to p2 party
                        PartyManager.JoinParty(m1, e.Channel, p2.PartyID);
                    }
                    else
                    {
                        await PartyManager.CreateParty(m1, m2, e.Channel);
                    }
                }
                else
                {
                    await PartyManager.CreateParty(m1, m2, e.Channel);
                }
            }
            else
                Console.WriteLine("ERROR: Error creating party, member is npc!");
        }

        public async Task LeaveParty(CommandEventArgs e)
        {
            GameUser user = FindGameUser(e.User.Id);
            if(user.PartyID == Party.NotInParty)
            {
                await e.Channel.SendMessage($"{e.User.NicknameMention}, you are not in a party!");
            }
            else
            {
                PartyManager.RemoveFromParty(new CombatMember(user.Character, user), user.PartyID, e.Channel);
            }
        }
        */

        /*
        public async Task MoveZoneSpace(CommandEventArgs e)
        {

            Vector2 direction = Vector2.Zero;
            switch(e.Args[0].ToLower())
            {
                case "n":
                case "north":
                    {
                        direction = new Vector2(0, -1);
                        break;
                    }
                case "e":
                case "east":
                    {
                        direction = new Vector2(1,0);
                        break;
                    }
                case "s":
                case "south":
                    {
                        direction = new Vector2(0, 1);
                        break;
                    }
                case "w":
                case "west":
                    {
                        direction = new Vector2(-1, 0);
                        break;
                    }
                default:
                    {
                        await e.Channel.SendMessage("Error, check syntax");
                        return;
                    }
            }

            GameUser gUser = FindGameUser(e.User.Id);

            Vector2 currentZone = new Vector2(gUser.Character.Zone%MUDSize, gUser.Character.Zone/MUDSize);

            Vector2 finalZone = currentZone + direction;
            if (finalZone.x < 0 || finalZone.x >= MUDSize || finalZone.y < 0 || finalZone.y >= MUDSize)
            {
                await e.Channel.SendMessage($"Something is blocking you from heading futher {e.Args[0]}");
            }
            else
            {
                bool verbose = true;
                if(gUser.PartyID != Party.NotInParty)
                {
                    foreach(var player in PartyManager.GetParty(gUser.PartyID).GetPartyMembers())
                    {
                        if(player.playerID != CombatMember.NotAPlayer)
                        {
                            ZoneManager.ChangeZone(gUser, finalZone, ZoneManager.GetZone(currentZone), $"You travel with your party to the {e.Args[0]} and find ", verbose);
                            verbose = false;
                        }
                    }
                }
                else
                {
                    ZoneManager.ChangeZone(gUser, finalZone, ZoneManager.GetZone(currentZone), $"You travel {e.Args[0]} and find ");
                }
                
            }
        }
        */

        public int RollDice(int dieType, int numberDice = 1)
        {

            int total = 0;
            
            Random r = new Random();
            int[] dice = new int[numberDice];

            total = 0;
            for (int i = 0; i < numberDice; i++)
            {
                dice[i] = r.Next(1, dieType);
                total += dice[i];
            }

            return total;
        }
        
        public bool NameExists(string name)
        {
            return !(Users.Exists(x => x.Character.Name == name));
        }

        private GameUser FindOrCreateNewUser(SocketGuildUser input)
        {
            GameUser user = Users.Find(x => x.DiscordUserID == input.Id);
            if  (user == null)
            {
                user = new GameUser(input);
                Users.Add(user);
            }

            return user;
        }

        
        public async Task UserUpdatedCallback(SocketUser before, SocketUser after)
        {
            SocketGuildUser Before = before as SocketGuildUser;
            SocketGuildUser After = after as SocketGuildUser;

            if (!ready)
                return;
            
            if(Before.Nickname != After.Nickname)
            {
                GameUser user = FindOrCreateNewUser(After);
                if (user.Character != null && user.CharacterCreation == -1)
                {
                    user.Character.ForceNameChange(After.Nickname);
                }
            }
            
            if (After.VoiceChannel != Before.VoiceChannel)
            {
                if (After.VoiceChannel != null && After.VoiceChannel.Name == "Online")
                {
                    if (Before.VoiceChannel == null || Before.VoiceChannel.Name == "Offline")
                    {
                        GameUser user = FindOrCreateNewUser(After);
                        user.Online = true;
                        Console.WriteLine("User logged in!");
                        if (user.Character == null)
                        {
                            SocketGuildChannel newChannel = await ChannelManager.CreatePrivateChannel(After) as SocketGuildChannel;
                            //HandleNewUser(user, newChannel);
                        }
                        else
                        {
                            ZoneManager.ChangeZone(user, user.Character.Zone, null, $"A ring of magical circles encompass your body as you are deposited back again at ");
                        }
                    }
                }
                else if (After.VoiceChannel == null || After.VoiceChannel.Name == "Offline")
                {
                    if (Before.VoiceChannel != null && Before.VoiceChannel.Name == "Online")
                    {
                        GameUser user = FindOrCreateNewUser(After);
                        user.Online = false;
                        user.inCombat = false;
                        if(user.Character != null && ZoneManager.GetZone(user.Character.Zone).ZoneChannel != null)
                        ChannelManager.LeaveChannel(After, ZoneManager.GetZone(user.Character.Zone).ZoneChannel as SocketTextChannel);
                        //DataManager.WriteJson(user);
                        Console.WriteLine("User logged out!");
                    }
                }
            }
        }

        public static EmbedBuilder BuildCharacterEmbed()
        {
            var embed = new EmbedBuilder();
            embed.Color = new Color(104, 128, 173);
            embed.Title = "Character Stat Sheet";
            return embed;
        }

        public static string BuildCharacterString(Character character, CharSheetType sheetType = CharSheetType.Complete)
        {
            #region CharacterInfo
            string retStr 
                = $"Name: {character.Name}\n";

            if ((sheetType & CharSheetType.CharInfo) == CharSheetType.CharInfo)
                retStr += $"Gender: {character.Gender}\n";
            
            if ((sheetType & CharSheetType.CharInfoVerbose) == CharSheetType.CharInfoVerbose)
                retStr += $"Profession: {character.Profession}\n";

            //if ((sheetType & CharSheetType.CharInfoVerbose) == CharSheetType.CharInfoVerbose)
            //    retStr += $"Affinities: {character.Profession}\n";

            if ((sheetType & CharSheetType.CharInfoVerbose) == CharSheetType.CharInfoVerbose)
                retStr += $"Description: {character.Desc}\n";
            
            #endregion

            retStr += "\n";

            if ((sheetType & CharSheetType.Stats) == CharSheetType.Stats || (sheetType & CharSheetType.StatsVerbose) == CharSheetType.StatsVerbose)
                retStr += "Stats: \n";
            #region StatBlock
            if ((sheetType & CharSheetType.StatsVerbose) == CharSheetType.StatsVerbose)
                retStr += $"Health: {character.CurrentHealth}/{character.Health}\n\n";
            else if ((sheetType & CharSheetType.Stats) == CharSheetType.Stats)
                retStr += $"Health: {character.Health}\n\n";

            if ((sheetType & CharSheetType.StatsVerbose) == CharSheetType.StatsVerbose)
                retStr += $"Strength: {character.CurrentStrength}/{character.Strength}\n";
            else if ((sheetType & CharSheetType.Stats) == CharSheetType.Stats)
                retStr += $"Strength: {character.Strength}\n";

            if ((sheetType & CharSheetType.StatsVerbose) == CharSheetType.StatsVerbose)
                retStr += $"Dexterity: {character.CurrentDexterity}/{character.Dexterity}\n";
            else if ((sheetType & CharSheetType.Stats) == CharSheetType.Stats)
                retStr += $"Dexterity: {character.Dexterity}\n";

            if ((sheetType & CharSheetType.StatsVerbose) == CharSheetType.StatsVerbose)
                retStr += $"Intelligence: {character.CurrentIntelligence}/{character.Intelligence}\n\n";
            else if ((sheetType & CharSheetType.Stats) == CharSheetType.Stats)
                retStr += $"Intelligence: {character.Intelligence}\n\n";

            if ((sheetType & CharSheetType.StatsVerbose) == CharSheetType.StatsVerbose)
                retStr += $"Mana: {character.CurrentMana}/{character.Mana}\n";
            else if ((sheetType & CharSheetType.Stats) == CharSheetType.Stats)
                retStr += $"Mana: {character.Mana}\n";

            #endregion 

            retStr += "\n";

            if ((sheetType & CharSheetType.Experience) == CharSheetType.Experience)
                retStr += $"Experience: {character.Experience}\n";


            return retStr;
        }


        public static EmbedBuilder BuildNpcEmbed(string subtext, string responderName = null, string ThumbnailURL = null)
        {
            var embed = new EmbedBuilder();
            embed.Color = new Color(104, 128, 173);
            //embed.ThumbnailUrl = "http://i104.photobucket.com/albums/m161/kai7735/MortalWorldICON_zpst6sya4so.png";
            if(ThumbnailURL != null)
            {
                embed.ThumbnailUrl = ThumbnailURL;
            }
            //embed.Description = "Pong!";
            if(subtext != null)
            {
                embed.Footer = new EmbedFooterBuilder() { Text = subtext };
            }
            
            embed.Author = new EmbedAuthorBuilder() { IconUrl = "http://i104.photobucket.com/albums/m161/kai7735/MortalWorldICON_zpst6sya4so.png", Name = responderName!=null?responderName:"NPC" };
            embed.Build();

            return embed;
        }
        
        /*
        private async void HandleNewUser(GameUser user, SocketTextChannel newChannel)
        {
            string proposedName = "";
            Gender proposedGender = Gender.Female;
            SocketGuildUser discordUser = server.GetUser(user.DiscordUserID);

            await newChannel.SendMessageAsync($"**Welcome to {server.Name}.**\n\n Many adventures and mishaps await you in the world, as you will soon discover.\n\nFor all rules, see {server.GetChannel(260853439695683584).Mention} for details.");
            
            
            await Task.Delay(5000);
            RestUserMessage tmp = await newChannel.SendMessageAsync($"\"For you, {server.GetUser(user.DiscordUserID).Mention}, humble beginnings await your character.\"\n\"But first and foremost, are you a Boy, Girl, or Neither?\"\n*Choices:* ***Boy***, ***Girl***, ***Neither***");
            await newChannel.AddPermissionOverwriteAsync(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            DateTimeOffset origTimeStamp = tmp.Timestamp;
            int timeout = 300;
            await Task.Delay(2000);
            SocketUserMessage[] buffer;

            while (user.CharacterCreation == 0)
            {
                buffer = await newChannel.DownloadMessages(1, null, Relative.Before, true);

                if (buffer.Length != 0)
                {
                    if (buffer[0].User.Id == user.DiscordUserID)
                    {
                        if (buffer[0].Timestamp.CompareTo(origTimeStamp) > 0)
                        {
                            if (buffer[0].Text.ToLower().Equals("boy"))
                            {
                                proposedGender = Gender.Male;
                                user.CharacterCreation++;
                                break;
                            }
                            else if (buffer[0].Text.ToLower().Equals("girl"))
                            {
                                proposedGender = Gender.Female;
                                user.CharacterCreation++;
                                break;
                            }
                            else if (buffer[0].Text.ToLower().Equals("neither"))
                            {
                                proposedGender = Gender.Genderless;
                                user.CharacterCreation++;
                                break;
                            }
                            else
                            {
                                await newChannel.SendMessage("\"I didn't quite catch that.\" (check spelling)");
                            }
                        }
                    }
                }

                if (timeout <= 0 || user.Online == false)
                {
                    user.Character = null;
                    await newChannel.SendMessage("***Character Creation Timed out. Please go offline and try again***");
                    user.CharacterCreation = 0;
                    ChannelManager.DeleteChannel(newChannel);
                    return;
                }
                await Task.Delay(2000);
                timeout--;
            }
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            await newChannel.SendIsTyping();
            await Task.Delay(4000);

            tmp = await newChannel.SendMessage($"\"You look rather dashing for a {proposedGender}.\n\n However, what should we call you?\"");
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            origTimeStamp = tmp.Timestamp;
            await Task.Delay(500);
            timeout = 300;
            while (user.CharacterCreation == 1)
            {
                buffer = await newChannel.DownloadMessages(1, null, Relative.Before, true);

                if (buffer.Length != 0)
                {
                    if (buffer[0].User.Id == user.DiscordUserID)
                    {
                        if (buffer[0].Timestamp.CompareTo(origTimeStamp) > 0)
                        {
                            proposedName = buffer[0].Text;
                            if(proposedName.Length < 15)
                            {
                                user.CharacterCreation++;
                                break;
                            }
                            else
                            {
                                await newChannel.SendMessage("\"Thats a little long. Maybe a shorter name?\"\n\n*Name was too long*");
                            }
                        }
                    }
                }
                if (timeout <= 0 || user.Online == false)
                {
                    user.Character = null;
                    await newChannel.SendMessage("***Character Creation Timed out. Please go offline and try again***");
                    user.CharacterCreation = 0;
                    ChannelManager.DeleteChannel(newChannel);
                    return;
                }
                await Task.Delay(2000);
                timeout--;
            }
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));
            await newChannel.SendIsTyping();
            await Task.Delay(4000);

            await newChannel.SendMessage($"\"{proposedName} huh? I like it. Short and sweet.\"\n\n \"Well, {proposedName}, I think I'm starting to get a better idea of who you are.\"");
            
            try
            {
                await server.GetUser(user.DiscordUserID).Edit(nickname: proposedName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+ e.Message);
            }
            await newChannel.SendIsTyping();
            await Task.Delay(5000);

            user.Character = new Character(proposedName, proposedGender);
            
            await newChannel.SendMessage($"\"This is what I'm imagining so far...\"\n\n{user.Character.ToString()}");
            await newChannel.SendIsTyping();
            await Task.Delay(3000);
            await newChannel.SendMessage($"\"Based on this, I'd say youre lacking a little bit in the \'experience\' part.\"\n\n\"At this rate, you wont last very long in {server.Name}.\"\n\n \"Why don't we change that?\"");
            await newChannel.SendIsTyping();
            await Task.Delay(3000);
            await newChannel.SendMessage($"\"You have **6** points to put in Strength, Dexterity and Intelligence. Which would you like?\"\n (Example: Each stat choice seperated by \'/\' - Str/Str/Dex/Int/Dex/Int)");
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));

            int strP = 0;
            int dexP = 0;
            int intP = 0;
            timeout = 300;
            while (user.CharacterCreation == 2)
            {
                buffer = await newChannel.DownloadMessages(1, null, Relative.Before, true);

                if (buffer.Length != 0)
                {
                    if (buffer[0].User.Id == user.DiscordUserID)
                    {
                        if (buffer[0].Timestamp.CompareTo(origTimeStamp) > 0)
                        {
                            string[] choices = buffer[0].Text.Replace("(","").Replace(")","").Replace(" ", "").Split(new char[] { '/', '-', '\\' });

                            strP = 0;
                            dexP = 0;
                            intP = 0;

                            bool successful = true;

                            if (choices.Length == 6)
                            {
                                foreach (string s in choices)
                                {
                                    switch (s.ToLower())
                                    {
                                        case "str":
                                        case "strength":
                                            {
                                                strP++;
                                                break;
                                            }
                                        case "dex":
                                        case "dexterity":
                                            {
                                                dexP++;
                                                break;
                                            }
                                        case "int":
                                        case "intelligence":
                                            {
                                                intP++;
                                                break;
                                            }
                                        default:
                                            {
                                                successful = false;
                                                await newChannel.SendMessage("\"Try again.\"\n\n*Incorrect syntax or misspelling*");
                                                break;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                successful = false;
                                await newChannel.SendMessage("\"Try again.\"\n\n*Incorrect syntax or misspelling*");
                            }
                            
                            if(successful)
                            {
                                user.CharacterCreation++;
                                user.Character.ForceStatUp(CharacterStat.Str, strP);
                                user.Character.ForceStatUp(CharacterStat.Dex, dexP);
                                user.Character.ForceStatUp(CharacterStat.Int, intP);
                            }
                        }
                    }
                }
                if (timeout <= 0 || user.Online == false)
                {
                    user.Character = null;
                    await newChannel.SendMessage("***Character Creation Timed out. Please go offline and try again***");
                    user.CharacterCreation = 0;
                    ChannelManager.DeleteChannel(newChannel);
                    return;
                }
                await Task.Delay(2000);
                timeout--;
            }
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));

            string status = "";
            if(strP > dexP && strP > intP)
            {
                status = "I'd say youd make a great warrior.";
            }
            else if (dexP > strP && dexP > intP)
            {
                status = "You'd probably make for a great archer.";
            }
            else if (intP > strP && intP > dexP)
            {
                status = "You seem like a good mage.";
            }
            else
            {
                status = "You're a pretty balanced fellow.";
            }
            await newChannel.SendIsTyping();
            await Task.Delay(3000);
            await newChannel.SendMessage($"\"Not too shabby. {status} \"");

            await newChannel.SendIsTyping();
            await Task.Delay(3000);
            await newChannel.SendMessage($"\"On top of your stats, you have **5** points to put in Health and Mana. Note that each of these counts as 2 additional points. Which would you like?\"\n (Example: Each stat choice seperated by \'/\' - Health/Mana/Mana/Health/Mana)");
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            

            int hp = 0;
            int mp = 0;
            buffer = null;

            timeout = 300;
            while (user.CharacterCreation == 3)
            {
                buffer = await newChannel.DownloadMessages(1, null, Relative.Before, true);

                if (buffer.Length != 0)
                {
                    if (buffer[0].User.Id == user.DiscordUserID)
                    {
                        if (buffer[0].Timestamp.CompareTo(origTimeStamp) > 0)
                        {
                            string[] choices = buffer[0].Text.Replace("(", "").Replace(")", "").Replace(" ", "").Split(new char[] { '/', '-', '\\' });

                            hp = 0;
                            mp = 0;

                            bool successful = true;

                            if(choices.Length == 5)
                            {
                                foreach (string s in choices)
                                {
                                    switch (s.ToLower())
                                    {
                                        case "hp":
                                        case "health":
                                            {
                                                hp++;
                                                break;
                                            }
                                        case "mp":
                                        case "mana":
                                            {
                                                hp++;
                                                break;
                                            }
                                        default:
                                            {
                                                successful = false;
                                                await newChannel.SendMessage("\"Try again.\"\n\n*Incorrect syntax or misspelling*");
                                                break;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                successful = false;
                                await newChannel.SendMessage("\"Try again.\"\n\n*Incorrect syntax or misspelling*");
                            }
                            
                            if (successful)
                            {
                                user.CharacterCreation++;
                                user.Character.ForceStatUp(CharacterStat.Health, hp);
                                user.Character.ForceStatUp(CharacterStat.Mana, mp);
                            }
                        }
                    }
                }
                if (timeout <= 0 || user.Online == false)
                {
                    user.Character = null;
                    await newChannel.SendMessage("***Character Creation Timed out. Please go offline and try again***");
                    user.CharacterCreation = 0;
                    ChannelManager.DeleteChannel(newChannel);
                    return;
                }
                await Task.Delay(2000);
                timeout--;
            }
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));

            await newChannel.SendIsTyping();
            await Task.Delay(3000);
            await newChannel.SendMessage($"\"Finally, how would you describe yourself? (Example: A dwarf wearing only a loincloth and a single thigh-high.)\"");

            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));

            buffer = null;
            string description = "";
            timeout = 300;
            while (user.CharacterCreation == 4)
            {
                buffer = await newChannel.DownloadMessages(1, null, Relative.Before, true);

                if (buffer.Length != 0)
                {
                    if (buffer[0].User.Id == user.DiscordUserID)
                    {
                        if (buffer[0].Timestamp.CompareTo(origTimeStamp) > 0)
                        {
                            bool successful = true;

                            if (buffer[0].Text.Length > 10)
                            {
                                description = buffer[0].Text.Replace("\"", "").Replace(".","");
                            }
                            else
                            {
                                successful = false;
                                await newChannel.SendMessage("\"Your description is too short.\"");
                            }

                            if (successful)
                            {
                                user.Character.ForceDescriptionChange(description);
                                user.CharacterCreation++;

                            }
                        }
                    }
                }
                if (timeout <= 0 || user.Online == false)
                {
                    user.Character = null;
                    await newChannel.SendMessage("***Character Creation Timed out. Please go offline and try again***");
                    user.CharacterCreation = 0;
                    ChannelManager.DeleteChannel(newChannel);
                    return;
                }
                await Task.Delay(2000);
                timeout--;
            }
            await newChannel.AddPermissionsRule(discordUser, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Deny));

            await newChannel.SendIsTyping();
            await Task.Delay(3000);
            await newChannel.SendMessage($"\"With that, your time in {server.Name} begins! If you have any questions, you can always contact me with /help for more information.\"");


            await newChannel.SendMessage($"Enjoy your time in {server.Name}, {server.GetUser(user.DiscordUserID).NicknameMention}!");

            await Task.Delay(3000);

            ChannelManager.DeleteChannel(newChannel);

            ZoneManager.ChangeZone(user, Vector2.Zero);

            user.CharacterCreation = -1;
            DataManager.WriteJson(user);
        }
        */
    }
}
