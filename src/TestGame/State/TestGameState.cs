using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Dgf.TestGame.Simulation;
using Dgf.TestGame.Simulation.Generator;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Dgf.TestGame.State
{
    public class TestGameState : MappedObjectBase<TestGameState>, IInteractionGameState
    {
        private World world;
        private Location location;

        public int Interaction { get; set; }

        public Stack<int> States { get; set; }

        internal World World
        {
            get
            {
                if (world == null)
                    world = new World(Seed, this);

                return world;
            }
        }

        public int Seed { get; set; }

        public List<PartyMember> Members { get; set; }

        public int AreaId { get; set; }

        public Point AreaLocation { get; set; }

        public Location Location 
        { 
            get
            {
                if (location == null)
                    location = World[AreaId].GetLocation(AreaLocation.X, AreaLocation.Y);

                return location;
            } 
        }
    }
}
