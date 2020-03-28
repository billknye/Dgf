using System;
using System.Collections.Generic;
using System.IO;
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
            if (member.PropertyType == typeof(string))
            {
                mapString(member);
            }
            else if (member.PropertyType == typeof(int))
            {
                mapInt(member);
            }
            else if (member.PropertyType == typeof(byte))
            {
                mapByte(member);
            }
            else if (member.PropertyType == typeof(DateTime))
            {
                mapDateTime(member);
            }
            else if (member.PropertyType.BaseType == typeof(Array))
            {
                var itemType = member.PropertyType.GetElementType();

                reads.Add((rdr, obj) =>
                {
                    var exist = rdr.ReadBoolean();
                    if (exist)
                    {

                        var len = rdr.ReadInt32();
                        var ar = Array.CreateInstance(itemType, len);

                        for (int i = 0; i < len; i++)
                        {
                            var item = Activator.CreateInstance(itemType) as IMappedObject;
                            item.Read(rdr);
                            ar.SetValue(item, i);
                        }

                        member.SetValue(obj, ar);
                    }
                });

                writes.Add((wrt, obj) =>
                {
                    Array ar = (Array)member.GetValue(obj);

                    if (ar == null)
                    {
                        wrt.Write(false);
                    }
                    else
                    {
                        wrt.Write(true);
                        wrt.Write(ar.Length);

                        for (int i = 0; i < ar.Length; i++)
                        {
                            if (ar.GetValue(i) is IMappedObject child)
                            {
                                child.Write(wrt);
                            }
                        }
                    }
                });
            }
            else
            {
                var i = member.PropertyType.GetInterface("IMappedObject");
                if (i != null)
                {
                    mapObject(member);
                }
                else
                {
                    if (member.PropertyType.IsEnum)
                    {
                        var underlying = Enum.GetUnderlyingType(member.PropertyType);
                        if (underlying == typeof(int))
                        {
                            mapInt(member);
                        }
                        else if (underlying == typeof(byte))
                        {
                            mapByte(member);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }
        }

        private void mapInt(PropertyInfo member)
        {
            reads.Add((rdr, obj) => member.SetValue(obj, rdr.ReadInt32()));
            writes.Add((wrt, obj) => wrt.Write((int)member.GetValue(obj)));
        }

        private void mapDateTime(PropertyInfo member)
        {
            reads.Add((rdr, obj) => member.SetValue(obj, new DateTime(rdr.ReadInt64())));
            writes.Add((wrt, obj) => wrt.Write(((DateTime)member.GetValue(obj)).Ticks));
        }

        private void mapString(PropertyInfo member)
        {
            reads.Add((rdr, obj) =>
            {
                member.SetValue(obj, rdr.ReadString());
            });

            writes.Add((wrt, obj) =>
            {
                wrt.Write(member.GetValue(obj) as string);
            });
        }

        private void mapByte(PropertyInfo member)
        {
            reads.Add((rdr, obj) =>
            {
                member.SetValue(obj, rdr.ReadByte());
            });

            writes.Add((wrt, obj) =>
            {
                wrt.Write((byte)member.GetValue(obj));
            });
        }

        private void mapBool(PropertyInfo member)
        {
            reads.Add((rdr, obj) =>
            {
                member.SetValue(obj, rdr.ReadBoolean());
            });

            writes.Add((wrt, obj) =>
            {
                wrt.Write((bool)member.GetValue(obj));
            });
        }

        private void mapObject(PropertyInfo member)
        {
            Func<IMappedObject> ctor = null;
            if (member.PropertyType.IsClass)
            {
                var a = member.PropertyType.GetConstructor(Type.EmptyTypes);
                if (a == null)
                    throw new InvalidOperationException();

                ctor = () => a.Invoke(null) as IMappedObject;
            }
            else
            {
                //ctor = () => Activator.CreateInstance(member.PropertyType) as IMappedObject;
                throw new NotSupportedException();
            }

            reads.Add((rdr, obj) =>
            {
                var exist = rdr.ReadBoolean();
                if (exist)
                {
                    var created = ctor();
                    created.Read(rdr);
                    member.SetValue(obj, created);
                }
            });

            writes.Add((wrt, obj) =>
            {
                var typed = member.GetValue(obj) as IMappedObject;
                if (typed != null)
                {
                    wrt.Write(true);
                    typed.Write(wrt);
                }
                else
                {
                    wrt.Write(false);
                }
            });
        }
    }
}
