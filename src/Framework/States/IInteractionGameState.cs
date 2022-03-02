using System.Collections.Generic;

namespace Dgf.Framework.States
{
    /// <summary>
    /// An game state based onthe interaction system
    /// </summary>
    public interface IInteractionGameState : IGameState
    {
        public int Interaction { get; set; }

        public Stack<int> States { get; }
    }
}
