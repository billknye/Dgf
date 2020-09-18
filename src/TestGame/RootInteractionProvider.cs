using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class RootInteractionProvider : InteractionProvider<TestGameState>
    {
        private readonly HelpInteractionProvider helpInteractionProvider;
        private readonly CreditsInteractionProvider creditsInteractionProvider;

        public RootInteractionProvider()
        {
            helpInteractionProvider = new HelpInteractionProvider();
            creditsInteractionProvider = new CreditsInteractionProvider();
        }

        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = DisplayItem.Create("Welcome to the Game"),
                Description = DisplayItem.Create(@$"Welcome, here would be some help text or other helpful information"),
                MusicUri = Assets.Music.NoMoreMagic,
                Attributes = new[]
                {
                    DisplayItem.CreateWithImage("Party: 1 / 1", Assets.Images.Person),
                    DisplayItem.CreateWithImage("12953", Assets.Images.Coins)
                }
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield return new Interaction<TestGameState>
            {
                Modifier = n => { },
                Item = DisplayItem.CreateWithImage("Nothing", Assets.Images.ChatBubble),
                Completed = DisplayItem.Create("You have accomplished...nothing"),
                CompletedAudioUri = Assets.Sfx.HandleCoins
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = n => { n.States.Push(0); },
                Item = DisplayItem.CreateWithImage("Help", Assets.Images.Help),
                Completed = DisplayItem.Create("Getting Help")
            };

            yield return new Interaction<TestGameState>
            {
                Modifier = n => { n.States.Push(1); },
                Item = DisplayItem.CreateWithImage("Credits", Assets.Images.ThumbUp),
                Completed = DisplayItem.Create("Viewing Credits")
            };
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetChildProviders(TestGameState state)
        {
            yield return helpInteractionProvider;
            yield return creditsInteractionProvider;
        }
    }

}

