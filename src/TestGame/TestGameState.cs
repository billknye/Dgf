using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class TestGameState : MappedObjectBase<TestGameState>, IInteractionGameState
    {
        public int Interaction { get; set; }

        public Stack<int> States { get; set; }
    }

    
}
