using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using ICities;
using UnityEngine;
using ColossalFramework.Plugins;


namespace WG_ConcentratedSolarThermal
{
    public class XML_VersionOne : WG_XMLBaseVersion
    {
        private const string solarNodeName = "solar";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        public override void readXML(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            try
            {
                DataStore.enableExperimental = Convert.ToBoolean(root.Attributes["experimental"].InnerText);
            }
            catch (Exception)
            {
                DataStore.enableExperimental = false;
            }

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name.Equals(solarNodeName))
                {
                    readSolarNode(node);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPathFileName"></param>
        /// <returns></returns>
        public override bool writeXML(string fullPathFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode rootNode = xmlDoc.CreateElement("WG_Power");
            XmlAttribute attribute = xmlDoc.CreateAttribute("version");
            attribute.Value = "1";
            rootNode.Attributes.Append(attribute);
            /*
            attribute = xmlDoc.CreateAttribute("experimental");
            attribute.Value = DataStore.enableExperimental ? "true" : "false";
            rootNode.Attributes.Append(attribute);
            */
            xmlDoc.AppendChild(rootNode);

            try
            {
                makeSolarNode(rootNode, xmlDoc);
            }
            catch (Exception e)
            {
                Debugging.panelMessage(e.Message);
            }

            if (File.Exists(fullPathFileName))
            {
                try
                {
                    if (File.Exists(fullPathFileName + ".bak"))
                    {
                        File.Delete(fullPathFileName + ".bak");
                    }

                    File.Move(fullPathFileName, fullPathFileName + ".bak");
                }
                catch (Exception e)
                {
                    Debugging.panelMessage(e.Message);
                }
            }

            try
            {
                xmlDoc.Save(fullPathFileName);
            }
            catch (Exception e)
            {
                Debugging.panelMessage(e.Message);
                return false;  // Only time when we say there's an error
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xmlDoc"></param>
        private void makeSolarNode(XmlNode root, XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateElement("solar");

            XmlAttribute attribute = xmlDoc.CreateAttribute("mean");
            attribute.Value = Convert.ToString(DataStore.mean);
            node.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("std_dev");
            attribute.Value = Convert.ToString(DataStore.std_dev);
            node.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("normal_mult");
            attribute.Value = Convert.ToString(DataStore.norm_mult);
            node.Attributes.Append(attribute);

            attribute = xmlDoc.CreateAttribute("initial_start");
            attribute.Value = Convert.ToString(DataStore.start);
            node.Attributes.Append(attribute);

            root.AppendChild(node);
        }

                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void readSolarNode(XmlNode node)
        {
            try
            {
                double mean = Convert.ToDouble(node.Attributes["mean"].InnerText);
                double std_dev = Convert.ToDouble(node.Attributes["std_dev"].InnerText);
                double norm_mult = Convert.ToDouble(node.Attributes["normal_mult"].InnerText);
                double initial_start = Convert.ToDouble(node.Attributes["initial_start"].InnerText);

                DataStore.mean = mean;
                DataStore.std_dev = std_dev;
                DataStore.norm_mult = norm_mult;
                DataStore.start = initial_start;
            }
            catch (Exception e)
            {
                Debugging.panelMessage("readSolarNode: " + e.Message);
            }
            finally
            {

                // Make the mean between 70 and 165
                if (DataStore.mean < 70.0)
                {
                    // Prevent errors
                    DataStore.mean = 70.0;
                }
                else if (DataStore.mean < 165.0)
                {
                    // Prevent errors
                    DataStore.mean = 165.0;
                }

                if (DataStore.std_dev < 0.1)
                {
                    // Prevent errors
                    DataStore.std_dev = 0.1;
                }

                if (DataStore.norm_mult < 0.01)
                {
                    // Prevent errors
                    DataStore.norm_mult = 0.01;
                }
            }
        }
    }
}