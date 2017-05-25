using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericComparer
{
    class Comparer<T> : IEqualityComparer<T>
    {
        private string[] _propertiesToCompare;

        public Comparer()
        {
            _propertiesToCompare = typeof(T).GetProperties().Select(x => x.Name).ToArray();
        }

        public Comparer(Type type)
        {
            _propertiesToCompare = type.GetProperties().Select(x => x.Name).ToArray();
        }

        public Comparer(params string[] propertiesToCompare)
        {
            _propertiesToCompare = propertiesToCompare;
        }

        public bool Equals(T x, T y)
        {
            var type = x.GetType();
            bool result = true;
            foreach (var prop in _propertiesToCompare)
            {
                var property = type.GetProperty(prop);
                var xPropObj = property.GetValue(x);
                var yPropObj = property.GetValue(y);

                if (xPropObj == null || yPropObj == null)
                {
                    result &= (xPropObj == yPropObj);
                }
                else if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    var xProp = xPropObj.ToString();
                    var yProp = yPropObj.ToString();
                    result &= xProp == yProp;
                }
                else if (property.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
                {
                    var xList = ((IEnumerable<object>)xPropObj).ToList();
                    var yList = ((IEnumerable<object>)yPropObj).ToList();
                    result &= xList.SequenceEqual(yList, new Comparer<object>(property.PropertyType));
                }
                else if (property.PropertyType.IsClass)
                {
                    result &= new Comparer<object>(property.PropertyType).Equals(xPropObj, yPropObj);
                }

                if (!result) break;
            }

            return result;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
