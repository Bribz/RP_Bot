
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Addons.InteractiveCommands;
using Discord.WebSocket;

namespace MUDBot.Commands
{
    public class IAgreeCommand : InteractiveModuleBase
    {
        [Command("IAgree"), Summary("Agree to terms and conditions, handle character creation")]
        public async Task IAgree()
        {
            string proposedName = "";
            Gender proposedGender = Gender.Female;

            var embed = MUDWorld.BuildNpcEmbed(null, "NPC");

            GameUser user = Bot.instance.GameWorld.FindGameUser(Context.User.Id);
            user.CharacterCreation = 0;

            await Context.Channel.SendMessageAsync($"**Welcome to {Bot.instance._server.Name}.**\n\n Many adventures and mishaps await you in the world, as you will soon discover.\n\nFor all rules, see {Bot.instance._server.GetChannel(260853439695683584)} for details.", false, embed);

            embed.Footer = new EmbedFooterBuilder() { Text = "\n*Choices:* ***Boy***, ***Girl***, ***Neither***" };
            
            await Context.Channel.SendMessageAsync($"\"For you, {Context.User.Mention}, humble beginnings await your character.\"\n\"But first and foremost, are you a Boy, Girl, or Neither?\"", false, embed);
            //await ((Context.Channel) as SocketGuildChannel).AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));

            TimeSpan Timeout = TimeSpan.FromMinutes(5d);

            IUserMessage response;
            while (user.CharacterCreation < 1)
            {
                try
                {
                    response = await WaitForMessage(Context.Message.Author, Context.Channel, Timeout);

                    if (response.Content.ToLower().Equals("boy"))
                    {
                        proposedGender = Gender.Male;
                        await Context.Channel.SendMessageAsync($"\"You look rather handsome for a {proposedGender}.\n\n However, what should we call you?\"");
                        user.CharacterCreation++;
                        break;
                    }
                    else if (response.Content.ToLower().Equals("girl"))
                    {
                        proposedGender = Gender.Female;
                        await Context.Channel.SendMessageAsync($"\"You look rather gorgeous for a {proposedGender}.\n\n However, what should we call you?\"");
                        user.CharacterCreation++;
                        break;
                    }
                    else if (response.Content.ToLower().Equals("neither"))
                    {
                        proposedGender = Gender.Genderless;
                        await Context.Channel.SendMessageAsync($"\"You look rather dashing for a {proposedGender}.\n\n However, what should we call you?\"");
                        user.CharacterCreation++;
                        break;
                    }
                    else
                    {
                        embed.Footer = new EmbedFooterBuilder() { Text = "(check spelling)" };
                        await Context.Channel.SendMessageAsync("\"I didn't quite catch that.\"", false, embed);
                    }
                }
                catch(Exception e)
                {
                    embed.Footer = null;
                    user.Character = null;
                    await Context.Channel.SendMessageAsync("Character Creation has been cancelled. Please login once more to restart.", false, embed);
                    Bot.instance.GameWorld.ChannelManager.DeleteChannel(Context.Channel as SocketTextChannel);
                }
                finally
                {
                    user.CharacterCreation = 0;
                }
                
            }

            while (user.CharacterCreation < 2)
            {
                try
                {
                    response = await WaitForMessage(Context.Message.Author, Context.Channel, Timeout);

                    proposedName = response.Content;
                    if (proposedName.Length < 15)
                    {
                        user.CharacterCreation++;
                        await Context.Channel.SendMessageAsync($"\"{proposedName} huh? I like it. Short and sweet.\"\n\n \"Well, {proposedName}, I think I'm starting to get a better idea of who you are.\"");
                        break;
                    }
                    else
                    {
                        embed.Footer = new EmbedFooterBuilder() { Text = "(Name was too long)" };
                        await Context.Channel.SendMessageAsync("\"Thats a little long. Maybe a shorter name?\"", false, embed);
                    }
                }
                catch (Exception e)
                {
                    embed.Footer = null;
                    user.Character = null;
                    await Context.Channel.SendMessageAsync("Character Creation has been cancelled. Please login once more to restart.", false, embed);
                    Bot.instance.GameWorld.ChannelManager.DeleteChannel(Context.Channel as SocketTextChannel);
                }
                finally
                {
                    user.CharacterCreation = 0;
                }
            }

            await Bot.instance.ChangeNickname(Context.User, proposedName);

            user.Character = new Character(proposedName, proposedGender);

            await Context.Channel.SendMessageAsync("\"This is what I'm imagining so far...\"\n\n");
            
            embed = MUDWorld.BuildCharacterEmbed();
            string message = MUDWorld.BuildCharacterString(user.Character, CharSheetType.CharInfo | CharSheetType.Stats);
            await Context.Channel.SendMessageAsync(message, false, embed);

            embed = MUDWorld.BuildNpcEmbed(null, "NPC");
            await Context.Channel.SendMessageAsync($"\"Based on this, I'd say youre lacking a little bit in the \'experience\' part.\"\n\n\"At this rate, you wont last very long in {Bot.instance._server.Name}.\"\n\n \"Why don't we change that?\"", false, embed);

            embed.Footer = new EmbedFooterBuilder() {Text = "Type 6 stat names seperated by \'/\' - Example: Str/Str/Dex/Int/Dex/Int)" };
            await Context.Channel.SendMessageAsync($"\"You have **6** points to put in Strength, Dexterity and Intelligence. Which would you like?\"\n", false, embed);
        }
    }
}
