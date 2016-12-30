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
using System.Diagnostics;
using Boformer.Redirection;

namespace WG_ConcentratedSolarThermal
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public const String XML_FILE = "WG_RealisticSolar.xml";

        private readonly Dictionary<MethodInfo, Redirector> redirectsOnLoaded = new Dictionary<MethodInfo, Redirector>();
        private readonly Dictionary<MethodInfo, Redirector> redirectsOnCreated = new Dictionary<MethodInfo, Redirector>();

        private string currentFileLocation = "";
        private static volatile bool isModEnabled = false;
        private static volatile bool isLevelLoaded = false;
        private static Stopwatch sw;

        public override void OnCreated(ILoading loading)
        {
            if (!isModEnabled)
            {
                isModEnabled = true;
                sw = Stopwatch.StartNew();
                Redirect(true);

                readFromXML();


                sw.Stop();
                UnityEngine.Debug.Log("WG_ImprovedSolar: Successfully loaded in " + sw.ElapsedMilliseconds + " ms.");
            }
        }

        public override void OnReleased()
        {
            if (isModEnabled)
            {
                isModEnabled = false;
                //Debugging.flushDebug();

                try
                {
                    WG_XMLBaseVersion xml = new XML_VersionOne();
                    xml.writeXML(currentFileLocation);
                }
                catch (Exception e)
                {
                    Debugging.panelMessage(e.Message);
                }

                RevertRedirect(true);
            }
        }


        public override void OnLevelUnloading()
        {
            if (isLevelLoaded)
            {
                isLevelLoaded = false;
            }
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            {
                if (!isLevelLoaded)
                {
                    isLevelLoaded = true;
                    DataStore.calculateSolarCurve();
                    Debugging.panelMessage("Successfully loaded in " + sw.ElapsedMilliseconds + " ms.");
                }
            }
        }


        private void Redirect(bool onCreated)
        {
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            redirects.Clear();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                try
                {
                    var r = RedirectionUtil.RedirectType(type, onCreated);
                    if (r != null)
                    {
                        foreach (var pair in r)
                        {
                            redirects.Add(pair.Key, pair.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"An error occured while applying {type.Name} redirects!");
                    UnityEngine.Debug.Log(e.StackTrace);
                }
            }
        }

        private void RevertRedirect(bool onCreated)
        {
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            foreach (var kvp in redirects)
            {
                try
                {
                    kvp.Value.Revert();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"An error occured while reverting {kvp.Key.Name} redirect!");
                    UnityEngine.Debug.Log(e.StackTrace);
                }
            }
            redirects.Clear();
        }
// TODO - New

        /// <summary>
        ///
        /// </summary>
        private void readFromXML()
        {
            currentFileLocation = ColossalFramework.IO.DataLocation.localApplicationData + Path.DirectorySeparatorChar + XML_FILE;

            if (File.Exists(currentFileLocation))
            {
                // Load in from XML - Designed to be flat file for ease
                XML_VersionOne reader = new XML_VersionOne();
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(currentFileLocation);
                    reader.readXML(doc);
                }
                catch (Exception e)
                {
                    // Game will now use defaults
                    Debugging.panelMessage(e.Message);
                }
            }
            else
            {
                Debugging.panelMessage("Configuration file not found. Will output new file to : " + currentFileLocation);
            }
        }
    }
}
