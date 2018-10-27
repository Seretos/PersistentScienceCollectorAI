using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentScienceCollectorAI
{
    class ScienceAIExperiment
    {
        [Persistent]
        public uint partId;
        [Persistent]
        public String experimentID;
        [Persistent]
        public bool enabled;
        [Persistent]
        public bool inoperable;
        [Persistent]
        public bool rerunnable;
        [Persistent]
        public bool resettable;
        [Persistent]
        public bool resettableOnEVA;
        [Persistent]
        public float xmitDataScalar;

        protected PartModule experiment = null;
    }
}
