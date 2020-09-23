using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Dgf.Framework.States.Transitions;
using System.Collections.Generic;
using System.Linq;

namespace Dgf.Framework.States.Interactions
{
    public abstract class InteractionProvider<T> where T : IInteractionGameState, new()
    {
        public virtual InteractionProvider<T> GetChildProvider(T state, int index)
        {
            if (index == 0)
                return GetChildProviders(state).First();

            return GetChildProviders(state).Skip(index).First();            
        }

        public virtual Interaction<T> GetInteraction(T state, int index)
        {
            if (index == 0)
                return GetInteractions(state).First();

            return GetInteractions(state).Skip(index).First();
        }

        public (Interaction<T> interaction, GameStateSummary summary, IEnumerable<Transition> transitions) WalkInteraction(T state)
        {
            // Get provider for the current interaction
            var provider = this;
            foreach (var sub in state.States)
            {
                provider = provider.GetChildProvider(state, sub);
            }

            var completedInteraction = provider.GetInteraction(state, state.Interaction);

            // Apply interaction to state
            completedInteraction.Modifier(state);

            // Rebuild provider state hierarchy to get possibly updated provider for current state
            provider = this;
            foreach (var sub in state.States)
            {
                provider = provider.GetChildProvider(state, sub);
            }

            // use provider to get interactions and describe state
            var summary = provider.DescribeState(state);
            var interactions = provider.GetInteractions(state);

            // unrolled to support hidden indices
            var transitions = new List<Transition>();
            int index = 0;
            foreach (var interaction in interactions)
            {
                if (interaction.Hidden)
                {
                    index++;
                    continue;
                }

                var r = state.Clone();
                r.Interaction = index;

                transitions.Add(new Transition
                {
                    State = r,
                    Display = interaction.Item
                });
                index++;
            }

            return (completedInteraction, summary, transitions);
        }
        
        protected abstract IEnumerable<InteractionProvider<T>> GetChildProviders(T state);

        /// <summary>
        /// Returns a set of interactions available for the given state
        /// </summary>
        /// <remarks>
        /// This is separate from describe as it is used for applying the interactions as well
        /// </remarks>
        public abstract IEnumerable<Interaction<T>> GetInteractions(T state);

        public abstract GameStateSummary DescribeState(T state);
    }

    // Types of interaction provider
    // Static - always the same
    //      Help, Shop Buy/Sell sub menu (not the list of items)
    // Dynamic, tied to child states
    //      Shop items list
    //      Area of interest in town

    public abstract class InteractionGameBase<T> : GameBase<T> where T : IInteractionGameState, new()
    {
        public InteractionGameBase(IGameStateSerializer gameStateSerializer)
            : base(gameStateSerializer)
        {

        }

        protected abstract InteractionProvider<T> GetRootInteractionProvider(T state);

        protected override GameStateDescription DescribeStateInternal(T state)
        {
            var root = GetRootInteractionProvider(state);
            var d = root.WalkInteraction(state);

            // Copy interaction completion url to summary
            d.summary.SfxUri = d.interaction.CompletedAudioUri;

            return new GameStateDescription
            {
                Summary = d.summary,
                Status = d.interaction.Completed,
                Transitions = d.transitions,
            };
        }
    }
}
