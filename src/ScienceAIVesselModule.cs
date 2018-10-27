using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PersistentScienceCollectorAI
{
    class ScienceAIVesselModule : VesselModule
    {
        [Persistent]
        public bool active;
        [Persistent]
        public String biome = string.Empty;
        [Persistent]
        public List<ScienceAIExperiment> modules = new List<ScienceAIExperiment>();
        [Persistent]
        public List<ScienceAIData> data = new List<ScienceAIData>();

        protected override void OnStart()
        {
        }
    }
}
