using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace MercPlusLibrary
{
    public static class ObjectCopy<T>
    {
        /// <summary>
        ///Method: DeepCopy
        ///Functionality: Makes an Deep copy of an type
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public static List<T> DeepCopyByObjectCreation(List<T> objList)
        {
            List<T> newList = new List<T>();
            try
            {
                if (objList == null) return objList;

                T newObj;
                foreach (T obj in objList)
                {
                    PropertyInfo[] propertyList = obj.GetType().GetProperties();
                    Object[] tmpObj = new Object[propertyList.Length];
                    newObj = (T)Activator.CreateInstance(obj.GetType(), false);
                    for (int i = 0; i < propertyList.Count(); i++)
                    {
                        try
                        {
                            tmpObj[i] = obj.GetType().InvokeMember(propertyList[i].Name,
                                    BindingFlags.GetProperty, null, obj, new object[0]);
                            if (tmpObj[i] != null || (tmpObj[i] != null && !String.IsNullOrEmpty(tmpObj[i].ToString())))
                            {
                                tmpObj[i] = newObj.GetType().InvokeMember(propertyList[i].Name,
                                BindingFlags.SetProperty, null, newObj, new Object[] { tmpObj[i] });

                            }
                        }
                        catch (Exception ex)
                        {
                            //TRKPlusLogger.Log(ex.Message, ex.StackTrace, LoggingCategory.Error);
                            continue;
                        }
                    }
                    newList.Add(newObj);
                }
                return newList;

            }
            catch (Exception ex)
            {
                //TRKPlusLogger.Log(ex.Message, ex.StackTrace, LoggingCategory.Error);
                return newList;
            }

        }

        /// <summary>
        ///Method: DeepCopy
        ///Functionality: Makes an Deep copy of an type
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public static T DeepCopyByObjectCreation(T obj)
        {
            T newObj = default(T);
            try
            {
                if (obj == null) return obj;

                PropertyInfo[] propertyList = obj.GetType().GetProperties();
                Object[] tmpObj = new Object[propertyList.Length];
                newObj = (T)Activator.CreateInstance(obj.GetType(), false);
                for (int i = 0; i < propertyList.Count(); i++)
                {
                    try
                    {
                        tmpObj[i] = obj.GetType().InvokeMember(propertyList[i].Name,
                                BindingFlags.GetProperty, null, obj, new object[0]);
                        if (tmpObj[i] != null || (tmpObj[i] != null && !String.IsNullOrEmpty(tmpObj[i].ToString())))
                        {
                            tmpObj[i] = newObj.GetType().InvokeMember(propertyList[i].Name,
                            BindingFlags.SetProperty, null, newObj, new Object[] { tmpObj[i] });

                        }
                    }
                    catch (Exception ex)
                    {
                        //TRKPlusLogger.Log(ex.Message, ex.StackTrace, LoggingCategory.Error);
                        //continue;
                    }
                }

                return newObj;

            }
            catch (Exception ex)
            {
                //TRKPlusLogger.Log(ex.Message, ex.StackTrace, LoggingCategory.Error);
                return newObj;
            }

        }

        /// <summary>
        ///Method: DeepCopyEntireObject
        ///Functionality: Makes an Deep copy of a type, entire object will be deep copied along with the inner objects like lists, structure, arrays etc.
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public static List<T> DeepCopyEntireObject(List<T> objList)
        {
            List<T> newList = new List<T>();
            try
            {
                if (objList == null)
                    throw new ArgumentNullException("Object list cannot be null");
                foreach (T obj in objList)
                {
                    newList.Add((T)Process(obj));
                }
                return newList;
            }
            catch (Exception ex)
            {
                //TRKPlusLogger.Log(ex.Message, ex.StackTrace, LoggingCategory.Error);
                return newList;
            }
        }
        /// <summary>
        ///Method: DeepCopyEntireObject
        ///Functionality: Makes an Deep copy of a type, entire object will be deep copied along with the inner objects like lists, structure, arrays etc.
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        public static T DeepCopyEntireObject(T obj)
        {
            try
            {
                if (obj == null)
                    throw new ArgumentNullException("Object cannot be null");
                return (T)Process(obj);
            }
            catch (Exception ex)
            {
                //TRKPlusLogger.Log(ex.Message, ex.StackTrace, LoggingCategory.Error);
                T newObj = default(T);
                return newObj;
            }
        }

        private static object Process(object obj)
        {
            if (obj == null)
                return null;
            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }
            else if (type.IsArray)
            {
                Type elementType = Type.GetType(type.AssemblyQualifiedName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                //Prithwi: Convert.ChangeType would change the type those implement IConvertible interface and unfortunately we cann't assume here the same for all type.
                //return Convert.ChangeType(copied, obj.GetType());
                return (object)copied;
            }
            else if (type.IsClass)
            {
                //if type is a propertyChangedEventHandler the  return null
                if (type.AssemblyQualifiedName.Contains("PropertyChangedEventHandler"))
                    return null;

                //Prithwi: code modified
                //Get the parameterless ctor then invode the the ctor to create the instance instead of calling Activator.CreateInstance(type, true)
                //object toret = Activator.CreateInstance(type, true);
                object toret = null;
                var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                var ctor = type.GetConstructor(flags, null, new Type[0], null);
                if (ctor != null)
                {
                    toret = ctor.Invoke(null);
                }
                else
                {
                    //TRKPlusLogger.Log("No parameterless constructor found in type " + type.AssemblyQualifiedName, "", LoggingCategory.Warning);
                    return toret;
                }
                //end of code modification

                //Prithwi: You can't access the private fields of base type using the type of subclass because those fields don't exist in subclass
                //So, need to retrive field info via other means (such as getting the base class from the type of subclass by using recursion)
                List<FieldInfo> fields = new List<FieldInfo>();
                FindFields(fields, type);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, Process(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }

        private static void FindFields(ICollection<FieldInfo> fields, Type t)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in t.GetFields(flags))
            {
                // Ignore inherited fields.
                if (field.DeclaringType == t)
                    fields.Add(field);
            }

            var baseType = t.BaseType;
            if (baseType != null)
                FindFields(fields, baseType);
        }
    }
}
