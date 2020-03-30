using Dgf.Framework.States;
using Dgf.Framework.States.Transitions;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class WorldStateGenerator
    {
        private readonly int seed;

        public WorldStateGenerator(int seed)
        {
            this.seed = seed;
        }

        public (string title, string description) GetLocation(int worldLocationId)
        {
            switch (worldLocationId)
            {
                case 0: // start room
                    return ("Welcome", @"This is the beginning, of the end.
Things to learn:

- [ ] How to fight
- [ ] How to buy
- [x] Other things

");
                case 1: // another place
                    return ("Another place", "This is the only other place in the world.");
            }

            throw new InvalidOperationException();
        }

        public IEnumerable<TransitionGroup> GetNavigationGroups(TestGameState state)
        {
            yield return new TransitionGroup
            {
                DisplayType = GroupDisplayType.List,
                Name = "World Navigation",
                Transitions = GetWorldTransitions(state)
            };
        }

        private IEnumerable<Transition> GetWorldTransitions(TestGameState state)
        {
            switch (state.WorldLocationId)
            {
                case 0: // start room
                    yield return state.CreateTransition("Another Room", n => n.WorldLocationId = 1);
                    yield break;
                case 1: // another place
                    yield return state.CreateTransition("Start Room", n => n.WorldLocationId = 0);
                    yield break;
            }
        }
    }
}
