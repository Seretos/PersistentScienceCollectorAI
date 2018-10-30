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
        public void Start()
        {
            /*GameEvents.onDockingComplete.Remove(OnDockingComplete);
            GameEvents.onVesselsUndocking.Remove(OnVesselIsUndocking);
            GameEvents.onPartUndock.Remove(OnPartUndock);
            GameEvents.onVesselDocking.Remove(OnVesselDocking);
            GameEvents.onVesselWasModified.Remove(OnVesselWasModified);*/
            GameEvents.onVesselStandardModification.Remove(OnVesselStandardModification);

            if (HighLogic.LoadedSceneIsEditor)
                return;

            /*GameEvents.onDockingComplete.Add(OnDockingComplete);
            GameEvents.onVesselsUndocking.Add(OnVesselIsUndocking);
            GameEvents.onPartUndock.Add(OnPartUndock);
            GameEvents.onVesselDocking.Add(OnVesselDocking);
            GameEvents.onVesselWasModified.Add(OnVesselWasModified);*/
            GameEvents.onVesselStandardModification.Add(OnVesselStandardModification);
        }

        private void OnVesselStandardModification(Vessel data)
        {
            data.GetComponent<ScienceAIVesselModule>().Disable();
        }

        public void FixedUpdate()
        {
            String output = "";
            foreach(Vessel v in FlightGlobals.Vessels)
            {
                output += v.GetDisplayName() + "(" + v.name + "): " + v.GetComponent<ScienceAIVesselModule>().active + " ";
            }
            Debug.Log(output);
        }

        /*private void OnVesselWasModified(Vessel data)
        {
        }

        private void OnVesselDocking(uint data0, uint data1)
        {
            FlightGlobals.Vessels.Find(v => v.persistentId == data0).GetComponent<ScienceAIVesselModule>().Disable();
            FlightGlobals.Vessels.Find(v => v.persistentId == data1).GetComponent<ScienceAIVesselModule>().Disable();
        }

        private void OnDockingComplete(GameEvents.FromToAction<Part, Part> data)
        {
        }

        private void OnPartUndock(Part data)
        {
            data.vessel.GetComponent<ScienceAIVesselModule>().Disable();
        }

        private void OnVesselIsUndocking(Vessel origin, Vessel created)
        {
        }*/
    }
}
