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
                List = GetWorldTransitions(state)
            };

            if (state.WorldLocationId == 1)
            {
                // dummy up a vendor table
                yield return new TransitionGroup
                {
                    DisplayType = GroupDisplayType.Table,
                    Name = "Book Vendor",
                    Rows = GetTestVendorRows(state)
                };
            }
        }

        private IEnumerable<TransitionGroup> GetTestVendorRows(TestGameState state)
        {
            var books = new[] { "Glass", "Wood", "Iron", "Stone" };

            foreach (var book in books)
            {
                yield return new TransitionGroup
                {
                    Name = "A book on " + book,
                    List = new[] 
                    { 
                        state.CreateTransition("-5", n => { }), 
                        state.CreateTransition("-1", n => { }), 
                        state.CreateTransition("+1", n => { }), 
                        state.CreateTransition("+5", n => { }) 
                    }
                };
            }
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
