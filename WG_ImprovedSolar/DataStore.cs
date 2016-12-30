using System;
using System.Collections.Generic;
using System.IO;

namespace WG_ConcentratedSolarThermal
{
    public class DataStore
    {
        public const int ARRAY_LENGTH = 240;

        public static float batteryFactor = 0.2f;
        public static double mean = 125.0;
        public static double std_dev = 20.0;
        public static double norm_mult = 1.0;
        public static double start = 0.9;

        public static bool enableExperimental = false;


        public static float[] thermalPowerCurve = new float[ARRAY_LENGTH];
        public static float[] solarIntensity = new float[ARRAY_LENGTH];

        /// <summary>
        /// Should be called once the XML is read in to calculate the values
        /// </summary>
        public static void calculateSolarCurve()
        {
            // Hard code 240 because of the 240 tenths of hours
            double gradient = 240.0 / norm_mult;
            double sun_hours = 10.0 * (SimulationManager.SUNSET_HOUR - SimulationManager.SUNRISE_HOUR);
            double sunrise = 10.0 * SimulationManager.SUNRISE_HOUR;
            double freq = Math.PI / sun_hours; // Was 2 * pi / 2 * sun_hours
            double divider = std_dev * Math.Sqrt(2);

            // Solar intensity
            for (int i = 0; i < DataStore.ARRAY_LENGTH; i++)
            {
                // Approxmiation of the graph atthe bottom of http://earthobservatory.nasa.gov/Features/EnergyBalance/page2.php
                solarIntensity[i] = (float)Math.Max(Math.Sin((i - sunrise) * freq), 0);
            }

            // Normal distribution
            for (int i = 0; i < DataStore.ARRAY_LENGTH; i++)
            {
                // Negative sloping line
                double one = start - ((double)i / gradient);
                // The normal distribution
                double two = norm_mult * (0.5 * (1.0 + Erf((i - mean) / divider)));
                thermalPowerCurve[i] = (float)(one + two);
            }
        }

        // The below has been taken from the page http://www.johndcook.com/blog/csharp_erf/
        //---------------------------------------------------------------------
        // A&S refers to Handbook of Mathematical Functions by Abramowitz and Stegun.
        // See Stand-alone error function for details of the algorithm.
        // This code is in the public domain. Do whatever you want with it, no strings attached.
        //---------------------------------------------------------------------
        private static double Erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        } // end erf
    } // end DataStore
}
