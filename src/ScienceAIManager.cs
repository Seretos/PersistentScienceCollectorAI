using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.UI.Screens;
using KSP.UI.Screens.SpaceCenter.MissionSummaryDialog;
using UnityEngine;

namespace PersistentScienceCollectorAI
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    class ScienceAIManager : MonoBehaviour
    {
        public void Start()
        {
            GameEvents.onPartDeCouple.Remove(OnUndock);
            GameEvents.onPartUndock.Remove(OnUndock);
            GameEvents.onPartDeCoupleComplete.Remove(OnUndockComplete);
            GameEvents.onPartUndockComplete.Remove(OnUndockComplete);

            GameEvents.onVesselDocking.Remove(OnVesselDocking);

            if (HighLogic.LoadedSceneIsEditor)
                return;

            // first time, the decouple method will called... then undock... why ever
            GameEvents.onPartDeCouple.Add(OnUndock);
            GameEvents.onPartUndock.Add(OnUndock);
            GameEvents.onPartDeCoupleComplete.Add(OnUndockComplete);
            GameEvents.onPartUndockComplete.Add(OnUndockComplete);

            GameEvents.onVesselDocking.Add(OnVesselDocking);
            
            GameEvents.onVesselRecoveryProcessing.Add(OnVesselRecoveryProcessing);

            ModWindow.Instance.Init();
        }

        private void OnVesselRecoveryProcessing(ProtoVessel data0, MissionRecoveryDialog data1, float data2)
        {
            ScienceAIVesselModule mod = data0.vesselRef.GetComponent<ScienceAIVesselModule>();
            if (mod.active)
            {
                foreach(ProtoPartSnapshot part in data0.protoPartSnapshots)
                {
                    if (part.persistentId == mod.partID)
                    {
                        foreach (ProtoPartModuleSnapshot partmodule in part.modules)
                        {
                            if (partmodule.moduleName == "ModuleScienceContainer")
                            {
                                ConfigNode node = partmodule.moduleValues;
                                node.RemoveNodes("ScienceData");
                                foreach (ScienceAIData result in mod.results)
                                {
                                    node.AddNode("ScienceData", result.GetConfigNode());
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void OnGUI()
        {
            ModWindow.Instance.OnGUI();
        }

        private void OnVesselDocking(uint data0, uint data1)
        {
            ScienceAIVesselModule source = FlightGlobals.Vessels.Find(v => v.persistentId == data0).GetComponent<ScienceAIVesselModule>();
            ScienceAIVesselModule target = FlightGlobals.Vessels.Find(v => v.persistentId == data1).GetComponent<ScienceAIVesselModule>();
            source.Deactivate();
            target.Deactivate();
        }

        private void OnUndockComplete(Part data)
        {
            data.vessel.GetComponent<ScienceAIVesselModule>().Deactivate();
        }

        private void OnUndock(Part data)
        {
            data.vessel.GetComponent<ScienceAIVesselModule>().Deactivate();
        }

        public void FixedUpdate()
        {
            foreach(Vessel v in FlightGlobals.Vessels)
            {
                ScienceAIVesselModule mod = v.GetComponent<ScienceAIVesselModule>();
                if (mod.active)
                {
                    String biome = ScienceUtil.GetExperimentBiome(v.mainBody, v.latitude, v.longitude);
                    ExperimentSituations situation = ScienceUtil.GetExperimentSituation(v);
                    foreach(ScienceAIExperiment experiment in mod.experiments)
                    {
                        if (ValidateExperiment(experiment, situation, mod, v))
                        {
                            RunExperiment(experiment, situation, mod, v, biome);
                        }
                    }
                }
            }
        }

        private bool ValidateExperiment(ScienceAIExperiment experiment, ExperimentSituations situation, ScienceAIVesselModule mod, Vessel v)
        {
            float ACLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex);
            float RnDLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.ResearchAndDevelopment);

            if (experiment.rerunnable == false && mod.reusableOnly)
                return false;

            bool hasCrew = v.GetVesselCrew().Count() > 0;
            bool hasScientist = v.GetVesselCrew().FindAll(pcm => pcm.trait == "Scientist").Count() > 0;
            bool evaPossible = false;
            double altitude = (v.GetWorldPos3D() - v.mainBody.position).magnitude - v.mainBody.Radius;
            if (hasCrew)
            {
                if (!v.mainBody.atmosphere && ACLevel >= 0.5)
                {
                    evaPossible = true;
                }
                else if (altitude > v.mainBody.atmosphereDepth && ACLevel >= 0.5)
                {
                    evaPossible = true;
                }
                else if (situation == ExperimentSituations.SrfLanded || situation == ExperimentSituations.SrfSplashed)
                {
                    evaPossible = true;
                }
            }

            if (experiment.experimentID == "evaReport")
            {
                if (!evaPossible)
                    return false;
            }

            if(experiment.experimentID == "surfaceSample")
            {
                if (situation != ExperimentSituations.SrfLanded && situation != ExperimentSituations.SrfSplashed)
                    return false;

                if (RnDLevel < 0.5)
                    return false;
            }

            if (experiment.inoperable == true && hasScientist && evaPossible)
            {
                experiment.inoperable = false;
            }

            if (experiment.inoperable)
                return false;

            return true;
        }

        private void RunExperiment(ScienceAIExperiment experiment, ExperimentSituations situation, ScienceAIVesselModule mod, Vessel v, String biome)
        {
            String currentBiome = String.Empty;
            ScienceExperiment exp = ResearchAndDevelopment.GetExperiment(experiment.experimentID);
            if (exp.BiomeIsRelevantWhile(situation))
            {
                currentBiome = v.landedAt != string.Empty ? v.landedAt : biome;
            }
            ScienceSubject subj = ResearchAndDevelopment.GetExperimentSubject(exp, situation, v.mainBody, currentBiome, null);
            if (((subj.scienceCap - subj.science) > 0.1 || mod.collectEmpty) && mod.results.Find(r => r.subjectID == subj.id) == null)
            {
                ScienceAIData result = new ScienceAIData(exp.baseValue * exp.dataScale, experiment.xmitDataScalar, 0f, subj.id, subj.title);
                mod.results.Add(result);

                if (experiment.rerunnable == false)
                {
                    experiment.inoperable = true;
                }
            }
        }
    }
}
