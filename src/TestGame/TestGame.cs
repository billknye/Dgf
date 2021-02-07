using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.Framework.States.Serialization;
using Dgf.TestGame;
using Dgf.TestGame.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;

namespace Dgf.TestGame
{
    public class TestGame : InteractionGameBase<TestGameState>
    {
        private readonly InteractionProvider<TestGameState> rootInteractionProvider;

        public TestGame(IGameStateSerializer gameStateSerializer) : base(gameStateSerializer)
        {
            rootInteractionProvider = new RootInteractionProvider();
        }

        public override string Slug => "Test";

        public override string Name => "Test Game";

        public override string Description => "A Test Game for debugging and testing.";

        public override (IGameState state, string description) CreateStartingState()
        {
            var r = new Random();

            return (new TestGameState
            {
                Interaction = 0,
                States = new Stack<int>(),
                Seed = r.Next(),
                Members = new List<PartyMember>
                {
                    new PartyMember
                    {
                        Name = "Joe"
                    }
                },
                AreaId = 5,
                AreaLocation = new Point(12, 12)
            }, "A new game");
        }

        protected override GameHostingConfiguration GetGameHostingConfiguration()
        {
            return new GameHostingConfiguration
            {
                StyleSheetPaths = new[] { Assets.Style.Styles }
            };
        }

        protected override InteractionProvider<TestGameState> GetRootInteractionProvider(TestGameState state) => rootInteractionProvider;

        protected override bool ValidateStartingStateInternal(TestGameState state, List<string> errors)
        {
            if (state.Members == null || state.Members.Count != 1)
            {
                errors.Add("Party must start with exactly 1 character.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(state.Members.First().Name))
            {
                errors.Add("Character Name must not be null or empty.");
                return false;
            }

            return true;
        }
    }
}
