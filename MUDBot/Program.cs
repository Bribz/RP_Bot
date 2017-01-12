using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace MUDBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");
            /*
            
            GameUser g = new GameUser();
            g.DiscordUserID = 124910128582361092;
            g.CharacterCreation = 0;
            g.Character = null; // new Character("Potato", 1, 2, 3, 4, 1, 0, Gender.Genderless, Profession.Fletcher);

            DataManager.WriteJson(g);
            g = null;
            //DataManager.ReadJson(126538520193007616,out g);

            //Console.WriteLine(g.Character.ToString());

            g = new GameUser();
            g.DiscordUserID = 185546547482722305;
            g.CharacterCreation = 0;
            g.Character = null; // new Character("Potato", 1, 2, 3, 4, 1, 0, Gender.Genderless, Profession.Fletcher);
            
            DataManager.WriteJson(g);
            g = null;
            Console.ReadKey();
            */

            /*
            DataManager.WriteJson(new MUDObject("Rock", 0, 20, 
                new string[] { "An ordinary rock.", "Nothing special about it.", "A pretty normal looking rock.", "It's still just a rock." }, 
                new string[] { "You swing at the rock.", "You hack at the rock.", "You hit the rock." },
                new string[] { "It smashes to pieces.", "It breaks into bits.", "It shatters into fragments." },
                0, CharacterStat.Str, 10));
            */

            Console.Title = "Mortal World : RP Bot";

            Bot bot = Bot.instance;
            /*
            while (bot.running)
            {
                Task.Delay(3000);
            }

            bot = null;
            */
            Console.ReadKey();
        }
    }
}
