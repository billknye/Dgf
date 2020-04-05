using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Dgf.TestGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Dgf.TestGame
{
    public class TestGame : GameBase<TestGameState>
    {
        public TestGame(IGameStateSerializer gameStateSerializer) : base(gameStateSerializer)
        {
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

            var worldStateGenerator = new WorldStateGenerator(random.Next());

            var description = new GameStateDescription();

            switch (state.LocationType)
            {
                case LocationType.World:
                    var a = worldStateGenerator.GetLocation(state.WorldLocationId);
                    description.Title = a.title;
                    description.Description = a.description;
                    description.Groups = worldStateGenerator.GetNavigationGroups(state);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return description;
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
}
