using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TeamBuilderbot
{
    public class TeamBuilder
    {
        private readonly SocketCommandContext _context;
        private readonly ChannelCleaner _channelCleaner;
        private readonly Settings _settings;

        public TeamBuilder(SocketCommandContext context, ChannelCleaner channelCleaner, Settings settings)
        {
            _context = context;
            _channelCleaner = channelCleaner;
            _settings = settings;
        }

        public async Task<string> BuildTeams(short teamSize, string channelPrefix)
        {
            var currentChannel = _context.Message.Channel as SocketChannel;
            var users =
                currentChannel
                    .Users
                    .Where(x => !x.IsBot && x.Status == UserStatus.Online)
                    .ToList();
            var teams = CalculateTeams(teamSize, users);
            var output = teams.Select(BuildTeamText);
            BuildChannelsAndMoveUsers(channelPrefix, teams);
            return $"-- \n{string.Join("\n", output)}";
        }

        private async Task BuildChannelsAndMoveUsers(string channelPrefix, List<List<SocketUser>> teams)
        {
            await _channelCleaner.DeleteTeamBuilderChannels();
            var tasks = teams.Select(async (team, i) =>
            {
                var channel =
                    await _context.Guild.CreateVoiceChannelAsync($"{_settings.TeamBuilderChannelPrefix}-{channelPrefix}-team-{i}");
                var moveUserTasks = team.Select(async user =>
                {
                    var guildUser = user as IGuildUser;
                    await guildUser.ModifyAsync(
                        userProperties => userProperties.ChannelId = channel.Id
                    );
                });
                await Task.WhenAll(moveUserTasks);
            });
            await Task.WhenAll(tasks);
        }

        private static List<List<SocketUser>> CalculateTeams(short teamSize, List<SocketUser> users)
        {
            var totalTeams = Math.Ceiling((double) users.Count / teamSize);
            var random = new Random();
            var teams = new List<List<SocketUser>>();
            for (int teamNumber = 0; teamNumber < totalTeams; teamNumber++)
            {
                var team = new List<SocketUser>();
                for (int teamSlot = 0; teamSlot < teamSize; teamSlot++)
                {
                    if (users.Count == 0) break;
                    var selectedUser = random.Next(users.Count);
                    team.Add(users[selectedUser]);
                    users.RemoveAt(selectedUser);
                }
                teams.Add(team);
            }
            return teams;
        }

        private static string BuildTeamText(List<SocketUser> team, int i)
        {
            return $"Team {i + 1}: \n \t {string.Join("\n \t", BuildUsersText(team))}";
        }

        private static IEnumerable<string> BuildUsersText(List<SocketUser> team)
        {
            return team.Select(teamMember => $"{teamMember.Username}");
        }
    }
}