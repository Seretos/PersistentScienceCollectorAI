using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PersistentScienceCollectorAI
{
    class ScienceAIContainerModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "#SCIENCE_BOX_AUTO_COLLECT", guiFormat = "N1")]
        [UI_Toggle(disabledText = "#SCIENCE_BOX_DISABLED", enabledText = "#SCIENCE_BOX_ENABLED")]
        public bool IsAutoCollect = false;
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "#SCIENCE_BOX_COLLECT_EMPTY", guiFormat = "N2")]
        [UI_Toggle(disabledText = "#SCIENCE_BOX_DISABLED", enabledText = "#SCIENCE_BOX_ENABLED")]
        public bool IsCollectEmpty = false;
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "#SCIENCE_BOX_REUSABLE_ONLY", guiFormat = "N2")]
        [UI_Toggle(disabledText = "#SCIENCE_BOX_DISABLED", enabledText = "#SCIENCE_BOX_ENABLED")]
        public bool IsReusableOnly = true;

        protected ScienceAIVesselModule vesselModule = null;
        
        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            Fields[nameof(IsAutoCollect)].uiControlFlight.onFieldChanged = OnAutoCollectChanged;
            Fields[nameof(IsCollectEmpty)].uiControlFlight.onFieldChanged = OnAutoCollectPropertyChanged;
            Fields[nameof(IsReusableOnly)].uiControlFlight.onFieldChanged = OnAutoCollectPropertyChanged;
            vesselModule = vessel.GetComponent<ScienceAIVesselModule>();
        }

        internal void OnAutoCollectPropertyChanged(BaseField field, System.Object obj)
        {
            vesselModule.biome = String.Empty;
            vesselModule.situation = String.Empty;
            vesselModule.collectEmpty = IsCollectEmpty;
            vesselModule.collectReusableOnly = IsReusableOnly;
            //vesselModule.SaveToVessel();
            //vesselModule.LoadFromVessel();
        }

        internal void OnAutoCollectChanged(BaseField field, System.Object obj)
        {
            vesselModule.active = IsAutoCollect;
            vesselModule.biome = String.Empty;
            vesselModule.situation = String.Empty;
            if (IsAutoCollect)
            {
                foreach (ScienceAIContainerModule mod in vessel.FindPartModulesImplementing<ScienceAIContainerModule>().Where(m => m.IsAutoCollect == true))
                {
                    if (mod != this)
                    {
                        mod.IsAutoCollect = false;
                    }
                }
                vesselModule.LoadFromVessel();
            }
            vesselModule.SaveToVessel();
        }
    }
}
