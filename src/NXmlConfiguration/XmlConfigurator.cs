using System;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace kolbasik.NXmlConfiguration
{
    public static class XmlConfigurator
    {
        public static object CreateInstance(XmlElement xmlElement, Func<Type, object> resolve)
        {
            object instance = null;
            var type = Type.GetType(xmlElement.GetAttribute(@"type"), false, true);
            if (type != null)
            {
                instance = resolve(type);
            }
            return instance;
        }

        public static void SetProperties(XmlElement xmlElement, object instance)
        {
            foreach (var property in instance.GetType().GetProperties().Where(x => x.CanWrite))
            {
                var propertyName = property.Name;
                propertyName = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
                if (property.PropertyType.IsArray)
                {
                    string[] options = null;
                    if (xmlElement.HasAttribute(propertyName))
                    {
                        options = xmlElement.GetAttribute(propertyName).Split(',', ';', ' ').ToArray();
                    }
                    else
                    {
                        var xmlChild = xmlElement.SelectSingleNode("./" + propertyName) as XmlElement;
                        if (xmlChild != null)
                        {
                            if (xmlChild.HasAttribute(@"value"))
                            {
                                options = xmlChild.GetAttribute(@"value").Split(',', ';', ' ').ToArray();
                            }
                            else
                            {
                                options = xmlChild.SelectNodes("item/@value").OfType<XmlAttribute>().Select(x => x.Value).ToArray();
                            }
                        }
                    }
                    if (options != null)
                    {
                        var elementType = property.PropertyType.GetElementType();
                        var converter = TypeDescriptor.GetConverter(elementType);
                        var objects = options.Select(converter.ConvertFromInvariantString).ToArray();
                        var values = Array.CreateInstance(elementType, objects.Length);
                        Array.Copy(objects, values, objects.Length);
                        property.SetValue(instance, values);
                    }
                }
                else
                {
                    string option = null;
                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    if (xmlElement.HasAttribute(propertyName))
                    {
                        option = xmlElement.GetAttribute(propertyName);
                    }
                    else
                    {
                        var xmlChild = xmlElement.SelectSingleNode("./" + propertyName) as XmlElement;
                        if (xmlChild != null)
                        {
                            if (xmlChild.HasAttribute(@"value"))
                            {
                                option = xmlChild.GetAttribute(@"value");
                            }
                        }
                    }
                    if (option != null)
                    {
                        var value = converter.ConvertFromInvariantString(option);
                        property.SetValue(instance, value);
                    }
                }
            }
        }
    }
}