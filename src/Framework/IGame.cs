using Dgf.Framework.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgf.Framework
{
    /// <summary>
    /// Provides the abstract interaction to a game
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Unique url slug for this game
        /// </summary>
        string Slug { get; }

        /// <summary>
        /// Name of the Game
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the game
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The actual type of the game states for this game
        /// </summary>
        Type GameStateType { get; }

        /// <summary>
        /// Gets a default or randomly generated starting state for this game
        /// </summary>
        IGameState GetDefaultStartState();

        /// <summary>
        /// Provides validation when starting a new game and allowing players to choose starting values
        /// </summary>
        bool ValidateStartingState(IGameState state, out IEnumerable<string> messages);

        /// <summary>
        /// Provides a detailed description of the given state and transitions available
        /// </summary>
        GameStateDescription DescribeState(IGameState state);
    }
}
