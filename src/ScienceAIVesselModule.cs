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
        public bool active = false;
        [Persistent]
        public bool collectEmpty = false;
        [Persistent]
        public bool collectReusableOnly = false;
        [Persistent]
        public String biome = string.Empty;
        [Persistent]
        public String situation = string.Empty;
        [Persistent]
        public List<ScienceAIExperiment> modules = new List<ScienceAIExperiment>();
        [Persistent]
        public List<ScienceAIData> data = new List<ScienceAIData>();

        protected ModuleScienceContainer scienceContainer = null;
        protected ScienceAIContainerModule AIModule = null;

        public ModuleScienceContainer ScienceContainer
        {
            get
            {
                return scienceContainer;
            }
        }

        public bool AddScienceResult(ScienceAIData scienceResult)
        {
            if (vessel.loaded)
            {
                ScienceData result = new ScienceData(scienceResult.amount, scienceResult.xmitValue, scienceResult.xmitBonus, scienceResult.id, scienceResult.dataName);
                if (!scienceContainer.HasData(result))
                {
                    scienceContainer.AddData(result);
                    return true;
                }
                return false;
            }
            data.Add(scienceResult);
            return true;
        }

        private void AddScienceResultsToVessel()
        {
            foreach(ScienceAIData scienceResult in data)
            {
                ScienceData result = new ScienceData(scienceResult.amount, scienceResult.xmitValue, scienceResult.xmitBonus, scienceResult.id, scienceResult.dataName);
                if (!scienceContainer.HasData(result))
                {
                    scienceContainer.AddData(result);
                }
            }
            data.Clear();
        }

        protected override void OnStart()
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            if (vessel.vesselType == VesselType.Unknown
                || vessel.vesselType == VesselType.SpaceObject
                || vessel.vesselType == VesselType.Flag
                || vessel.vesselType == VesselType.EVA
                || vessel.vesselType == VesselType.Debris)
            {
                return;
            }

            if (vessel.loaded)
            {
                Debug.Log("OnStart " + vessel.name);
                LoadFromVessel();
                SaveToVessel();
                biome = String.Empty;
                situation = String.Empty;
            }
        }

        private void OnVesselsUndocking(Vessel data0, Vessel data1)
        {
            Debug.Log("OnVesselsUndocking " + data0.name + " " + data1.name);
        }

        private void OnUndock(EventReport data)
        {
            Debug.Log("OnUndock " + data.origin.vessel.name);
        }

        private void OnSameVesselUndock(GameEvents.FromToAction<ModuleDockingNode, ModuleDockingNode> data)
        {
            Debug.Log("OnSameVesselUndock " + data.from.vessel.name);
        }

        private void OnPartUndock(Part data)
        {
            Debug.Log("OnPartUndock " + data.vessel.name);
        }

        public void SaveToVessel()
        {
            List<ScienceAIContainerModule> aiMods = vessel.FindPartModulesImplementing<ScienceAIContainerModule>();
            List<ScienceAIContainerModule> activeAiMods = aiMods.FindAll(m => m.IsAutoCollect == true);
            if (activeAiMods.Count() > 1)
            {
                foreach(ScienceAIContainerModule mod in activeAiMods)
                {
                    if(mod != activeAiMods.First())
                    {
                        mod.IsAutoCollect = false;
                    }
                }
            }
            foreach (ScienceAIContainerModule aiMod in aiMods)
            {
                if (activeAiMods.Count() == 0)
                {
                    if(active && aiMod == aiMods.First())
                    {
                        aiMod.IsAutoCollect = true;
                    }
                    aiMod.isEnabled = true;
                    aiMod.part.FindModuleImplementing<ModuleScienceContainer>().isEnabled = true;
                }
                else
                {
                    aiMod.isEnabled = (aiMod == activeAiMods.First());
                    aiMod.part.FindModuleImplementing<ModuleScienceContainer>().isEnabled = aiMod.isEnabled;
                }
            }

            foreach (ScienceAIExperiment aiExperiment in modules)
            {
                if (aiExperiment.moduleRef != null && aiExperiment.inoperable)
                {
                    aiExperiment.moduleRef.SetInoperable();
                    aiExperiment.moduleRef.Deployed = true;
                }else if(aiExperiment.moduleRef != null)
                {
                    aiExperiment.moduleRef.Inoperable = false;
                    aiExperiment.moduleRef.Deployed = false;
                }
            }
            foreach (ModuleScienceExperiment experiment in vessel.FindPartModulesImplementing<ModuleScienceExperiment>())
            {
                //if(!experiment.Inoperable)
                experiment.isEnabled = !active;
                if (experiment.Inoperable)
                {
                    experiment.DumpData(null);
                    //Debug.Log(experiment.experimentID);
                }
                else
                {
                    experiment.DumpData(null);
                    experiment.ResetExperiment();
                }
            }

            AddScienceResultsToVessel();
        }

        public void LoadFromVessel()
        {
            try
            {
                AIModule = vessel.FindPartModulesImplementing<ScienceAIContainerModule>().Where(m => m.IsAutoCollect == true).First();
                active = false;
                if (AIModule != null)
                {
                    active = true;
                }
            }catch(InvalidOperationException e)
            {
                AIModule = null;
                scienceContainer = null;
                active = false;
            }

            modules.Clear();
            if (active)
            {
                collectEmpty = AIModule.IsCollectEmpty;
                collectReusableOnly = AIModule.IsReusableOnly;
                scienceContainer = AIModule.part.FindModuleImplementing<ModuleScienceContainer>();
                scienceContainer.CollectAllEvent();
                bool hasScientist = Vessel.GetVesselCrew().FindAll(pcm => pcm.trait == "Scientist").Count() > 0;
                foreach (ModuleScienceExperiment experiment in vessel.FindPartModulesImplementing<ModuleScienceExperiment>())
                {
                    
                        ScienceAIExperiment module = new ScienceAIExperiment
                        {
                            partId = experiment.part.persistentId,
                            experimentID = experiment.experimentID,
                            enabled = experiment.enabled,
                            inoperable = experiment.Inoperable,
                            resettable = experiment.resettable,
                            resettableOnEVA = experiment.resettableOnEVA,
                            xmitDataScalar = experiment.xmitDataScalar,
                            rerunnable = experiment.rerunnable,
                            moduleRef = experiment
                        };
                        modules.Add(module);
                }

                if(Vessel.GetVesselCrew().Count() > 0)
                {
                    ScienceAIExperiment module = new ScienceAIExperiment
                    {
                        partId = 0,
                        experimentID = "evaReport",
                        enabled = true,
                        inoperable = false,
                        resettable = true,
                        resettableOnEVA = true,
                        xmitDataScalar = 1,
                        rerunnable = true,
                        moduleRef = null
                    };
                    modules.Add(module);

                    ScienceAIExperiment surfaceModule = new ScienceAIExperiment
                    {
                        partId = 0,
                        experimentID = "surfaceSample",
                        enabled = true,
                        inoperable = false,
                        resettable = true,
                        resettableOnEVA = true,
                        xmitDataScalar = 1,
                        rerunnable = true,
                        moduleRef = null
                    };
                    modules.Add(surfaceModule);
                }
            }
        }

        protected override void OnLoad(ConfigNode node)
        {
            ConfigNode.LoadObjectFromConfig(this, node.GetNode(GetType().FullName));
        }

        protected override void OnSave(ConfigNode node)
        {
            ConfigNode currentNode = ConfigNode.CreateConfigFromObject(this);
            node.AddNode(currentNode);
        }
    }
}
