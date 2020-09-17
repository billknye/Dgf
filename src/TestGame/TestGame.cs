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

        public override (IGameState state, string description) CreateStartingState()
        {
            return (new TestGameState
            {
                Interaction = 0,
                States = new Stack<int>()
            }, "A new game");
        }

        protected override GameStateDescription DescribeStateInternal(TestGameState state)
        {
            var d = rootInteractionProvider.WalkInteraction(state);

            // Build description with state description suffixed with the interaction completion message/image
            d.summary.Description = $@"
{d.summary.Description}

<div class=""interaction-result {d.interaction.Completed.Classes}"">
    {(d.interaction.Completed.ImageUri != null ? 
        $@"
<svg xmlns=""http://www.w3.org/2000/svg"" version=""1.1"">
                <defs>
                    <filter id=""colorMask3"">
                        <feFlood flood-color=""currentColor"" result=""flood"" />
                        <feComposite in=""SourceGraphic"" in2=""flood"" operator=""arithmetic"" k1=""1"" k2=""0"" k3=""0"" k4=""0"" />
                    </filter>
                </defs>
                <image width=""100%"" height=""100%"" xlink:href=""{d.interaction.Completed.ImageUri}"" filter=""url(#colorMask3)"" />
            </svg>
    " : "")}
    {d.interaction.Completed.Text}
</div>
";

            // Copy interaction completion url to summary
            d.summary.SfxUri = d.interaction.CompletedAudioUri;

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
                StyleSheetPaths = new[] { Assets.Style.Styles }
            };
        }

        protected override bool ValidateStartingStateInternal(TestGameState state, List<string> errors)
        {
            /*if (state.PartyMembers == null || state.PartyMembers.Count != 1)
            {
                errors.Add("Party must start with exactly 1 character.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(state.PartyMembers.First().Name))
            {
                errors.Add("Character Name must not be null.");
                return false;
            }*/

            return true;
        }
    }
}
