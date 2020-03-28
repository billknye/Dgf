using System.IO;

namespace Dgf.Framework.States.Serialization
{
    /// <summary>
    /// Interface used for object serialization, used by the game state objects
    /// </summary>
    public interface IMappedObject
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}
