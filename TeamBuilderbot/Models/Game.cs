using System.Collections.Generic;

namespace TeamBuilderbot.Models
{
    public class Game
    {
        public string Name { get; }
        public IEnumerable<Team> Teams { get; }

        public Game(string name, IEnumerable<Team> teams)
        {
            Name = name;
            Teams = teams;
        }
    }
}