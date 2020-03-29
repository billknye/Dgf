using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dgf.Framework.States.Serialization
{
    /// <summary>
    /// Users reflection to build a simple map of read and write actions for an object
    /// based on its public properties.  Uses binary reader/writer for encoding
    /// NOT version safe (adding / removing / changing properties, may even be
    /// sad between runtime version changes if property order changes)
    /// </summary>
    public class MapBuilder
    {
        List<Action<BinaryReader, object>> reads = new List<Action<BinaryReader, object>>();
        List<Action<BinaryWriter, object>> writes = new List<Action<BinaryWriter, object>>();

        public MapBuilder(Type type, bool autoMapPublicProperties = true)
        {
            if (autoMapPublicProperties)
            {
                foreach (var property in type.GetProperties())
                {
                    var g = property.GetGetMethod();
                    var s = property.GetSetMethod();

                    if (g != null && s != null)
                        AddPropertyMap(property);
                }
            }
        }

        public void Write(object obj, BinaryWriter writer)
        {
            foreach (var write in writes)
            {
                write(writer, obj);
            }
        }

        public void Read(object obj, BinaryReader reader)
        {
            foreach (var read in reads)
            {
                read(reader, obj);
            }
        }

        public void AddPropertyMap(PropertyInfo member)
        {
            var mapping = map(member.PropertyType);
            reads.Add((rdr, obj) =>
            {
                member.SetValue(obj, mapping.reader(rdr));
            });
            writes.Add((wrt, obj) =>
            {
                mapping.writer(wrt, member.GetValue(obj));
            });
        }

        private (Func<BinaryReader, object> reader, Action<BinaryWriter, object> writer) map(Type t)
        {
            if (t == typeof(int))
            {
                return (r => r.ReadInt32(), 
                    (w, o) => w.Write((int)o));
            }
            if (t == typeof(string))
            {
                return (r => r.ReadString(), 
                    (w, o) => w.Write(o.ToString()));
            }
            if (t == typeof(bool))
            {
                return (r => r.ReadBoolean(), 
                    (w, o) => w.Write((bool)o));
            }
            if (t == typeof(DateTime))
            {
                return (r => new DateTime(r.ReadInt64()), 
                    (w, o) => w.Write(((DateTime)o).Ticks));
            }
            if (t == typeof(byte))
            {
                return (r => r.ReadByte(), 
                    (w, o) => w.Write((byte)o));
            }
            if (t.IsEnum)
            {
                var underlying = Enum.GetUnderlyingType(t);
                return map(underlying);
            }
            if (t.GetInterface(nameof(IMappedObject)) != null)
            {
                return mapMappedObject(t);
            }
            if (t.IsGenericType && 
                (
                    t.GetGenericTypeDefinition() == typeof(IDictionary<,>) 
                || t.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                ))
            {
                return mapDictionary(t);
            }
            if (t.BaseType == typeof(Array) || (t.IsGenericType &&
                (
                    t.GetGenericTypeDefinition() == typeof(List<>)
                    || t.GetGenericTypeDefinition() == typeof(IList<>)
                    || t.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                )))
            {
                return mapArrayOrList(t);
            }

            throw new NotSupportedException($"Unsppported mapping type: {t.FullName}");
        }

        private static (Func<BinaryReader, object> reader, Action<BinaryWriter, object> writer) mapMappedObject(Type t)
        {
            var a = t.GetConstructor(Type.EmptyTypes);
            if (a == null)
                throw new InvalidOperationException();

            return (r =>
            {
                var exists = r.ReadBoolean();
                if (!exists)
                    return null;

                var o = a.Invoke(Type.EmptyTypes);
                (o as IMappedObject).Read(r);
                return o;

            }, (w, o) =>
            {
                if (o == null)
                {
                    w.Write(false);
                    return;
                }

                w.Write(true);
                (o as IMappedObject).Write(w);
            }
            );
        }

        private (Func<BinaryReader, object> reader, Action<BinaryWriter, object> writer) mapDictionary(Type t)
        {
            var typeArguments = t.GetGenericArguments();

            var ctor = typeof(Dictionary<,>).MakeGenericType(typeArguments).GetConstructor(new[] { typeof(int) });
            var indexer = typeof(IDictionary<,>).MakeGenericType(typeArguments).GetProperties().Where(n => n.GetIndexParameters().Length > 0).FirstOrDefault();
            var enumeratedType = typeof(KeyValuePair<,>).MakeGenericType(typeArguments);
            var countMethod = typeof(ICollection<>).MakeGenericType(enumeratedType).GetProperty("Count");
            var keyProp = enumeratedType.GetProperty("Key");
            var valueProp = enumeratedType.GetProperty("Value");
            var keyMap = map(typeArguments[0]);
            var valueMape = map(typeArguments[1]);

            return (r =>
            {
                var exists = r.ReadBoolean();
                if (!exists)
                    return null;

                var len = r.ReadInt32();

                var instance = ctor.Invoke(new object[] { len });
                for (int i = 0; i < len; i++)
                {
                    var key = keyMap.reader(r);
                    var val = valueMape.reader(r);
                    indexer.SetValue(instance, val, new[] { key });
                }
                return instance;
            }, (w, o) =>
            {
                if (o == null)
                {
                    w.Write(false);
                    return;
                }

                var count = (int)countMethod.GetValue(o);
                w.Write(true);
                w.Write(count);
               
                foreach (var kvp in o as System.Collections.IEnumerable)
                {
                    var key = keyProp.GetValue(kvp);
                    var value = valueProp.GetValue(kvp);
                    keyMap.writer(w, key);
                    valueMape.writer(w, value);
                }
            });
        }

        private (Func<BinaryReader, object> reader, Action<BinaryWriter, object> writer) mapArrayOrList(Type t)
        {
            Func<Type, int, object> ctor = null;
            Func<object, int> getLength = null;
            Action<object, int, object> setIndex = null;
            Func<object, int, object> getIndex = null;
            Type elementType = null;

            if (t.BaseType == typeof(Array))
            {
                ctor = (t, l) => Array.CreateInstance(t, l);
                setIndex = (c, i, e) => ((Array)c).SetValue(e, i);
                getIndex = (c, i) => ((Array)c).GetValue(i);
                getLength = c => ((Array)c).Length;
                elementType = t.GetElementType();
            }
            // For IEnumerable<> this is only going to work if it is backed by some IList like type, 
            // e.g. array, List<>, etc.  If it were an iterator derrived enumerable or something this
            // will fail
            else if (t.GetGenericTypeDefinition() == typeof(List<>) 
                || t.GetGenericTypeDefinition() == typeof(IList<>)
                || t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = t.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(elementType);
                var listConstructor = listType.GetConstructor(new Type[] { typeof(int) });
                var listAdd = listType.GetMethod("Add");
                var listCount = listType.GetProperty("Count");
                var listSet = listType.GetProperties().Where(n => n.GetIndexParameters().Length > 0).First();
                ctor = (t, l) => listConstructor.Invoke(new object[] { l });
                setIndex = (c, i, e) => listAdd.Invoke(c, new object[] { e });
                getIndex = (c, i) => listSet.GetValue(c, new object[] { i });
                getLength = c => (int)listCount.GetValue(c, null);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported array/list type mapping: {t.FullName}.");
            }

            var elementMap = map(elementType);

            return (rdr =>
            {
                var exist = rdr.ReadBoolean();
                if (!exist)
                    return null;

                var len = rdr.ReadInt32();
                var collection = ctor(elementType, len);

                for (int i = 0; i < len; i++)
                {
                    var read = elementMap.reader(rdr);
                    setIndex(collection, i, read);
                }

                return collection;
            }, (wrt, obj) =>
            {
                if (obj == null)
                {
                    wrt.Write(false);
                }
                else
                {
                    var len = getLength(obj);
                    wrt.Write(true);
                    wrt.Write(getLength(obj));

                    for (int i = 0; i < len; i++)
                    {
                        var value = getIndex(obj, i);
                        elementMap.writer(wrt, value);
                    }
                }
            }
            );
        }
    }
}
