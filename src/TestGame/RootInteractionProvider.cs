using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame;
using Dgf.TestGame.State;
using System.Collections.Generic;
using System.Linq;

namespace Dgf.TestGame
{
    public class RootInteractionProvider : StaticInteractionProvider<TestGameState>
    {
        public RootInteractionProvider()
        {
            
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetStaticProviders()
        {
            yield return new HelpInteractionProvider();
            yield return new CreditsInteractionProvider();
            yield return new WorldInteractionProvider();
        }

        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = DisplayItem.Create("Menu"),
                Description = DisplayItem.Create(@$"Welcome.  This game exists to test feature of the framework.  This is the main menu which can provide help, credits and other meta information about the game.
"),
                MusicUri = Assets.Music.NoMoreMagic                
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield return new Interaction<TestGameState>
            {
                Modifier = n => { },
                Item = null,
                Completed = DisplayItem.Create("New Game Created"),
                Hidden = true
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = TransitionTo<WorldInteractionProvider>(n => n.AreaId = 5),
                Item = DisplayItem.CreateWithImage("Begin Journey", Assets.Images.Walk),
                Completed = DisplayItem.Create("You begin your journey"),
                CompletedAudioUri = Assets.Sfx.HandleCoins
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = TransitionTo<HelpInteractionProvider>(), 
                Item = DisplayItem.CreateWithImage("Help", Assets.Images.Help),
                Completed = DisplayItem.Create("Getting Help")
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = TransitionTo<CreditsInteractionProvider>(), 
                Item = DisplayItem.CreateWithImage("Credits", Assets.Images.ThumbUp),
                Completed = DisplayItem.Create("Viewing Credits")
            };
        }
    }
}

