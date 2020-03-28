using Dgf.Framework.Display;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using System;

namespace Dgf.TestGame
{

    public class TestGameState : MappedObjectBase<TestGameState>, IGameState
    {
        public string CharacterName { get; set; }

        public int GameSeed { get; set; }

        public DateTime Now { get; set; }
    }
}
