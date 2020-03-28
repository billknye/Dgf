using System.IO;

namespace Dgf.Framework.States.Serialization
{
    /// <summary>
    /// Provides base class for game state objects to use the MapBuilder system for serialization
    /// </summary>
    public abstract class MappedObjectBase<T> : IMappedObject
    {
        private static MapBuilder map;

        static MappedObjectBase()
        {
            map = new MapBuilder(typeof(T));
        }

        public MappedObjectBase()
        {
        }

        public void Read(BinaryReader reader)
        {
            map.Read(this, reader);
        }

        public void Write(BinaryWriter writer)
        {
            map.Write(this, writer);
        }
    }
}
