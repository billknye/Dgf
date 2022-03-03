using Dgf.Framework.States;
using Dgf.Framework.States.Interactions;
using Dgf.TestGame.Simulation;
using Dgf.TestGame.Simulation.Generator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dgf.TestGame.Playing;

public class WorldInteractionProvider : StaticInteractionProvider<TestGameState>
{
    public override GameStateSummary DescribeState(TestGameState state)
    {
        return new GameStateSummary
        {
            Title = $"Somewhere...",
            Description = ""
        };
    }

    protected override void AugmentChildStateDescription(TestGameState state, GameStateSummary summary)
    {
        summary.Attributes = new[]
            {
                DisplayItem.CreateWithImage($"@ {state.AreaLocation.X},{state.AreaLocation.Y}", Assets.Images.WireframeGlobe),
                DisplayItem.CreateWithImage($"Forest", Assets.Images.Forest)
            }.Concat(summary.Attributes ?? Enumerable.Empty<DisplayItem>());

        base.AugmentChildStateDescription(state, summary);
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
                        n.AreaLocation = new Point(loc.X, loc.Y); 
                    },
                    Item = DisplayItem.CreateWithImage($"Move {val} to {loc.X}, {loc.Y}", Assets.Images.Walk, description: "Walk somewhere..."),
                    Completed = $"You move {val}"
                };
            }
        }

        yield return new Interaction<TestGameState>
        {
            Modifier = TransitionTo<PartyManagementInteractionProvider>(),
            Item = DisplayItem.CreateWithImage($"Manage Party", Assets.Images.MeepleGroup, description: "Manage party members, inventory, settings, etc."),
            Completed = $"Managing Party"
        };

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
        yield return new PartyManagementInteractionProvider();
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

