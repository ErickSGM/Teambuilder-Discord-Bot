using System.Collections.Generic;
using Discord.WebSocket;

namespace TeamBuilderbot.Models
{
    public class Team
    {
        public readonly List<SocketUser> Players;

        public Team(List<SocketUser> players)
        {
            Players = players;
        }
    }
}