using System.Collections.Generic;

namespace TeamBuilderbot.Models
{
    public class CurrentGames
    {
        public Dictionary<string, Game> Games { get; set; } = new Dictionary<string, Game>();
    }
}
