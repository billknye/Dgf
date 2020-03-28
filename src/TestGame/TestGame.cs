﻿using Dgf.Framework;
using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using Dgf.Framework.States.Transitions;
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
                CharacterName = "Joe",
                GameSeed = 42,
                Now = new DateTime(910, 03, 03, 09, 0, 0)
            };
        }

        protected override GameStateDescription DescribeStateInternal(TestGameState state)
        {
            var description = new GameStateDescription();

            description.Title = "Some state title";
            description.Description = "Something of a long descriptive text here about whatever is happening.";
            description.Groups = GetTransitionGroups(state);
            return description;
        }

        private IEnumerable<TransitionGroup> GetTransitionGroups(TestGameState state)
        {
            yield return new TransitionGroup
            {
                DisplayType = GroupDisplayType.List,
                Name = "Navigation",
                Transitions = GetNavigationTransitions(state)
            };
        }

        private IEnumerable<Transition> GetNavigationTransitions(TestGameState state)
        {
            yield return CreateTransition(state, "North", n => { n.Now += TimeSpan.FromSeconds(10); });
            yield return CreateTransition(state, "South", n => { n.Now += TimeSpan.FromSeconds(10); });
        }

        protected override bool ValidateStartingStateInternal(TestGameState state, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(state.CharacterName))
            {
                errors.Add("Character Name must not be null.");
                return false;
            }

            return true;
        }
    }
}
