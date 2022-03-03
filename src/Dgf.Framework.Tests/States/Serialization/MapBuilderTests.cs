using Dgf.Framework.States.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Dgf.Framework.Tests.States.Serialization;

[TestClass]
public class MapBuilderTests
{
    [TestMethod]
    public void Simple_properties_can_be_serialized()
    {
        var o = new SimpleProperties
        {
            Number = 42,
            String = "A string"
        };

        var bytes = serialize(o);
        var newInstance = new SimpleProperties();
        deserialize(newInstance, bytes);
        Assert.AreEqual(o.Number, newInstance.Number);
        Assert.AreEqual(o.String, newInstance.String);
    }

    [TestMethod]
    public void Enumerable_properties_can_be_serialized()
    {
        var o = new EnumerableProperties
        {
            Array = new int[] { 1, 1, 2, 3, 5, 8, 11 },
            Strings = new List<string> { "Frank", "Tom", "Jane", "Sarah" },
            Dates = new string[] { "A", "B", "C" }
        };

        var bytes = serialize(o);
        var newInstance = new EnumerableProperties();
        deserialize(newInstance, bytes);

        Assert.IsTrue(o.Array.SequenceEqual(newInstance.Array));
        Assert.IsTrue(o.Dates.SequenceEqual(newInstance.Dates));
        Assert.IsTrue(o.Strings.SequenceEqual(newInstance.Strings));
    }

    [TestMethod]
    public void Dictionary_properties_can_be_serialized()
    {
        var o = new DictionaryProperties
        {
            Tags = new Dictionary<string, string>
            {
                { "A", "Apple" },
                { "B", "Bat" },
                { "C", "Cat" }
            }
        };

        var bytes = serialize(o);
        var newInstance = new DictionaryProperties();
        deserialize(newInstance, bytes);

        Assert.IsTrue(o.Tags.SequenceEqual(newInstance.Tags));
    }

    [TestMethod]
    public void SubObjectProperties_can_be_serialized()
    {
        var o = new SubObjectProperties
        {
            Teacher = new SubObjectProperties.ChildObject
            {
                Name = "Mrs. Smith",
                Age = 30
            },
            Students = new[]
            {
                new SubObjectProperties.ChildObject { Name = "Susie Jones", Age = 10 },
                new SubObjectProperties.ChildObject { Name = "Bobby Tables", Age = 11 }
            }
        };

        var bytes = serialize(o);
        var newinstance = new SubObjectProperties();
        deserialize(newinstance, bytes);

        Assert.AreEqual(o.Teacher.Age, newinstance.Teacher.Age);
        Assert.AreEqual(o.Teacher.Name, newinstance.Teacher.Name);
        Assert.IsTrue(o.Students.SequenceEqual(newinstance.Students, new SubObjectsComparer()));
    }

    private static byte[] serialize(IMappedObject o)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriterEx(ms);
        o.Write(writer);
        return ms.ToArray();
    }

    private static void deserialize(IMappedObject o, byte[] buffer)
    {
        using var ms = new MemoryStream(buffer);
        using var reader = new BinaryReaderEx(ms);
        o.Read(reader);
    }

    class SimpleProperties : MappedObjectBase<SimpleProperties>
    {
        public int Number { get; set; }
        public string String { get; set; }
    }

    class EnumerableProperties : MappedObjectBase<EnumerableProperties>
    {
        public List<string> Strings { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<string> Dates { get; set; }
    }

    class DictionaryProperties : MappedObjectBase<DictionaryProperties>
    {
        public Dictionary<string, string> Tags { get; set; }
    }

    class SubObjectProperties : MappedObjectBase<SubObjectProperties>
    {
        public ChildObject Teacher { get; set; }

        public IEnumerable<ChildObject> Students { get; set; }


        public class ChildObject : MappedObjectBase<ChildObject>
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }

    class SubObjectsComparer : IEqualityComparer<SubObjectProperties.ChildObject>
    {
        public bool Equals([AllowNull] SubObjectProperties.ChildObject x, [AllowNull] SubObjectProperties.ChildObject y)
        {
            return x?.Age == y?.Age && x?.Name == y?.Name;
        }

        public int GetHashCode([DisallowNull] SubObjectProperties.ChildObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
