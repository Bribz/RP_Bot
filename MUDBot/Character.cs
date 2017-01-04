using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MUDBot
{
    public class Character
    {
        [JsonProperty("Name")]
        public string       Name { get; private set; }

        [JsonProperty("Desc")]
        public string       Desc { get; private set; }

        [JsonProperty("Health")]
        public int          Health { get; private set; }
        [JsonProperty("CurrentHealth")]
        public int          CurrentHealth { get; set; }

        [JsonProperty("Strength")]
        public int          Strength { get; private set; }
        [JsonIgnore]
        public int          CurrentStrength { get; set; }

        [JsonProperty("Dexterity")]
        public int          Dexterity { get; private set; }
        [JsonIgnore]
        public int          CurrentDexterity { get; set; }

        [JsonProperty("Intelligence")]
        public int          Intelligence { get; private set; }
        [JsonIgnore]
        public int          CurrentIntelligence { get; set; }

        [JsonProperty("Mana")]
        public int          Mana { get; private set; }
        [JsonProperty("CurrentMana")]
        public int          CurrentMana { get; set; }

        [JsonProperty("Experience")]
        public int          Experience { get; private set; }

        [JsonProperty("Gender")]
        public Gender       Gender { get; private set; }

        [JsonProperty("Profession")]
        public Profession   Profession { get; private set; }

        [JsonProperty("WorldZone")]
        public int          Zone;
        
        [JsonConstructor]
        public Character(string _name, string _desc, int HP, int cHP, int Str, int Dex, int Int, int MP, int cMP, int Exp, Gender Gen, Profession prof)
        {
            Name = _name;
            Desc = _desc;
            Health = HP;
            CurrentHealth = cHP;
            Strength = Str;
            CurrentStrength = Str;
            Dexterity = Dex;
            CurrentDexterity = Dex;
            Intelligence = Int;
            CurrentIntelligence = Int;
            Mana = MP;
            CurrentMana = cMP;
            Experience = Exp;
            Gender = Gen;
            Profession = prof;
        }

        public Character(string _name, Gender _gender)
        {
            Name = _name;
            Desc = "An ordinary human";
            Health = 15;
            CurrentHealth = 15;
            Strength = 1;
            CurrentStrength = 1;
            Dexterity = 1;
            CurrentDexterity = 1;
            Intelligence = 1;
            CurrentIntelligence = 1;
            Mana = 10;
            CurrentMana = 10;
            Experience = 0;
            Gender = _gender;
            Profession = Profession.None;
        }

        public void ForceDescriptionChange(string desc)
        {
            Desc = desc;
        }

        public void ForceNameChange(string newName)
        {
            Name = newName;
        }

        public void Damage(int value)
        {
            CurrentHealth -= value;
            if (CurrentHealth > Health)
            {
                CurrentHealth = Health;
            }
            else if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
            }
        }

        public int GetStat(CharacterStat s)
        {
            int value = 0;
            switch(s)
            {
                case CharacterStat.Health:
                    {
                        value = CurrentHealth;
                        break;
                    }
                case CharacterStat.Str:
                    {
                        value = CurrentStrength;
                        break;
                    }
                case CharacterStat.Dex:
                    {
                        value = CurrentDexterity;
                        break;
                    }
                case CharacterStat.Int:
                    {
                        value = CurrentIntelligence;
                        break;
                    }
                case CharacterStat.Mana:
                    {
                        value = CurrentMana;
                        break;
                    }
            }
            return value;
        }

        public void GainExp(int value)
        {
            Experience += value;
        }

        public void ChangeProfession(Profession profession)
        {
            Profession = profession;
        }
        
        public void GainAffinity()
        {
            //TODO: Implement this method.
        }

        public void ForceStatUp(CharacterStat stat, int amount = 1)
        {
            switch (stat)
            {
                case CharacterStat.Health:
                    {
                        Health += 2*amount;
                        CurrentHealth += 2 * amount;
                        break;
                    }
                case CharacterStat.Str:
                    {
                        Strength += amount;
                        CurrentStrength += amount;
                        break;
                    }
                case CharacterStat.Dex:
                    {
                        Dexterity += amount;
                        CurrentDexterity += amount;
                        break;
                    }
                case CharacterStat.Int:
                    {
                        Intelligence += amount;
                        CurrentIntelligence += amount;
                        break;
                    }
                case CharacterStat.Mana:
                    {
                        Mana += 2*amount;
                        CurrentMana += 2*amount;
                        break;
                    }
            }
        }

        public bool LevelUp(CharacterStat stat)
        {
            bool retCode = false;
            switch(stat)
            {
                case CharacterStat.Health:
                    {
                        if(Experience > 6)
                        {
                            Experience -= 6;
                            Health += 2;
                            retCode = true;
                        }
                        break;
                    }
                case CharacterStat.Mana:
                    {
                        if (Experience > 6)
                        {
                            Experience -= 6;
                            Mana += 2;
                            retCode = true;
                        }
                        break;
                    }
                case CharacterStat.Str:
                    {
                        if(Experience > (Strength*2))
                        {
                            Experience -= (Strength * 2);
                            Strength++;
                            retCode = true;
                        }
                        break;
                    }
                case CharacterStat.Dex:
                    {
                        if (Experience > (Dexterity * 2))
                        {
                            Experience -= (Dexterity * 2);
                            Dexterity++;
                            retCode = true;
                        }
                        break;
                    }
                case CharacterStat.Int:
                    {
                        if (Experience > (Intelligence * 2))
                        {
                            Experience -= (Intelligence * 2);
                            Intelligence++;
                            retCode = true;
                        }
                        break;
                    }
            }

            return retCode;
        }

        public override string ToString()
        {
            return $"Character Stats: \n\n  Name: {Name}\n  Gender: {Gender}\n  Profession: {Profession}\n\n  Health: {CurrentHealth}/{Health}\n  Strength: {Strength}\n"
            + $"  Dexterity: {Dexterity}\n  Intelligence: {Intelligence}\n  Mana: {CurrentMana}/{Mana}\n\n  Experience: {Experience}";
        }
    }
}
