using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MUDBot
{
    public class MUDObject
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("ObjectID")]
        public int ObjTypeID;
        [JsonProperty("Health")]
        public int Health;
        [JsonProperty("Desc")]
        public string[] Description;
        [JsonProperty("AtkDesc")]
        public string[] AttackDescription;
        [JsonProperty("DestroyDesc")]
        public string[] DestroyDescription;
        [JsonProperty("InteractDmg")]
        public int InteractDamage;
        [JsonProperty("InteractStat")]
        public CharacterStat InteractStat;
        [JsonProperty("Cooldown")]
        private int cooldown;
        //Affinity effect;
        [JsonIgnore]
        public List<InteractUserData> interactData;
        //List<Item> itemDrops;

        public MUDObject()
        {
            Health = 0;
            InteractDamage = 0;
            cooldown = 0;
        }

        [JsonConstructor]
        public MUDObject(string _name, int _objTypeID, int _health, string[] _description, string[] _attackDescription, string[] _destroyDescription, int _interactDamage, CharacterStat _interactStat, int _cooldown)
        {
            Name = _name;
            ObjTypeID = _objTypeID;
            Health = _health;
            Description = _description;
            AttackDescription = _attackDescription;
            DestroyDescription = _destroyDescription;
            InteractDamage = _interactDamage;
            InteractStat = _interactStat;
            cooldown = _cooldown;
        }

        public async void Interact(GameUser user, Channel channel)
        {
            InteractUserData timestamp = interactData.Find(x => x.UserDiscordID == user.DiscordUserID);
            if (timestamp != null)
            {
                if((DateTime.Now - timestamp.Timestamp).Seconds <= cooldown )
                {
                    await channel.SendMessage("It is too soon to do that again!");
                    return;
                }
                else
                {
                    interactData.Remove(timestamp);
                }
            }
            Random r = new Random();
            int it = r.Next(Description.Length);

            await channel.SendMessage(Description[it]);

            if(InteractDamage > 0)
            {
                await channel.SendMessage($"You take {InteractDamage} damage.");
                user.Character.Damage(InteractDamage);
            }
            else if(InteractDamage < 0)
            {
                await channel.SendMessage($"You heal {InteractDamage} health.");
                user.Character.Damage(InteractDamage);
            }

            interactData.Add(new InteractUserData(user.DiscordUserID, DateTime.Now));
            await Task.Delay(0);
        }

        public async void Attack(GameUser user, Channel channel)
        {
            InteractUserData timestamp = interactData.Find(x => x.UserDiscordID == user.DiscordUserID);
            if (timestamp != null)
            {
                if ((DateTime.Now - timestamp.Timestamp).Seconds <= cooldown)
                {
                    await channel.SendMessage("It is too soon to do that again!");
                    return;
                }
                else
                {
                    interactData.Remove(timestamp);
                }
            }

            //Attack object
            Random r = new Random();
            int damage = r.Next(1, 10) + user.Character.GetStat(InteractStat);
            Health -= damage;

            int it = r.Next(Description.Length);
            string descriptor = AttackDescription[it];

            if(damage < 3)
            {
                descriptor += "\nYou feel pretty unsuccessful in your attempt.\n";
            }
            else if(damage < 5)
            {
                descriptor += "\nYou think you did a decent job in your attempt.\n";
            }
            else if(damage < 8)
            {
                descriptor += "\nYou did a fairly good job in your attempt.\n";
            }
            else
            {
                descriptor += "\nYou did a really great job in your attempt.\n";
            }

            if(Health > 0)
            {
                descriptor += "But it still stands.";
            }
            else
            {
                it = r.Next(DestroyDescription.Length);
                descriptor += DestroyDescription[it];
            }

            await channel.SendMessage(descriptor);

            interactData.Add(new InteractUserData(user.DiscordUserID, DateTime.Now));
        }
    }

    
    public class InteractUserData
    {
        [JsonProperty("UserID")]
        public ulong UserDiscordID;
        [JsonProperty("Timestamp")]
        public DateTime Timestamp;
        
        public InteractUserData(ulong discordID, DateTime timeStamp)
        {
            UserDiscordID = discordID;
            Timestamp = timeStamp;
        }
    }
}
