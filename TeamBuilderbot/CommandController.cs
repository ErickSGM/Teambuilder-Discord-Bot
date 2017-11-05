using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using TeamBuilderbot.Commands;
using TeamBuilderbot.Models;

namespace TeamBuilderbot
{
    public class CommandController : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;
        private readonly CurrentGames _currentGames;
        private readonly SoundPlayer _soundPlayer;

        public CommandController(Settings settings, CurrentGames currentGames)
        {
            _settings = settings;
            _currentGames = currentGames;
            _soundPlayer = new SoundPlayer();
        }
        [Command("clean")]
        [Summary("Cleans all teambuilder channels.")]
        public async Task Clean()
        {
            var channelCleaner = new ChannelCleaner(Context, _settings);
            await ReplyAsync("Cleaning teambuilder channels...");
            await channelCleaner.DeleteTeamBuilderChannels();
        }

        [Command("build")]
        [Summary("Build teams based on the users on the current channel and teamSize.")]
        public async Task BuildTeamAsync([Summary("Team Size")] int teamSize, [Summary("ChannelPrefix")] string gameInput = null)
        {
            var teamBuilder = new TeamBuilder(Context);
            var gameName = gameInput ?? _settings.DefaultGame;
            var (output, game) = await teamBuilder.BuildTeams(teamSize, gameName);
            _currentGames.Games[game.Name] = game;
            await ReplyAsync(output);
        }

        [Command("start", RunMode = RunMode.Async)]
        [Summary("Start Countdown.")]
        public async Task Start([Summary("ChannelPrefix")] string gameInput = null)
        {
            await ReplyAsync("Starting...");
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            var audioClient = await channel.ConnectAsync();
            await _soundPlayer.PlayStartSound(audioClient);

            var gameStarter = new GameStarter(_settings, Context);
            var gameName = gameInput ?? _settings.DefaultGame;
            await gameStarter.Start(_currentGames.Games[gameName]);
        }
    }
}
