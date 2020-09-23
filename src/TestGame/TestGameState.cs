using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Dgf.TestGame.Simulation;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class TestGameState : MappedObjectBase<TestGameState>, IInteractionGameState
    {
        private World world;

        public int Interaction { get; set; }

        public Stack<int> States { get; set; }

        internal World World
        {
            get
            {
                if (world == null)
                    world = new World(Seed);

                return world;
            }
        }

        public int Seed { get; set; }

        public List<PartyMember> Members { get; set; }

        public int AreaId { get; set; }
    }

    public class PartyMember : MappedObjectBase<PartyMember>
    {
        public string Name { get; set; }
    }
}
