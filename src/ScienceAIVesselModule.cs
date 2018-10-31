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
        public List<ScienceAIData> results = new List<ScienceAIData>();
        [Persistent]
        public List<ScienceAIExperiment> experiments = new List<ScienceAIExperiment>();
        [Persistent]
        public uint partID;
        [Persistent]
        public bool collectEmpty;
        [Persistent]
        public bool reusableOnly;

        private ModuleScienceContainer container = null;

        protected override void OnStart()
        {
            if (vessel.loaded && active)
            {
                ScienceAIContainerModule mod = vessel.FindPartModulesImplementing<ScienceAIContainerModule>().Where(m => m.IsAutoCollect == true).First();
                container = mod.part.FindModuleImplementing<ModuleScienceContainer>();
                partID = mod.part.persistentId;
            }
        }

        public void Activate()
        {
            active = true;
            bool alreadyCollect = false;
            foreach (ScienceAIContainerModule mod in vessel.FindPartModulesImplementing<ScienceAIContainerModule>())
            {
                if (mod.IsAutoCollect && !alreadyCollect)
                {
                    alreadyCollect = true;
                    container = mod.part.FindModuleImplementing<ModuleScienceContainer>();
                    partID = mod.part.persistentId;
                }
                else if (mod.IsAutoCollect && alreadyCollect)
                {
                    mod.IsAutoCollect = false;
                    mod.isEnabled = false;
                }
                else
                {
                    mod.isEnabled = false;
                }
            }
            LoadScienceData();
            LoadScienceExperiments();
            SetScienceContainerStatus(false);
            SetScienceModuleStatus(false);
        }

        public void Deactivate()
        {
            active = false;
            foreach (ScienceAIContainerModule mod in vessel.FindPartModulesImplementing<ScienceAIContainerModule>())
            {
                mod.IsAutoCollect = false;
                mod.isEnabled = true;
            }
            SetScienceContainerStatus(true);
            SaveScienceData();
            SaveScienceExperiments();
            SetScienceModuleStatus(true);
        }

        private void SetScienceContainerStatus(bool enable)
        {
            foreach (ModuleScienceContainer con in vessel.FindPartModulesImplementing<ModuleScienceContainer>())
            {
                con.isEnabled = enable;
            }
        }

        private void SetScienceModuleStatus(bool enable)
        {
            foreach(ModuleScienceExperiment exp in vessel.FindPartModulesImplementing<ModuleScienceExperiment>())
            {
                exp.isEnabled = enable;
            }
        }

        private void LoadScienceExperiments()
        {
            experiments.Clear();
            foreach(ModuleScienceExperiment experiment in vessel.FindPartModulesImplementing<ModuleScienceExperiment>())
            {
                ScienceAIExperiment modExperiment = new ScienceAIExperiment(experiment);
                experiments.Add(modExperiment);
            }
            experiments.Add(new ScienceAIExperiment("evaReport"));
            experiments.Add(new ScienceAIExperiment("surfaceSample"));
        }

        private void SaveScienceExperiments()
        {
            foreach(ScienceAIExperiment experiment in experiments)
            {
                experiment.SaveExperiment(vessel);
            }
        }

        private void LoadScienceData()
        {
            results.Clear();
            foreach(ScienceData result in container.GetData())
            {
                results.Add(new ScienceAIData(result));
            }
        }

        private void SaveScienceData()
        {
            foreach(ScienceAIData result in results)
            {
                ScienceData scienceResult = result.CreateScienceData();
                if (!container.HasData(scienceResult))
                {
                    container.AddData(scienceResult);
                }
            }
            results.Clear();
        }

        protected override void OnLoad(ConfigNode node)
        {
            ConfigNode currentNode = node.GetNode(GetType().FullName);
            ConfigNode.LoadObjectFromConfig(this, currentNode);
        }

        protected override void OnSave(ConfigNode node)
        {
            ConfigNode currentNode = ConfigNode.CreateConfigFromObject(this);
            node.AddNode(currentNode);
        }
    }
}