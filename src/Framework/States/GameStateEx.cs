using Dgf.Framework.States.Serialization;
using Dgf.Framework.States.Transitions;
using System;
using System.IO;

namespace Dgf.Framework.States
{
    public static class GameStateEx
    {
        // So lazy, we built everything to be serializable so let's use it
        public static T Clone<T>(this T gameState) where T : IGameState, new()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriterEx(ms);
            gameState.Write(writer);
            ms.Position = 0;
            var state = new T();
            using var reader = new BinaryReaderEx(ms);
            state.Read(reader);
            return state;
        }
    }
}
