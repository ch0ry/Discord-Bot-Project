using DiscordBotProject.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotProject
{
    class Bot
    {
        private DiscordEntities ContextDB { get; set; }
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            ContextDB = new DiscordEntities();

            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug  
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;


            Client.UseInteractivity(new InteractivityConfiguration
            {

            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                CaseSensitive = false,
                DmHelp = true,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<WeatherCommand>();

            Client.MessageCreated += async (s, e) =>
            {
                var id = e.Author.Id.ToString();
                var result = ContextDB.users.Where(u => u.discordId == id).FirstOrDefault();
                Console.WriteLine(id);
                if (result != null)
                {
                    result.msgnum += 1;
                    if (result.msgnum % 10 == 0)
                    {
                        result.lvl += 1;
                        e.Message.RespondAsync("Level Up!\n Your level is now " + result.lvl);
                    }
                }
                ContextDB.SaveChanges();

            };

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            var servers = Client.Guilds.Values;

            foreach (DiscordGuild server in servers)
            {
                foreach(DiscordMember member in server.GetAllMembersAsync().Result)
                {
                    try
                    {
                        Console.WriteLine(member.Id);
                        var id = member.Id.ToString();
                        if (!member.IsBot && ContextDB.users.Where(u => u.discordId == id).FirstOrDefault() == null)
                        {
                            var userdata = new userData()
                            {
                                Id = Guid.NewGuid(),
                                discordId = member.Id.ToString(),
                                username = member.Username,
                                lvl = 0,
                                msgnum = 0
                            };
                            ContextDB.users.Add(userdata);
                        }
                        ContextDB.SaveChanges();

                    }
                    catch (DbEntityValidationException de)
                    {
                        foreach (var eve in de.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    

                }
            }

            
            return Task.CompletedTask;
        }
    }
}
