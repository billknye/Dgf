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
            var provider = this;
            foreach (var sub in state.States)
            {
                provider = provider.GetChildProvider(state, sub);
            }

            var interaction = provider.GetInteraction(state, state.Interaction);
            interaction.Modifier(state);

            provider = this;
            foreach (var sub in state.States)
            {
                provider = provider.GetChildProvider(state, sub);
            }

            var summary = provider.DescribeState(state);

            var transitions = provider.GetInteractions(state).Select((interaction, index) =>
            {
                var r = state.Clone();
                r.Interaction = index;
                return new Transition
                {
                    State = r,
                    Text = interaction.Text
                };
            });

            return (interaction, summary, transitions);
        }
        
        protected abstract IEnumerable<InteractionProvider<T>> GetChildProviders(T state);

        public abstract IEnumerable<Interaction<T>> GetInteractions(T state);

        public abstract GameStateSummary DescribeState(T state);
    }
}
