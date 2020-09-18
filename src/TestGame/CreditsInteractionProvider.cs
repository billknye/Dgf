using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class CreditsInteractionProvider : InteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = DisplayItem.Create("Credits"),
                Description = $@"

Icons:
https://game-icons.net/

Fonts:
https://www.dafont.com/deutsche-zierschrif.font
https://www.dafont.com/cloister-black.font

Music: 
https://opengameart.org/content/no-more-magic
https://opengameart.org/content/orchestral-battle-music

Sound Effects:
https://opengameart.org/content/50-rpg-sound-effects
"
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

