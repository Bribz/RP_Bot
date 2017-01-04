using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace MUDBot
{
    public class ChannelManager
    {
        private MUDWorld world;
        private Server Server;
        private List<Channel> Channels;
        private ulong[] BypassedChannels = 
            {
                /*landing_page*/    260889740432113666,
                /*development*/     260853439695683584,
                /*Online*/          260894817335115787,
                /*Offline*/         260895517691740161,
                /*offtopic*/         261403861128839168
            };

        //client.GetServer(260853439695683584);
        public ChannelManager(Server _server, MUDWorld _world)
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
                    Channels[i].Delete();
                }
            }
        }

        public void Initialize()
        {
            Channels = Server.AllChannels.ToList();

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

        public async void LeaveChannel(User user, Channel channel)
        {
            if (user == null || channel == null) return;

            await channel.SendMessage($"A set of magical rings spiral {world.FindGameUser(user.Id).Character.Name} as {GlobalValues.Pronoun(world.FindGameUser(user.Id).Character.Gender)} begins to fade from view.");
            await Task.Delay(4000);
            await channel.RemovePermissionsRule(user);
        }

        public async Task<Channel> CreatePrivateChannel(User user)
        {
            Channel newChannel = await Server.CreateChannel(user.Id.ToString(), ChannelType.Text);
            await newChannel.AddPermissionsRule(Server.EveryoneRole, new ChannelPermissionOverrides(readMessages: PermValue.Deny, readMessageHistory:PermValue.Deny, sendMessages: PermValue.Deny));
            await newChannel.AddPermissionsRule(Server.FindRoles("Moderator", true).FirstOrDefault(), new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow));
            await newChannel.AddPermissionsRule(user, new ChannelPermissionOverrides(readMessages: PermValue.Allow, sendMessages: PermValue.Allow, readMessageHistory: PermValue.Allow));
            await newChannel.AddPermissionsRule(Server.FindRoles("NPC", true).FirstOrDefault(), new ChannelPermissionOverrides(readMessageHistory: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, mentionEveryone: PermValue.Allow, manageNicknames: PermValue.Allow, manageChannel: PermValue.Allow, manageMessages: PermValue.Allow));
            Channels.Add(newChannel);
            return newChannel;
        }

        public async void DeleteChannel(Channel target)
        {
            await target.SendMessage("Channel will now delete...");
            await Task.Delay(8000);
            Channels.Remove(target);
            await target.Delete();
        }

        public async Task ChangeChannel(ulong userID, Vector2 location, Channel currentChannel = null)
        {
            User user = Server.GetUser(userID);

            Channel nChannel = null;

            foreach(var c in Channels)
            {
                if(c.Name.Equals("zone_"+location.ToGridDigit(world.MUDSize).ToString()))
                {
                    nChannel = c;
                    break;
                }
            }

            if(nChannel == null)
            {
                nChannel = await Server.CreateChannel($"zone_{location.ToGridDigit(world.MUDSize).ToString()}", ChannelType.Text);
                await nChannel.AddPermissionsRule(Server.EveryoneRole, new ChannelPermissionOverrides(readMessages:PermValue.Deny, readMessageHistory: PermValue.Deny, sendMessages:PermValue.Deny));
                await nChannel.AddPermissionsRule(Server.FindRoles("NPC", true).FirstOrDefault(), new ChannelPermissionOverrides(readMessageHistory:PermValue.Allow, readMessages:PermValue.Allow, sendMessages:PermValue.Allow, mentionEveryone:PermValue.Allow, manageNicknames: PermValue.Allow, manageChannel:PermValue.Allow, manageMessages:PermValue.Allow));
                await nChannel.AddPermissionsRule(Server.FindRoles("Moderator", true).FirstOrDefault(), new ChannelPermissionOverrides(readMessageHistory: PermValue.Allow, readMessages: PermValue.Allow, sendMessages: PermValue.Allow, changeNickname: PermValue.Allow, manageMessages: PermValue.Allow, manageNicknames: PermValue.Allow, mentionEveryone:PermValue.Allow));
                Console.WriteLine("All permissions " + nChannel.PermissionOverwrites.ToString());
                world.ZoneManager.SetZoneChannel(location, nChannel);
                Channels.Add(nChannel);
            }
            var nPermAllow = new ChannelPermissionOverrides(readMessages: PermValue.Allow, readMessageHistory: PermValue.Allow, sendMessages: PermValue.Allow);
            await nChannel.AddPermissionsRule(user, nPermAllow);

            if(currentChannel != null && !BypassedChannels.Contains(currentChannel.Id))
            {
                await (currentChannel.RemovePermissionsRule(user));
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
                    DeleteChannel(currentChannel);
                }
            }
        }
    }
}
