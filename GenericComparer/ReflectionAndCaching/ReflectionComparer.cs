using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Linq;

namespace CompareObjects
{
    public static class ReflectionEqualityComparer
    {
        public static bool Equality<T>(T firstObject, T secondObject)
        {
            bool equalityObjects = true;
            if (!ReferenceEquals(firstObject, null) && !ReferenceEquals(secondObject, null))
            {
                object value1, value2;
                PropertyInfo[] properties = firstObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo propertyInfo in properties)
                {
                    value1 = propertyInfo.GetValue(firstObject, null);
                    value2 = propertyInfo.GetValue(secondObject, null);

                    if (typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType) || propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType)
                    {
                        if (!EqualityValues(value1, value2))
                        {
                            equalityObjects = false;
                        }
                    }

                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        EqualityEnumerable(value1, value2);
                    }

                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!Equality(propertyInfo.GetValue(firstObject, null), (propertyInfo.GetValue(secondObject, null))))
                        {
                            equalityObjects = false;
                        }
                    }

                    else
                    {
                        equalityObjects = false;
                    }
                }
            }

            else
            {
                equalityObjects = false;
            }

            return equalityObjects;
        }

        private static bool EqualityValues(object value1, object value2)
        {
            IComparable selfValueComparer = value1 as IComparable;

            if (value1 == null && value2 != null || value1 != null && value2 == null)
                return false;
            else if (selfValueComparer != null && selfValueComparer.CompareTo(value2) != 0)
                return false;
            else if (!object.Equals(value1, value2))
                return false;

            return true;
        }

        private static bool EqualityEnumerable(object value1, object value2)
        {
            if (value1 == null && value2 != null || value1 != null && value2 == null)
            {
                return false;
            }

            else if (value1 != null && value2 != null)
            {
                IEnumerable<object> enumValue1, enumValue2;
                enumValue1 = ((IEnumerable)value1).Cast<object>();
                enumValue2 = ((IEnumerable)value2).Cast<object>();

                if (enumValue1.Count() != enumValue2.Count())
                {
                    return false;
                }

                else
                {
                    object enumValue1Item, enumValue2Item;
                    Type enumValue1ItemType;
                    for (int itemIndex = 0; itemIndex < enumValue1.Count(); itemIndex++)
                    {
                        enumValue1Item = enumValue1.ElementAt(itemIndex);
                        enumValue2Item = enumValue2.ElementAt(itemIndex);
                        enumValue1ItemType = enumValue1Item.GetType();
                        if (typeof(IComparable).IsAssignableFrom(enumValue1ItemType) || enumValue1ItemType.IsPrimitive || enumValue1ItemType.IsValueType)
                        {
                            if (!EqualityValues(enumValue1Item, enumValue2Item))
                                return false;
                        }
                        else if (!Equals(enumValue1Item, enumValue2Item))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}