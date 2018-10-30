using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PersistentScienceCollectorAI
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    class ScienceAIManager : MonoBehaviour
    {
        protected float LastRnDLevel = 0;
        protected float LastACLevel = 0;
        public void Start()
        {
            GameEvents.onDockingComplete.Remove(OnDockingComplete);
            GameEvents.onVesselsUndocking.Remove(OnVesselIsUndocking);
            GameEvents.onPartUndock.Remove(OnPartUndock);
            GameEvents.onVesselDocking.Remove(OnVesselDocking);
            GameEvents.onVesselWasModified.Remove(OnVesselWasModified);

            if (HighLogic.LoadedSceneIsEditor)
                return;

            GameEvents.onDockingComplete.Add(OnDockingComplete);
            GameEvents.onVesselsUndocking.Add(OnVesselIsUndocking);
            GameEvents.onPartUndock.Add(OnPartUndock);
            GameEvents.onVesselDocking.Add(OnVesselDocking);
            GameEvents.onVesselWasModified.Add(OnVesselWasModified);

            gui.ScienceAICollectorGUI.Instance.Init();
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            float RnDLevel = 0;
            float ACLevel = 0;

            if (LastRnDLevel < 0.5)
                RnDLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.ResearchAndDevelopment);
            else
                RnDLevel = LastRnDLevel;

            if (LastACLevel < 0.5)
                ACLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex);
            else
                ACLevel = LastACLevel;

            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                ScienceAIVesselModule vesselModule = vessel.GetComponent<ScienceAIVesselModule>();
                if (vesselModule != null && vesselModule.active)
                {
                    String currentBiome = ScienceUtil.GetExperimentBiome(vessel.mainBody, vessel.latitude, vessel.longitude);
                    if (vesselModule.biome != currentBiome || vesselModule.situation != vessel.SituationString || RnDLevel != LastRnDLevel || ACLevel != LastACLevel)
                    {
                        vesselModule.biome = currentBiome;
                        vesselModule.situation = vessel.SituationString;
                        ExperimentSituations situation = ScienceUtil.GetExperimentSituation(vessel);

                        foreach (ScienceAIExperiment experiment in vesselModule.modules)
                        {
                            RunExperiments(experiment, situation, vessel, vesselModule);
                        }
                    }
                }
            }
            LastRnDLevel = RnDLevel;
            LastACLevel = ACLevel;
        }

        public void OnGUI()
        {
            gui.ScienceAICollectorGUI.Instance.OnGUI();
        }

        private void RunExperiments(ScienceAIExperiment experiment, ExperimentSituations situation, Vessel vessel, ScienceAIVesselModule vesselModule)
        {
            bool noEva = false;
            if (situation != ExperimentSituations.SrfLanded
                && situation != ExperimentSituations.SrfSplashed
                && situation != ExperimentSituations.InSpaceLow
                && situation != ExperimentSituations.InSpaceHigh)
            {
                noEva = true;
            }

            if (ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex) < 0.5
                && situation != ExperimentSituations.SrfLanded && situation != ExperimentSituations.SrfSplashed)
            {
                noEva = true;
            }

            if (experiment.experimentID == "evaReport"
                && noEva)
            {
                return;
            }
            if (experiment.experimentID == "surfaceSample" && !(situation == ExperimentSituations.SrfLanded || situation == ExperimentSituations.SrfSplashed))
            {
                return;
            }
            if (experiment.experimentID == "surfaceSample" && ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.ResearchAndDevelopment) < 0.5)
            {
                return;
            }
            bool hasScientist = vessel.GetVesselCrew().FindAll(pcm => pcm.trait == "Scientist").Count() > 0;

            if (experiment.inoperable && !noEva && hasScientist)
            {
                experiment.inoperable = false;
            }

            if (vesselModule.collectReusableOnly && !(experiment.rerunnable || hasScientist))
            {
                return;
            }

            RunExperiment(experiment, situation, vessel, vesselModule,hasScientist,noEva);
        }

        private void RunExperiment(ScienceAIExperiment experiment, ExperimentSituations situation, Vessel vessel, ScienceAIVesselModule vesselModule, bool hasScientist, bool noEva)
        {
            if (!experiment.inoperable)
            {
                String biome = String.Empty;
                ScienceExperiment exp = ResearchAndDevelopment.GetExperiment(experiment.experimentID);
                if (exp.BiomeIsRelevantWhile(situation))
                {
                    biome = vessel.landedAt != string.Empty ? vessel.landedAt : ScienceUtil.GetExperimentBiome(vessel.mainBody, vessel.latitude, vessel.longitude);
                }
                ScienceSubject subj = ResearchAndDevelopment.GetExperimentSubject(exp, situation, vessel.mainBody, biome, null);
                if (((subj.scienceCap - subj.science) > 0.1 || vesselModule.collectEmpty) && vesselModule.data.Find(r => r.id == subj.id) == null)
                {
                    ScienceAIData result = new ScienceAIData
                    {
                        amount = exp.baseValue * exp.dataScale,
                        xmitValue = experiment.xmitDataScalar,
                        xmitBonus = 0f,
                        id = subj.id,
                        dataName = subj.title
                    };
                    if (vesselModule.AddScienceResult(result))
                    {
                        if (!experiment.rerunnable && (!hasScientist || noEva))
                        {
                            experiment.inoperable = true;
                        }
                    }
                }
            }
        }

        private void OnVesselWasModified(Vessel data)
        {
            ScienceAIVesselModule mod = data.GetComponent<ScienceAIVesselModule>();
            mod.LoadFromVessel();
            mod.SaveToVessel();
            mod.biome = String.Empty;
            mod.situation = String.Empty;
        }

        private void OnVesselDocking(uint data0, uint data1)
        {
            ScienceAIVesselModule source = FlightGlobals.Vessels.Find(v => v.persistentId == data0).GetComponent<ScienceAIVesselModule>();
            ScienceAIVesselModule target = FlightGlobals.Vessels.Find(v => v.persistentId == data1).GetComponent<ScienceAIVesselModule>();
            source.active = false;
            target.active = false;
            source.SaveToVessel();
            target.SaveToVessel();
        }

        private void OnDockingComplete(GameEvents.FromToAction<Part, Part> data)
        {
            ScienceAIVesselModule source = data.from.vessel.GetComponent<ScienceAIVesselModule>();
            source.LoadFromVessel();
            source.SaveToVessel();
            source.biome = String.Empty;
            source.situation = String.Empty;
        }

        private void OnPartUndock(Part data)
        {
            ScienceAIVesselModule mod = data.vessel.GetComponent<ScienceAIVesselModule>();
            mod.SaveToVessel();
            mod.biome = String.Empty;
            mod.situation = String.Empty;
        }

        private void OnVesselIsUndocking(Vessel origin, Vessel created)
        {
            origin.GetComponent<ScienceAIVesselModule>().LoadFromVessel();
            origin.GetComponent<ScienceAIVesselModule>().SaveToVessel();

            created.GetComponent<ScienceAIVesselModule>().LoadFromVessel();
            created.GetComponent<ScienceAIVesselModule>().SaveToVessel();
        }
    }
}
