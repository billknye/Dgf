﻿using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{

    public class TestGameState : MappedObjectBase<TestGameState>, IGameState
    {
        public int GameSeed { get; set; }

        public DateTime Now { get; set; }

        public List<PartyMember> PartyMembers { get; set; }
    }

    public class PartyMember : MappedObjectBase<PartyMember>
    {
        public string Name { get; set; }
    }
    
}
