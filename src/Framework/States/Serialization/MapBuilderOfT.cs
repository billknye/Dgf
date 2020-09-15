using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dgf.Framework.States.Serialization
{
    /// <summary>
    /// Unused generic based builder 
    /// </summary>
    public class MapBuilder<T> : MapBuilder
    {
        public MapBuilder(bool autoMapProperties = true)
            : base(typeof(T), autoMapProperties)
        {

        }

        public void AddMap<P>(Expression<Func<T, P>> expression)
        {
            var unary = expression.Body as MemberExpression;
            if (unary.Member is PropertyInfo member)
            {
                AddPropertyMap(member);
            }
            else
            {
                throw new ArgumentException();
            }
        }

    }
}
