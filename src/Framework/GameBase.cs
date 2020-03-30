using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Dgf.Framework.States.Transitions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Dgf.Framework
{
    /// <summary>
    /// Provides a base implementation to make implementing game classes easier
    /// </summary>
    /// <typeparam name="T">The game state type</typeparam>
    public abstract class GameBase<T> : IGame where T : IGameState, new()
    {
        private readonly IGameStateSerializer gameStateSerializer;

        public abstract string Slug { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public Type GameStateType => typeof(T);

        public GameBase(IGameStateSerializer gameStateSerializer)
        {
            this.gameStateSerializer = gameStateSerializer;
        }

        public abstract IGameState GetDefaultStartState();

        public virtual bool ValidateStartingState(IGameState state, out IEnumerable<string> messages)
        {            
            List<string> errors = new List<string>();
            messages = errors;

            if (state == null)
            {
                errors.Add("Got null game state.");
                return false;
            }

            if (state is T gameState) { }
            else
            {
                errors.Add($"Invalid game state instance type.  got {state.GetType().FullName}, expected: {typeof(T).FullName}");
                return false;
            }

            return ValidateStartingStateInternal(gameState, errors);
        }

        protected abstract bool ValidateStartingStateInternal(T state, List<string> errors);

        public GameStateDescription DescribeState(IGameState state)
        {
            if (state is null)
                throw new InvalidOperationException("Got null game state.");

            if (!(state is T gameState))
                throw new InvalidOperationException($"Invalid state type, got {state.GetType().FullName}, expected: {typeof(T).FullName}");

            return DescribeStateInternal(gameState);
        }

        protected abstract GameStateDescription DescribeStateInternal(T state);        
    }
}
