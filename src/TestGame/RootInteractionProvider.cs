using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class RootInteractionProvider : InteractionProvider<TestGameState>
    {
        private readonly HelpInteractionProvider helpInteractionProvider;

        public RootInteractionProvider()
        {
            helpInteractionProvider = new HelpInteractionProvider();
        }

        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = "Welcome to the Game",
                Description = @$"
Welcome, here would be some help text or other helpful information

<audio autoplay>
  <source src=""/$slug/Assets/Sfx/handleCoins.ogg"" type=""audio/ogg"">
  Your browser does not support the audio element.
</audio>

{(state.Interaction == 1 ? "<div id=\"music\" data-song=\"/$slug/Assets/Music/TheLoomingBattle.ogg\"></div>" : "<div id=\"music\" data-song=\"/$slug/Assets/Music/NoMoreMagic.ogg\"></div>")}
"
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield return new Interaction<TestGameState>
            {
                Modifier = n => { },
                Text = "![Conversation](/$slug/Assets/Images/delapouite/chat-bubble.svg#interaction) Nothing",
                CompletedMessage = "You have accomplished...nothing"
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = n => { n.States.Push(0); },
                Text = "Help",
                CompletedMessage = "You went to help."
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = n => { },
                Text = "Other thing",
                CompletedMessage = "did another thing"
            };
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetChildProviders(TestGameState state)
        {
            yield return helpInteractionProvider;
        }
    }
}

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
            Text = "Return to Main Menu",
            CompletedMessage = "Returned to main menu"
        };
    }

    protected override IEnumerable<InteractionProvider<TestGameState>> GetChildProviders(TestGameState state)
    {
        throw new NotImplementedException();
    }
}
