using Dgf.Framework.States;
using System;

namespace Dgf.Framework.States.Interactions
{
    public class Interaction<T> where T : IInteractionGameState
    {
        /// <summary>
        /// Action to apply this interaction to a given game state
        /// </summary>
        public Action<T> Modifier { get; set; }

        /// <summary>
        /// Message displayed when interaction is completed
        /// </summary>
        public string CompletedMessage { get; set; }

        /// <summary>
        /// Text provided to interact with
        /// </summary>
        public string Text { get; set; }

        //todo image, styling, etc?
        // playing sound effect when selling?
    }
}
