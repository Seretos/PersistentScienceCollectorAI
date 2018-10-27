using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentScienceCollectorAI
{
    class ScienceAIData
    {
        [Persistent]
        public float amount;
        [Persistent]
        public float xmitValue;
        [Persistent]
        public float xmitBonus;
        [Persistent]
        public String id;
        [Persistent]
        public String dataName;
    }
}
