using System;
using System.Configuration;
using System.Xml;

namespace kolbasik.NXmlConfiguration
{
    public static class XmlConfiguration
    {
        public static TConfig FromAppConfig<TConfig>(string sectionName, Func<Type, object> resolve)
            where TConfig : class
        {
            var section = ConfigurationManager.GetSection(sectionName) as XmlElement;
            if (section == null)
            {
                throw new ConfigurationErrorsException($"Could not find the '{sectionName}' section in app.config.");
            }
            return FromXml<TConfig>(section, resolve);
        }

        public static TConfig FromXml<TConfig>(string xml, Func<Type, object> resolve)
            where TConfig : class
        {
            return FromXml<TConfig>(ToXmlElement(xml), resolve);
        }

        public static TConfig FromXml<TConfig>(XmlElement xml, Func<Type, object> resolve)
            where TConfig : class
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));
            if (resolve == null)
                throw new ArgumentNullException(nameof(resolve));

            var config = XmlConfigurator.CreateInstance(xml, resolve) ?? Activator.CreateInstance(typeof(TConfig));
            if (config != null)
            {
                XmlConfigurator.SetProperties(xml, config);
            }
            return config as TConfig;
        }

        public static XmlElement ToXmlElement(string xml)
        {
            var xmlRoot = new XmlDocument();
            xmlRoot.LoadXml(xml);
            return xmlRoot.FirstChild as XmlElement;
        }
    }
}