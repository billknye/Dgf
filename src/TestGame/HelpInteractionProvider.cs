using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame.State;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class HelpInteractionProvider : InteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = "Help",
                Description = "Some help text."
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield return new Interaction<TestGameState>
            {
                Modifier = n => { n.States.Pop(); },
                Item = DisplayItem.Create("Return to Menu"),
                Completed = DisplayItem.Create("Returned to Menu")
            };
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetChildProviders(TestGameState state)
        {
            throw new NotImplementedException();
        }
    }

}

