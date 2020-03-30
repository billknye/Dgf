using Dgf.Framework.States.Transitions;
using System;
using System.IO;

namespace Dgf.Framework.States
{
    public static class GameStateEx
    {
        /// <summary>
        /// Clones the provded game state and creates a transition from it
        /// </summary>
        /// <param name="gameState">The game state to clone from</param>
        /// <param name="title">The title for the transition</param>
        /// <param name="modifier">An action applied to modify the clones copy of the game state</param>
        /// <param name="row">Unrealized thought about table based transition display</param>
        /// <param name="col">Unrealized thought about table based transition display</param>
        /// <returns></returns>
        public static Transition CreateTransition<T>(this T gameState, string title, Action<T> modifier, int? row = null, int? col = null) where T : IGameState, new()
        {
            T newState = Clone(gameState);
            modifier(newState);
            return new Transition
            {
                Title = title,
                State = newState,
                Row = row ?? 0,
                Column = col ?? 0
            };
        }

        // So lazy, we built everything to be serializable so let's use it
        public static T Clone<T>(T gameState) where T : IGameState, new()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            gameState.Write(writer);
            ms.Position = 0;
            var state = new T();
            using var reader = new BinaryReader(ms);
            state.Read(reader);
            return state;
        }
    }
}
