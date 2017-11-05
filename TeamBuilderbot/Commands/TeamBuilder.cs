using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TeamBuilderbot.Models;
using Game = TeamBuilderbot.Models.Game;

namespace TeamBuilderbot.Commands
{
    public class TeamBuilder
    {
        private readonly SocketCommandContext _context;

        public TeamBuilder(SocketCommandContext context)
        {
            _context = context;
        }

        public async Task<(string, Game)> BuildTeams(int teamSize, string gameName)
        {
            var invokedCommandChannel = (_context.User as SocketGuildUser)?.VoiceChannel;
            var users =
                invokedCommandChannel
                    .Users
                    .Where(user => 
                        !user.IsBot && 
                        !user.IsSelfMuted && 
                        user.VoiceChannel.Name == invokedCommandChannel.Name )
                    .ToList();
            var teams = CalculateTeams(teamSize, users);
            var output = teams.Select(BuildTeamText);
            var game = new Game(gameName, teams);
            return ($"-- \n{string.Join("\n", output)}", game);
        }

        private static List<Team> CalculateTeams(int teamSize, List<SocketGuildUser> users)
        {
            var totalUsers = users.Count;
            var totalTeams = Math.Ceiling((double) totalUsers / teamSize);
            var random = new Random();
            var teams = new List<Team>();
            for (int teamNumber = 0; teamNumber < totalTeams; teamNumber++)
            {
                var teamUsers = new List<SocketUser>();
                for (int teamSlot = 0; teamSlot < teamSize; teamSlot++)
                {
                    if (users.Count == 0) break;
                    var selectedUser = random.Next(users.Count);
                    teamUsers.Add(users[selectedUser]);
                    users.RemoveAt(selectedUser);
                }
                teams.Add(new Team(teamUsers));
            }
            return teams;
        }

        private static string BuildTeamText(Team team, int index)
        {
            return $"Team {index + 1}: \n \t {string.Join("\n \t", BuildUsersText(team.Players))}";
        }

        private static IEnumerable<string> BuildUsersText(List<SocketUser> team)
        {
            return team.Select(teamMember => $"{teamMember.Username}");
        }
    }
}