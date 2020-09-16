﻿namespace Dgf.Framework.States.Transitions
{
    /// <summary>
    /// Represents an available transition to a new game state
    /// </summary>
    public class Transition
    {
        public string Text { get; set; }

        public IGameState State { get; set; }
    }
}
