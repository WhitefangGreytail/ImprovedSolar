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
    [TargetType(typeof(SolarPowerPlantAI))]
    public class SolarPlantAIMod : PowerPlantAI
    {
        private static int[] solarOutput = new int[DataStore.ARRAY_LENGTH];


        // Need to do this to stop SolarPlant doing the default stuff.... then looping around here infinitely
        [RedirectMethod(true)]
        protected override void ProduceGoods(ushort buildingID, ref Building buildingData, ref Building.Frame frameData, int productionRate, int finalProductionRate,
                                             ref Citizen.BehaviourData behaviour, int aliveWorkerCount, int totalWorkerCount, int workPlaceCount, int aliveVisitorCount,
                                             int totalVisitorCount, int visitPlaceCount)
        {
//            float num = DataStore.batteryFactor + (1f - DataStore.batteryFactor) * Singleton<WeatherManager>.instance.SampleSunIntensity(buildingData.m_position, false);
            //finalProductionRate = Mathf.RoundToInt((float)finalProductionRate * num);
            // Above is the current way the system calculates it. It might over ride the below data ??

            // On look - buildingID!!!!
            // buildingData.m_position
            // If can use the position to get the district. Update once every six hours, the ground pollution rate
            byte pollution;
            Singleton<NaturalResourceManager>.instance.CheckPollution(buildingData.m_position, out pollution);
            SimulationManager simMan = Singleton<SimulationManager>.instance;
            int tenthHour = (int) (simMan.m_metaData.m_currentDayHour * 10);
            finalProductionRate = (int)((double)finalProductionRate * DataStore.thermalPowerCurve[tenthHour]);

//            Debugging.writeDebugToFile("Pollution data for " + buildingID + ": " + pollution);
//            Debugging.queueDebug("maint: " + this.m_maintenanceCost + ". Is night: " + simMan.m_isNightTime + ", " + tenthHour + " : " + productionRate);

            base.ProduceGoods(buildingID, ref buildingData, ref frameData, productionRate, finalProductionRate,
                              ref behaviour, aliveWorkerCount, totalWorkerCount, workPlaceCount, aliveVisitorCount,
                              totalVisitorCount, visitPlaceCount);
        }


        [RedirectMethod(true)]
        public override int GetElectricityRate(ushort buildingID, ref Building data)
        {
            int num = (int)data.m_productionRate;
            if ((data.m_flags & Building.Flags.Evacuating) != Building.Flags.None)
            {
                num = 0;
            }
            int budget = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
            num = PlayerBuildingAI.GetProductionRate(num, budget);
            //float num2 = DataStore.batteryFactor + (1f - DataStore.batteryFactor) * Singleton<WeatherManager>.instance.SampleSunIntensity(data.m_position, false);
            int tenthHour = (int)(Singleton<SimulationManager>.instance.m_metaData.m_currentDayHour * 10);
            num = Mathf.RoundToInt((float)num * DataStore.thermalPowerCurve[tenthHour]);  // THis number does it?
            int num3;
            int num4;
            this.GetElectricityProduction(out num3, out num4);
            return num4 * num / 100;
        }

    }
}


