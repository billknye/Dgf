using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.Framework.States.Serialization;
using Dgf.TestGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;

namespace Dgf.TestGame
{
    public class TestGame : GameBase<TestGameState>
    {
        private readonly InteractionProvider<TestGameState> rootInteractionProvider;

        public TestGame(IGameStateSerializer gameStateSerializer) : base(gameStateSerializer)
        {
            rootInteractionProvider = new RootInteractionProvider();
        }

        public override string Slug => "Test";

        public override string Name => "Test Game";

        public override string Description => "A Test Game for debugging and testing.";

        public override IGameState GetDefaultStartState()
        {
            return new TestGameState
            {
                GameSeed = 42,
                Now = new DateTime(910, 03, 03, 09, 0, 0),

                Interaction = 0,
                States = new List<int>
                {
                },

                PartyMembers = new List<PartyMember>
                {
                    new PartyMember
                    {
                        Name = "Joe",
                    }
                }
            };
        }

        protected override GameStateDescription DescribeStateInternal(TestGameState state)
        {
            var random = new Random(state.GameSeed);

            var d = rootInteractionProvider.WalkInteraction(state);

            return new GameStateDescription
            {
                Summary = d.summary,
                Transitions = d.transitions,               
            };
        }

        protected override GameHostingConfiguration GetGameHostingConfiguration()
        {
            return new GameHostingConfiguration
            {
                StyleSheetPaths = new[] { "Assets/Styles.css" }
            };
        }

        protected override bool ValidateStartingStateInternal(TestGameState state, List<string> errors)
        {
            if (state.PartyMembers == null || state.PartyMembers.Count != 1)
            {
                errors.Add("Party must start with exactly 1 character.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(state.PartyMembers.First().Name))
            {
                errors.Add("Character Name must not be null.");
                return false;
            }

            return true;
        }
    }

    public class RootInteractionProvider : InteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = "Welcome to the Game",
                Description = @"
Welcome, here would be some help text or other helpful information
"
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            yield return new Interaction<TestGameState>
            {
                Modifier = n => { },
                Text = "Nothing",
                CompletedMessage = "You have accomplished...nothing"
            };
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetChildProviders(TestGameState state)
        {
            throw new NotImplementedException();
        }
    }

    // Enumerate Transition Options
    // Persue sub option if in path to apply that action to state and get states from there

    // State vs interaction
    // state + interaction = new state
    // state stack

    // 1.2.3.5.6:10
    // dotted state reference, colon interaction reference?
}
