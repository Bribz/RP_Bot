using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MUDBot
{
    public class ZoneManager
    {
        private SocketGuild server;
        private MUDWorld world;
        private List<Zone> Zones;
        private int MUDSize;

        public ZoneManager(SocketGuild _server, MUDWorld _world)
        {
            server = _server;
            world = _world;
            MUDSize = world.MUDSize;
            Zones = DataManager.ReadAllZones(MUDSize);
        }

        public void ShutDown()
        {
            foreach (var zone in Zones)
            {
                DataManager.WriteJson(zone);
            }
        }

        public Zone GetZone(int zoneID)
        {
            return Zones[zoneID];
        }

        public Zone GetZone(Vector2 zoneID)
        {
            return Zones[zoneID.ToGridDigit(MUDSize)];
        }

        public void SetZoneChannel(Vector2 zoneID, IGuildChannel channel)
        {
            Zones[zoneID.ToGridDigit(MUDSize)].SetChannel(channel); 
        }

        public void SetZoneChannel(int zoneID, IGuildChannel channel)
        {
            Zones[zoneID].SetChannel(channel);
        }

        public void RemoveZoneChannel(Vector2 zoneID)
        {
            Zones[zoneID.ToGridDigit(MUDSize)].RemoveChannel();
        }

        public void RemoveZoneChannel(int zoneID)
        {
            Zones[zoneID].RemoveChannel();
        }

        public async void ChangeZone(GameUser user, Vector2 zoneID, Zone previousZone = null, string descriptorPrefix = null, bool verbose = true)
        {
            if(previousZone != null)
            {
                previousZone.RemovePlayer(user);
            }
            
            user.Character.Zone = zoneID.ToGridDigit(world.MUDSize);

            await world.ChannelManager.ChangeChannel(user.DiscordUserID, zoneID);
            Zones[zoneID.ToGridDigit(MUDSize)].AddPlayer(user);
            if(descriptorPrefix == null)
            {
                if(verbose)
                {
                    await (Zones[zoneID.ToGridDigit(MUDSize)].ZoneChannel as SocketTextChannel).SendMessageAsync($"{server.GetUser(user.DiscordUserID).Mention},\n\n *your traveling brings you upon {Zones[zoneID.ToGridDigit(MUDSize)].Description}.*");
                }
                else
                {
                    await (Zones[zoneID.ToGridDigit(MUDSize)].ZoneChannel as SocketTextChannel).SendMessageAsync($"{server.GetUser(user.DiscordUserID).Mention} accompanies you.");
                }
            }
            else
            {
                if(verbose)
                {
                    await (Zones[zoneID.ToGridDigit(MUDSize)].ZoneChannel as SocketTextChannel).SendMessageAsync($"{server.GetUser(user.DiscordUserID).Mention},\n\n *{descriptorPrefix} {Zones[zoneID.ToGridDigit(MUDSize)].Description}.*");
                }
                else
                {
                    await (Zones[zoneID.ToGridDigit(MUDSize)].ZoneChannel as SocketTextChannel).SendMessageAsync($"{server.GetUser(user.DiscordUserID).Mention} accompanies you.");
                }
            }
        }

        public async void ChangeZone(GameUser user, int zoneID, Zone previousZone = null, string descriptorPrefix = null)
        {
            if (previousZone != null)
            {
                previousZone.RemovePlayer(user);
            }

            user.Character.Zone = zoneID;

            await world.ChannelManager.ChangeChannel(user.DiscordUserID, new Vector2(zoneID%MUDSize, zoneID/MUDSize));
            Zones[zoneID].AddPlayer(user);
            if (descriptorPrefix == null)
            {
                await (Zones[zoneID].ZoneChannel as SocketTextChannel).SendMessageAsync($"{server.GetUser(user.DiscordUserID).Mention},\n\n *your traveling brings you upon {Zones[zoneID].Description}.*");
            }
            else
            {
                await (Zones[zoneID].ZoneChannel as SocketTextChannel).SendMessageAsync($"{server.GetUser(user.DiscordUserID).Mention},\n\n *{descriptorPrefix} {Zones[zoneID].Description}.*");
            }
        }
    }
}
