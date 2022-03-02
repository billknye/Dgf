using Dgf.Framework.States;
using System;
using System.Collections;
using System.Reflection.Emit;

namespace Dgf.Framework.States.Interactions
{
    public class Interaction<T> where T : IInteractionGameState
    {
        /// <summary>
        /// Action to apply this interaction to a given game state
        /// </summary>
        public Action<T> Modifier { get; set; }

        /// <summary>
        /// Displayed when interaction is completed
        /// </summary>
        public DisplayItem Completed { get; set; }

        /// <summary>
        /// Audio played when interaction is completed
        /// </summary>
        public string CompletedAudioUri { get; set; }

        /// <summary>
        /// Clickable option
        /// </summary>
        public DisplayItem Item { get; set; }

        /// <summary>
        /// Allows for hidden interactions
        /// </summary>
        public bool Hidden { get; set; }
    }

}
