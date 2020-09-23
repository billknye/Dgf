using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class ConversationInteractionProvider : StaticInteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetStaticProviders()
        {
            yield break;
        }
    }
}

