using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DiscordBotProject.APICalls;
using DSharpPlus.Entities;

namespace DiscordBotProject.Commands
{
    class WeatherCommand : BaseCommandModule
    {
        public WeatherAPI weatherAPI = new WeatherAPI();

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {   
            await ctx.Channel.SendMessageAsync("Roundtrip to host: " + ctx.Client.Ping.ToString() + " ms").ConfigureAwait(false);
           
        }

        [Command("weather")]
        public async Task Weather(CommandContext ctx)
        {
            string forecast = weatherAPI.GetBucaramangaForecast();

            await ctx.Channel.SendMessageAsync(forecast).ConfigureAwait(false);
        }


        //Lo que tenía antes de leer en la documentación que Client tiene un atributo Ping xd
        public static string PingHost(string nameOrAddress)
        {
            var pingable = String.Empty;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                if (reply.Status == IPStatus.Success)
                {
                    pingable = "Roundtrip to host: '" + nameOrAddress + "' = " +  reply.RoundtripTime.ToString() + " ms";
                }
            }
            catch (PingException)
            {
                
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

    }
}
