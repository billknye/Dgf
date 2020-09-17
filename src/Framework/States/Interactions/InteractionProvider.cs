using Dgf.Framework.States;
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

            var interaction = provider.GetInteraction(state, state.Interaction);

            // Apply interaction to state
            interaction.Modifier(state);

            // Rebuild provider state hierarchy to get possibly updated provider for current state
            provider = this;
            foreach (var sub in state.States)
            {
                provider = provider.GetChildProvider(state, sub);
            }

            // use provider to get interactions and describe state
            var summary = provider.DescribeState(state);
            var transitions = provider.GetInteractions(state).Select((interaction, index) =>
            {
                var r = state.Clone();
                r.Interaction = index;
                return new Transition
                {
                    State = r,
                    Display = interaction.Item
                };
            });

            return (interaction, summary, transitions);
        }
        
        protected abstract IEnumerable<InteractionProvider<T>> GetChildProviders(T state);

        public abstract IEnumerable<Interaction<T>> GetInteractions(T state);

        public abstract GameStateSummary DescribeState(T state);
    }

    // Types of interaction provider
    // Static - always the same
    //      Help, Shop Buy/Sell sub menu (not the list of items)
    // Dynamic, tied to child states
    //      Shop items list
    //      Area of interest in town
}
