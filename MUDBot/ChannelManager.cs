using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Rest;

namespace MUDBot
{
    public class ChannelManager
    {
        private MUDWorld world;
        private SocketGuild Server;
        private List<IGuildChannel> Channels;
        private ulong[] BypassedChannels = 
            {
                /*landing_page*/    260889740432113666,
                /*development*/     260853439695683584,
                /*Online*/          260894817335115787,
                /*Offline*/         260895517691740161,
                /*offtopic*/         261403861128839168
            };

        //client.GetServer(260853439695683584);
        public ChannelManager(SocketGuild _server, MUDWorld _world)
        {
            Server = _server;
            world = _world;
            Initialize();
        }

        public void Shutdown()
        {
            for(int i = 0; i < Channels.Count; i++)
            {
                if(!BypassedChannels.Contains(Channels[i].Id))
                {
                    Channels[i].DeleteAsync();
                }
            }
        }

        public void Initialize()
        {
            List<IGuildChannel> tmp = new List<IGuildChannel>();

            foreach (var ch in Server.Channels)
            {
                tmp.Add(ch as IGuildChannel);
            }

            Channels = tmp;

            for (int i = 0; i < Channels.Count; i++)
            {
                if (BypassedChannels.Contains(Channels[i].Id))
                {
                    Channels.RemoveAt(i);
                }
                else
                {
                    if(Channels[i].Name.Contains("zone_"))
                    {
                        int zoneID = -1;
                        Int32.TryParse(Channels[i].Name.Remove(0, 5), out zoneID);
                        world.ZoneManager.SetZoneChannel(zoneID, Channels[i]);
                    }
                }
            }

            //this should be zero. if not, figure out what to do later.
        }

        public async void LeaveChannel(SocketGuildUser user, SocketTextChannel channel)
        {
            if (user == null || channel == null) return;

            await channel.SendMessageAsync($"A set of magical rings spiral {world.FindGameUser(user.Id).Character.Name} as {GlobalValues.Pronoun(world.FindGameUser(user.Id).Character.Gender)} begins to fade from view.");
            await Task.Delay(4000);
            await channel.RemovePermissionOverwriteAsync(user);
        }

        public async Task<IGuildChannel> CreatePrivateChannel(SocketGuildUser user)
        {
            IGuildChannel newChannel = await Server.CreateTextChannelAsync(user.Id.ToString());

            await newChannel.AddPermissionOverwriteAsync(Server.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny, readMessageHistory: PermValue.Deny, sendMessages: PermValue.Deny));
            await newChannel.AddPermissionOverwriteAsync(Server.Roles.FirstOrDefault(x=>x.Name.Equals("Moderator")), new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            await newChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow, readMessageHistory: PermValue.Allow));
            await newChannel.AddPermissionOverwriteAsync(Server.Roles.FirstOrDefault(x => x.Name.Equals("NPC")), new OverwritePermissions(readMessageHistory: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, manageChannel: PermValue.Allow, manageMessages: PermValue.Allow));
            
            Channels.Add(newChannel as IGuildChannel);
            return newChannel;
        }

        public async void DeleteChannel(SocketTextChannel target)
        {
            await target.SendMessageAsync("Channel will now delete...");
            await Task.Delay(8000);
            Channels.Remove(target);
            await target.DeleteAsync();
        }

        public async Task ChangeChannel(ulong userID, Vector2 location, SocketGuildChannel currentChannel = null)
        {
            SocketGuildUser user = Server.GetUser(userID);

            RestGuildChannel nChannel = null;

            foreach(var c in Channels)
            {
                if(c.Name.Equals("zone_"+location.ToGridDigit(world.MUDSize).ToString()))
                {
                    nChannel = c as RestTextChannel;
                    break;
                }
            }

            if(nChannel == null)
            {
                nChannel = await Server.CreateTextChannelAsync($"zone_{location.ToGridDigit(world.MUDSize).ToString()}");
                await nChannel.AddPermissionOverwriteAsync(Server.EveryoneRole, new OverwritePermissions(readMessages:PermValue.Deny, readMessageHistory: PermValue.Deny, sendMessages:PermValue.Deny));
                await nChannel.AddPermissionOverwriteAsync(Server.Roles.FirstOrDefault(x=>x.Name.Equals("NPC")), new OverwritePermissions(readMessageHistory:PermValue.Allow, readMessages:PermValue.Allow, sendMessages:PermValue.Allow, mentionEveryone:PermValue.Allow, manageChannel:PermValue.Allow, manageMessages:PermValue.Allow));
                await nChannel.AddPermissionOverwriteAsync(Server.Roles.FirstOrDefault(x=>x.Name.Equals("Moderator")), new OverwritePermissions(readMessageHistory: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, manageMessages: PermValue.Allow, mentionEveryone:PermValue.Allow));
                //Console.WriteLine("All permissions " + nChannel.PermissionOverwrites.ToString());
                world.ZoneManager.SetZoneChannel(location, nChannel);
                Channels.Add(nChannel);
            }
            var nPermAllow = new OverwritePermissions(readMessages: PermValue.Allow, readMessageHistory: PermValue.Allow, sendMessages: PermValue.Allow);
            await nChannel.AddPermissionOverwriteAsync(user, nPermAllow);

            if(currentChannel != null && !BypassedChannels.Contains(currentChannel.Id))
            {
                await (currentChannel.RemovePermissionOverwriteAsync(user));
                if (currentChannel.Users.Count() == 0)
                {
                    if (currentChannel.Name != user.Id.ToString())
                    {
                        int zoneID;
                        if(Int32.TryParse(currentChannel.Name.Remove(0, 5), out zoneID))
                        {
                            world.ZoneManager.RemoveZoneChannel(zoneID);
                        }
                    }
                    DeleteChannel(currentChannel as SocketTextChannel);
                }
            }
        }
    }
}
