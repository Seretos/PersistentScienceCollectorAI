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

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            Fields[nameof(IsAutoCollect)].uiControlFlight.onFieldChanged = OnAutoCollectChanged;
            Fields[nameof(IsCollectEmpty)].uiControlFlight.onFieldChanged = OnAutoCollectPropertyChanged;
            Fields[nameof(IsReusableOnly)].uiControlFlight.onFieldChanged = OnAutoCollectPropertyChanged;
        }

        internal void OnAutoCollectPropertyChanged(BaseField field, System.Object obj)
        {
            ScienceAIVesselModule mod = vessel.GetComponent<ScienceAIVesselModule>();
            mod.collectEmpty = IsCollectEmpty;
            mod.reusableOnly = IsReusableOnly;
        }

        internal void OnAutoCollectChanged(BaseField field, System.Object obj)
        {
            ScienceAIVesselModule mod = vessel.GetComponent<ScienceAIVesselModule>();
            ModuleScienceContainer container = part.FindModuleImplementing<ModuleScienceContainer>();
            if (IsAutoCollect)
            {
                container.CollectAllEvent();
                mod.collectEmpty = IsCollectEmpty;
                mod.reusableOnly = IsReusableOnly;
                mod.Activate();
            }
            else
            {
                mod.Deactivate();
            }
        }
    }
}
