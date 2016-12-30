using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework.Plugins;
using System.Text;

namespace WG_ConcentratedSolarThermal
{
    class Debugging
    {
        static StringBuilder sb = new StringBuilder(1000000);

        // Write to specified file
        public static void writeDebugToFile(String text, String fileName)
        {
            using (FileStream fs = new FileStream(ColossalFramework.IO.DataLocation.localApplicationData + Path.DirectorySeparatorChar + "WG_Solar.log", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(text);
            }
        }

        // Write to WG log file
        public static void writeDebugToFile(String text)
        {
            writeDebugToFile(text, "WG_Solar.log");
        }

        // Write a message to the panel
        public static void panelMessage(string text)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "WG_ImprovedSolar: " + text);
        }


        // Write a warning to the panel
        public static void panelWarning(string text)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "WG_ImprovedSolar: " + text);
        }


        // Write an error to the panel
        public static void panelError(string text)
        {
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, "WG_ImprovedSolar: " + text);
        }


        public static void queueDebug(string p)
        {
            sb.Append(p + "\n");
        }

        public static void flushDebug()
        {
            writeDebugToFile(sb.ToString(), "WG_Solar.log");
        }
    }
}
