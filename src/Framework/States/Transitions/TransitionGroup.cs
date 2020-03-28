using System.Collections.Generic;

namespace Dgf.Framework.States.Transitions
{
    /// <summary>
    /// Represents a group of available game states to transition to
    /// </summary>
    public class TransitionGroup
    {
        public string Name { get; set; }

        public GroupDisplayType DisplayType { get; set; }

        public IEnumerable<Transition> Transitions { get; set; }
    }
}
