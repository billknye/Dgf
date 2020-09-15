using Dgf.Framework;
using Dgf.Framework.States;
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

    public class InteractionProvider<T> where T : IGameState
    {
        public IEnumerable<Interaction<T>> Generate(T state, DottedPath path)
        {
            yield break;
        }
    }

    public class Interaction<T> where T : IGameState
    {
        /// <summary>
        /// Action to apply this interaction to a given game state
        /// </summary>
        public Action<T> Modifier { get; set; }

        public InteractionProvider<T> ChildState { get; set; }

        public string CompletedMessage { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //todo image, styling, etc?
        // playing sound effect when selling?
    }

    public class DottedPath : IMappedObject
    {
        private List<byte> values;
        private int position;

        public DottedPath()
        {
            values = new List<byte>();
        }

        public DottedPath(byte[] bytes)
        {
            values = new List<byte>(bytes);
        }

        public void Reset()
        {
            position = 0;
        }

        public byte? GetNext()
        {
            if (position >= values.Count)
                return null;

            return values[++position];
        }

        public void Append(byte value)
        {
            values.Add(value);
        }

        public void Read(BinaryReader reader)
        {
            var length = reader.ReadByte();
            var bytes = reader.ReadBytes(length);
            values.AddRange(bytes);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)values.Count);
            writer.Write(values.ToArray());
        }
    }
}
