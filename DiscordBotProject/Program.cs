using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotProject
{
    class Program
    {
        static void Main(string[] args)
        {

                
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
