using System;

namespace Dgf.Framework.States.Serialization;

/// <summary>
/// Abstraction for serializing game states to/from strings
/// </summary>
public interface IGameStateSerializer
{
    IGameState Deserialize(Type expectedType, string serialized);
    string Serialize(IGameState obj);
}
