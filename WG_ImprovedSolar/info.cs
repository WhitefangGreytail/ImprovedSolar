using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;


namespace WG_ConcentratedSolarThermal
{
    public class ConcentratedSolarThermal : IUserMod
    {
        public string Name
        {
            get { return "WG Realistic Solar v1.3"; }
        }
        public string Description
        {
            get { return "Models solar plants as concentrated solar thermal with a bit of realism"; }
        }
    }
}


