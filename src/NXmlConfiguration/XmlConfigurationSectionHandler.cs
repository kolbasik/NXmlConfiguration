using System.Configuration;
using System.Xml;

namespace kolbasik.NXmlConfiguration
{
    public sealed class XmlConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            return section;
        }
    }
}