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

    public enum CharacterStat
    {
        Health,
        Mana,
        Str,
        Dex,
        Int
    }

    [FlagsAttribute]
    public enum CharSheetType : int
    {
        /// <summary>
        /// Display Name, Gender
        /// </summary>
        CharInfo                = 1,      //0000 0001
        /// <summary>
        /// Display Profession, Affinity, Description
        /// </summary>
        CharInfoVerbose         = 2,      //0000 0010
        /// <summary>
        /// Display Base Stats (HP, STR, DEX, INT, MP)
        /// </summary>
        Stats                   = 4,      //0000 0100
        /// <summary>
        /// Display Current Stats
        /// </summary>
        StatsVerbose            = 8,      //0000 1000
        /// <summary>
        /// Display Experience
        /// </summary>
        Experience              = 128,     //1000 0000

        /// <summary>
        /// Display All information
        /// </summary>
        Complete                = 255     //1111 1111
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

    #endregion
}
