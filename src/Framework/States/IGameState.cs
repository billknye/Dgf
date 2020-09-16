using Dgf.Framework.States.Serialization;
using System.Collections.Generic;

namespace Dgf.Framework.States
{
    /// <summary>
    /// Mostly used as a marker interface
    /// </summary>
    public interface IGameState : IMappedObject
    {
    }

    public interface IInteractionGameState : IGameState
    {
        public int Interaction { get; set; }

        public List<int> States { get; }
    }
}
