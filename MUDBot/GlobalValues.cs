using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUDBot
{
    public class GlobalValues
    {
        public static string Pronoun(Gender g)
        {
            switch (g)
            {
                case Gender.Female:
                    {
                        return "she";
                    }
                case Gender.Male:
                    {
                        return "he";
                    }
                case Gender.Genderless:
                    {
                        return "they";
                    }
                default:
                    {
                        return "they";
                    }
            }
        }
    }

    #region Enumerated Values
    public enum Direction
    {
        None,
        North,
        East,
        South,
        West
    }

    public enum Gender
    {
        Female                  = 0x0,
        Male                    = 0x1,
        Genderless              = 0x2
    }

    public enum Profession
    {
        Miner                   = 0x0,
        Blacksmith              = 0x1,
        Lumberjack              = 0x2,
        Fletcher                = 0x3,
        Tanner                  = 0x4,
        Clothier                = 0x5,
        None                    = 0xF
    }

    public enum CharacterStat
    {
        Health,
        Mana,
        Str,
        Dex,
        Int
    }

    #endregion
}
