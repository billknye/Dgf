using Dgf.Framework.States.Transitions;
using System.Collections.Generic;

namespace Dgf.Framework.States
{
    /// <summary>
    /// Provides a view model for information about a game state
    /// </summary>
    public class GameStateDescription
    {
        /// <summary>
        /// Summary of the current state
        /// </summary>
        public GameStateSummary Summary { get; set; }

        /// <summary>
        /// Status display item
        /// </summary>
        public DisplayItem Status { get; set; }

        /// <summary>
        /// Transitions available for this state
        /// </summary>
        public IEnumerable<Transition> Transitions { get; set; }
    }
}
