using Dgf.Framework.States.Transitions;
using System.Collections.Generic;

namespace Dgf.Framework.States
{
    /// <summary>
    /// Provides a view model for information about a game state
    /// </summary>
    public class GameStateDescription
    {
        public GameStateSummary Summary { get; set; }

        public IEnumerable<Transition> Transitions { get; set; }
    }

    public class GameStateSummary
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
