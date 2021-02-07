using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame.Simulation;
using Dgf.TestGame.Simulation.Generator;
using Dgf.TestGame.State;
using System;
using System.Collections.Generic;

namespace Dgf.TestGame
{
    public class WorldInteractionProvider : StaticInteractionProvider<TestGameState>
    {
        public override GameStateSummary DescribeState(TestGameState state)
        {
            return new GameStateSummary
            {
                Title = $"Location: {state.AreaLocation.X},{state.AreaLocation.Y}",
                Description = ""
            };
        }

        public override IEnumerable<Interaction<TestGameState>> GetInteractions(TestGameState state)
        {
            foreach (var val in Enum.GetValues(typeof(CardinalDirection)))
            {
                var loc = GetNeighboringLocation(state, (CardinalDirection)val);
                if (loc != null)
                {
                    yield return new Interaction<TestGameState>
                    {
                        Modifier = n => {
                            Console.WriteLine($"Moving, start: {n.AreaLocation}");
                            n.AreaLocation = new Point(loc.X, loc.Y); 
                            Console.WriteLine($"Moving, end: {n.AreaLocation}");
                        },
                        Item = $"Move {val} to {loc.X}, {loc.Y}",
                        Completed = $"You move {val}"
                    };
                }
            }

            yield break;
        }

        private static Location GetNeighboringLocation(TestGameState gameState, CardinalDirection direction)
        {
            var dirPoint = offsets[(int)direction];
            var pointX = gameState.AreaLocation.X + dirPoint.x;
            var pointY = gameState.AreaLocation.Y + dirPoint.y;

            var area = gameState.World[gameState.AreaId];
            return area.GetLocation(pointX, pointY);
        }

        protected override IEnumerable<InteractionProvider<TestGameState>> GetStaticProviders()
        {
            yield return new VendorInteractionProvider();
            yield return new ConversationInteractionProvider();
        }

        private readonly static (int x, int y)[] offsets = new (int x, int y)[]
        {
            (0, -1),
            (-1, 0),
            (0, 1),
            (1, 0)
        };
    }

    public enum CardinalDirection
    {
        North,
        West,
        South,
        East
    }
}

