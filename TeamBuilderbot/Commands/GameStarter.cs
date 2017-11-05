using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TeamBuilderbot.Models;
using Game = TeamBuilderbot.Models.Game;

namespace TeamBuilderbot.Commands
{
    public class GameStarter
    {
        private readonly ChannelCleaner _channelCleaner;
        private readonly Settings _settings;
        private readonly SocketCommandContext _context;

        public GameStarter(ChannelCleaner channelCleaner, Settings settings, SocketCommandContext context)
        {
            _channelCleaner = channelCleaner;
            _settings = settings;
            _context = context;
        }

        public async Task Start(Game game)
        {
            await _channelCleaner.DeleteTeamBuilderChannels();
            var tasks = game.Teams.Select(async (team, i) =>
            {
                var channel =
                    await _context.Guild.CreateVoiceChannelAsync($"{_settings.TeamBuilderChannelPrefix}-{game.Name}-team-{i+1}");
                var moveUserTasks = team.Players.Select(async player =>
                {
                    var guildUser = player as IGuildUser;
                    await guildUser.ModifyAsync(
                        userProperties => userProperties.ChannelId = channel.Id
                    );
                });
                await Task.WhenAll(moveUserTasks);
            });
            await Task.WhenAll(tasks);
        }
    }
}