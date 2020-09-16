using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{

    public class TestGameState : MappedObjectBase<TestGameState>, IInteractionGameState
    {
        public int GameSeed { get; set; }

        public DateTime Now { get; set; }

        public int Interaction { get; set; }

        public List<int> States { get; set; }

        public LocationType LocationType { get; set; }

        public int WorldLocationId { get; set; }

        public int CityLocationId { get; set; }

        public int DungeonLocationId { get; set; }

        public List<PartyMember> PartyMembers { get; set; }
    }

    public enum LocationType
    {
        World = 0,
        City = 1,
        Dungeon = 2
    }

    public class PartyMember : MappedObjectBase<PartyMember>
    {
        public string Name { get; set; }
    }
    
}
