using System;
using System.Xml;

namespace WG_ConcentratedSolarThermal
{
    public abstract class WG_XMLBaseVersion
    {
        public WG_XMLBaseVersion()
        {
        }

        public abstract void readXML(XmlDocument doc);
        public abstract bool writeXML(string fullPathFileName);
    }
}