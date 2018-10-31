using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentScienceCollectorAI
{
    class ScienceAIExperiment
    {
        [Persistent]
        public String experimentID = String.Empty;
        [Persistent]
        public bool inoperable = true;
        [Persistent]
        public bool rerunnable = false;
        [Persistent]
        public bool resettable = false;
        [Persistent]
        public bool resettableOnEVA = false;
        [Persistent]
        public float xmitDataScalar = 0f;
        [Persistent]
        public uint persistentId = 0;

        public ScienceAIExperiment(){}

        public ScienceAIExperiment(String id)
        {
            experimentID = id;
            inoperable = false;
            rerunnable = true;
            resettable = true;
            resettableOnEVA = true;
        }

        public ScienceAIExperiment(ModuleScienceExperiment experiment)
        {
            experimentID = experiment.experimentID;
            inoperable = experiment.Inoperable;
            rerunnable = experiment.rerunnable;
            resettable = experiment.resettable;
            resettableOnEVA = experiment.resettableOnEVA;
            xmitDataScalar = experiment.xmitDataScalar;
            persistentId = experiment.part.persistentId;
        }

        public void SaveExperiment(Vessel vessel)
        {
            if (persistentId != 0)
            {
                ModuleScienceExperiment mod = vessel.FindPartModulesImplementing<ModuleScienceExperiment>()
                        .Where(m => (m.part.persistentId == persistentId) && (m.experimentID == experimentID))
                        .First();
                mod.Inoperable = inoperable;
                if (inoperable)
                {
                    mod.Deployed = true;
                    mod.SetInoperable();
                    mod.DumpData(null);
                }
                else
                {
                    mod.DumpData(null);
                    mod.ResetExperiment();
                }
            }
        }
    }
}
