using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Game = TeamBuilderbot.Models.Game;

namespace TeamBuilderbot.Commands
{
    public class GameStarter
    {
        private readonly Settings _settings;
        private readonly SocketCommandContext _context;

        public GameStarter(Settings settings, SocketCommandContext context)
        {
            _settings = settings;
            _context = context;
        }

        public async Task Start(Game game)
        {
            var tasks = game.Teams.Select(async (team, i) =>
            {
                var channel = await CreateOrGetChannel(game, i);
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

        private async Task<IVoiceChannel> CreateOrGetChannel(Game game, int index)
        {
            var channelName = $"{_settings.TeamBuilderChannelPrefix}-{game.Name}-team-{index + 1}";
            var existingChannel = _context.Guild.VoiceChannels.SingleOrDefault(x => x.Name == channelName);
            return 
                existingChannel ?? 
                (IVoiceChannel)await _context.Guild.CreateVoiceChannelAsync($"{_settings.TeamBuilderChannelPrefix}-{game.Name}-team-{index + 1}");
        }
    }
}