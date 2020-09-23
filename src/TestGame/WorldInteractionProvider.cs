using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame.Simulation;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class WorldInteractionProvider : StaticInteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {

            throw new NotImplementedException();
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield break;
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetStaticProviders()
        {
            yield return new VendorInteractionProvider();
            yield return new ConversationInteractionProvider();
        }
    }
}

