using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TeamBuilderbot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;

        public Commands(Settings settings)
        {
            _settings = settings;
        }
        [Command("clean")]
        [Summary("Cleans all teambuilder channels.")]
        public async Task Clean()
        {
            var channelCleaner = new ChannelCleaner(Context, _settings);
            await channelCleaner.DeleteTeamBuilderChannels();
        }

        [Command("build")]
        [Summary("Build teams based on the users on the current channel and teamSize.")]
        public async Task BuildTeamAsync([Summary("Team Size")] short teamSize, [Summary("ChannelPrefix")] string channelPrefix = "d")
        {
            var channelCleaner = new ChannelCleaner(Context, _settings);
            var teamBuilder = new TeamBuilder(Context, channelCleaner, _settings);
            await ReplyAsync(await teamBuilder.BuildTeams(teamSize, channelPrefix));
        }
    }
}
