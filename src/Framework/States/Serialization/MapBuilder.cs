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
        List<Action<BinaryReaderEx, object>> reads;
        List<Action<BinaryWriterEx, object>> writes;

        public MapBuilder(Type type, bool autoMapPublicProperties = true)
        {
            reads = new List<Action<BinaryReaderEx, object>>();
            writes = new List<Action<BinaryWriterEx, object>>();

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

        public void Write(object obj, BinaryWriterEx writer)
        {
            foreach (var write in writes)
            {
                write(writer, obj);
            }
        }

        public void Read(object obj, BinaryReaderEx reader)
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

        private (Func<BinaryReaderEx, object> reader, Action<BinaryWriterEx, object> writer) map(Type t)
        {
            if (t == typeof(int))
            {
                return (r => r.Read7BitEncoded(), 
                    (w, o) => w.Write7BitEncoded((int)o));
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
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Stack<>))
            {
                return mapStack(t);
            }

            throw new NotSupportedException($"Unsppported mapping type: {t.FullName}");
        }

        private static (Func<BinaryReaderEx, object> reader, Action<BinaryWriterEx, object> writer) mapMappedObject(Type t)
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

        private (Func<BinaryReaderEx, object> reader, Action<BinaryWriterEx, object> writer) mapDictionary(Type t)
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

        private (Func<BinaryReaderEx, object> reader, Action<BinaryWriterEx, object> writer) mapStack(Type t)
        {
            var elementType = t.GetGenericArguments()[0];
            var stackType = typeof(Stack<>).MakeGenericType(elementType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var stackConstructor = stackType.GetConstructor(new Type[] { enumerableType });
            var arrayGet = stackType.GetMethod("ToArray");

            var elementMap = map(elementType);

            return (rdr =>
            {
                var exist = rdr.ReadBoolean();
                if (!exist)
                    return null;

                var len = rdr.Read7BitEncoded();
                var array = Array.CreateInstance(elementType, len);

                for (int i = 0; i < len; i++)
                {
                    var read = elementMap.reader(rdr);
                    array.SetValue(read, i);
                }

                return stackConstructor.Invoke(new object[] { array });
            }, (wrt, obj) =>
            {
                if (obj == null)
                {
                    wrt.Write(false);
                }
                else
                {
                    wrt.Write(true);
                    var array = arrayGet.Invoke(obj, null) as Array;
                    wrt.Write7BitEncoded(array.Length);

                    for (int i = array.Length - 1; i >= 0; i--)
                    {
                        elementMap.writer(wrt, array.GetValue(i));
                    }
                }
            });
        }

        private (Func<BinaryReaderEx, object> reader, Action<BinaryWriterEx, object> writer) mapArrayOrList(Type t)
        {
            Func<Type, int, object> ctor = null;
            Func<object, int> getLength = null;
            Action<object, int, object> setIndex = null;
            Type elementType = null;

            if (t.BaseType == typeof(Array))
            {
                ctor = (t, l) => Array.CreateInstance(t, l);
                setIndex = (c, i, e) => ((Array)c).SetValue(e, i);
                getLength = c => ((Array)c).Length;
                elementType = t.GetElementType();
            }
            else if (t.GetGenericTypeDefinition() == typeof(List<>)
                || t.GetGenericTypeDefinition() == typeof(IList<>)
                || t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = t.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(elementType);
                var listConstructor = listType.GetConstructor(new Type[] { typeof(int) });
                var listAdd = listType.GetMethod("Add");
                var listCount = typeof(System.Linq.Enumerable).GetMethods().Where(n => n.Name == "Count" && n.GetParameters().Length == 1).First().MakeGenericMethod(elementType);
                var listSet = listType.GetProperties().Where(n => n.GetIndexParameters().Length > 0).First();
                ctor = (t, l) => listConstructor.Invoke(new object[] { l });
                setIndex = (c, i, e) => listAdd.Invoke(c, new object[] { e });
                getLength = c => (int)listCount.Invoke(null, new object[] { c });
            }
            else if (t.GetGenericTypeDefinition() == typeof(Stack<>))
            {
                elementType = t.GetGenericArguments()[0];
                var stackType = typeof(Stack<>).MakeGenericType(elementType);
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
                var stackConstructor = stackType.GetConstructor(new Type[] { typeof(int) });
                var stackPush = stackType.GetMethod("Push");

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
                    wrt.Write(len);

                    foreach (var item in (System.Collections.IEnumerable)obj)
                    {
                        elementMap.writer(wrt, item);
                    }
                }
            });
        }
    }

    public class BinaryWriterEx : BinaryWriter
    {
        public BinaryWriterEx(Stream stream)
            : base(stream)
        {

        }

        public void Write7BitEncoded(int value)
        {
            Write7BitEncodedInt(value);
        }
    }

    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(Stream stream)
              : base(stream)
        {

        }

        public int Read7BitEncoded()
        {
            return Read7BitEncodedInt();
        }
    }
}
