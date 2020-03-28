namespace Dgf.Framework.States.Transitions
{
    /// <summary>
    /// Represents an available transition to a new game state
    /// </summary>
    public class Transition
    {
        public string Title { get; set; }

        public IGameState State { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
    }
}
