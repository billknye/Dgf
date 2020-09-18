using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Dgf.Framework.States.Transitions
{
    /// <summary>
    /// Represents an available transition to a new game state
    /// </summary>
    public class Transition
    {
        public DisplayItem Display { get; set; }

        public IInteractionGameState State { get; set; }
    }
}
