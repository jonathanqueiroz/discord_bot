using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;

        public static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }

        public Bot()
        {
            _client = new DiscordSocketClient();
            _interactionService = new InteractionService(_client);
            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.InteractionCreated += HandleInteractionAsync;
        }

        public async Task RunAsync()
        {
            var token = "token";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1); // Mantém o bot rodando
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return; // Ignora mensagens de outros bots

            if (message.Channel is IDMChannel) // Verifica se a mensagem é uma DM
            {
                Console.WriteLine($"Mensagem recebida do Usuário: ({message.Author.Id}) {message.Author.Username}");
                Console.WriteLine($"Mensagem: {message.Content}");

                if (message.Content == "i")
                {
                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "utils", "text.txt");
                    using (var stream = new FileStream(imagePath, FileMode.Open))
                    await message.Channel.SendFileAsync(stream, "text.txt", "Aqui está seu arquivo!");
                }
                else if (message.Content == "b")
                {
                    var builder = new ComponentBuilder()
                        .WithButton("Report", "report", ButtonStyle.Primary)
                        .WithButton("Gráfico", "graph", ButtonStyle.Primary)
                        .WithButton("Texto", "text", ButtonStyle.Primary);

                    await message.Channel.SendMessageAsync("Aqui está um botão:", components: builder.Build());
                }
                else
                {
                    await message.Channel.SendMessageAsync($"Sua mensagem é: {message.Content}");
                }
            }
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            if (interaction is SocketMessageComponent component)
            {
                if (component.Data.CustomId == "report")
                {
                    await component.RespondAsync("Buscando seu report...");
                }
                else if (component.Data.CustomId == "graph")
                {
                    await component.RespondAsync("Buscando seu gráfico...");
                }
                else
                {
                    await component.RespondAsync("Buscando seu texto...");
                }
            }
        }
    }
}