using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame.Playing
{
    public class PartyManagementInteractionProvider : InteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = "Party Management",
                Description = ""
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield return new Interaction<TestGameState>
            {
                Modifier = n => { n.States.Pop(); },
                Item = DisplayItem.CreateWithImage("Exit Party Management", Assets.Images.ReturnArrow),
                Completed = DisplayItem.Create("Exited Party Management")
            };
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetChildProviders(TestGameState state)
        {
            throw new NotImplementedException();
        }
    }
}

