using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;
using ColossalFramework;
using Boformer.Redirection;


namespace WG_ConcentratedSolarThermal
{
    [TargetType(typeof(WeatherManager))]
    public class WeatherManagerMod : WeatherManager
    {
        private static float NOON = SimulationManager.SUNSET_HOUR - SimulationManager.SUNRISE_HOUR;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [RedirectMethod(true)]
        private float GetSunIntensityFactor()
        {
            //float num2 = Mathf.Clamp01(0.5f + Mathf.Min(time - SimulationManager.SUNRISE_HOUR, SimulationManager.SUNSET_HOUR - time) * 2f);
            //float num2 = Mathf.Clamp01(0.5f + Mathf.Min(time - NOON, NOON - time) * 2f);  // TODO - Broken :(
            float time = Singleton<SimulationManager>.instance.m_dayTimeFrame * SimulationManager.DAYTIME_FRAME_TO_HOUR;
            return DataStore.solarIntensity[(int) (time * 10)] * (1f - (this.m_currentRain + this.m_currentFog) * 0.5f);  // Now can drop to 0!
        }
    }
}


