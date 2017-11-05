using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace TeamBuilderbot.Commands
{
    public class ChannelCleaner
    {
        private readonly SocketCommandContext _context;
        private readonly Settings _settings;

        public ChannelCleaner(SocketCommandContext context, Settings settings)
        {
            _context = context;
            _settings = settings;
        }

        public async Task DeleteTeamBuilderChannels()
        {
            var teamBuilderChannels = _context.Guild.Channels.Where(x =>
                x.Name != null && x.Name.StartsWith($"{_settings.TeamBuilderChannelPrefix}")
            ).ToList();
            var users = teamBuilderChannels.SelectMany(x => x.Users);
            var moveTasks = users.Select(user =>
            {
                if (user.VoiceChannel.Id != _settings.RedirectChannel)
                    return user.ModifyAsync(userProps => userProps.ChannelId = _settings.RedirectChannel);
                return Task.CompletedTask;
            });
            await Task.WhenAll(moveTasks);
            var deleteTasks = teamBuilderChannels.Select(x => x.DeleteAsync()).ToArray();
            await Task.WhenAll(deleteTasks);
        }
    }
}