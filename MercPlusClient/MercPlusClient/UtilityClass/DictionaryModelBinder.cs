using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MercPlusClient.UtilityClass
{
    public class DictionaryModelBinder : DefaultModelBinder
    {
        private static bool IsGenericDictionary(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>));
        }

        private void AddItemsToDictionary(IDictionary dictionary, Type dictionaryType, string modelName, ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            List<string> keys = new List<string>();
            HttpRequestBase request = controllerContext.HttpContext.Request;
            keys.AddRange(((IDictionary<string, object>)controllerContext.RouteData.Values).Keys.Cast<string>());
            keys.AddRange(request.QueryString.Keys.Cast<string>());
            keys.AddRange(request.Form.Keys.Cast<string>());

            Type dictionaryValueType = dictionaryType.GetGenericArguments()[1];
            IModelBinder dictionaryValueBinder = Binders.GetBinder(dictionaryValueType);

            foreach (string key in keys)
            {
                string dictItemKey = null;
                string valueModelName = null;

                if (!key.Equals("area", StringComparison.InvariantCultureIgnoreCase)
                    && !key.Equals("controller", StringComparison.InvariantCultureIgnoreCase)
                    && !key.Equals("action", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (key.StartsWith(modelName + "[", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int endIndex = key.IndexOf("]", modelName.Length + 1);
                        if (endIndex != -1)
                        {
                            dictItemKey = key.Substring(modelName.Length + 1, endIndex - modelName.Length - 1);
                            valueModelName = key.Substring(0, endIndex + 1);
                        }
                    }
                    else
                    {
                        dictItemKey = valueModelName = key;
                    }

                    if (dictItemKey != null && valueModelName != null && !dictionary.Contains(dictItemKey))
                    {
                        object dictItemValue = dictionaryValueBinder.BindModel(controllerContext,
                            new ModelBindingContext(bindingContext)
                            {
                                ModelName = valueModelName,
                                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, dictionaryValueType)
                            });
                        if (dictItemValue != null)
                        {
                            dictionary.Add(dictItemKey, dictItemValue);
                        }
                    }
                }
            }
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object result = null;
            Type modelType = bindingContext.ModelType;
            string modelName = bindingContext.ModelName;

            if (IsGenericDictionary(modelType))
            {
                // The model itself is a generic dictionary.
                IDictionary dictionary = (IDictionary)CreateModel(controllerContext, bindingContext, modelType);
                AddItemsToDictionary(dictionary, modelType, modelName, controllerContext, bindingContext);
                result = dictionary;
            }
            else
            {
                // The model may contain properties that get or set generic dictionaries.
                result = base.BindModel(controllerContext, bindingContext);
                PropertyInfo[] properties = modelType.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    Type propertyType = property.PropertyType;
                    if (IsGenericDictionary(propertyType))
                    {
                        var dictionary = (IDictionary)Activator.CreateInstance(propertyType);
                        AddItemsToDictionary(dictionary, propertyType, modelName, controllerContext, bindingContext);
                        property.SetValue(result, dictionary, null);
                    }
                }
            }
            return result;
        }
    }
}
