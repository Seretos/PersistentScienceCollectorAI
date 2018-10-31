using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentScienceCollectorAI
{
    class ScienceAIData
    {
        [Persistent]
        public float labValue = 0f;
        [Persistent]
        public float dataAmount = 0f;
        [Persistent]
        public float baseTransmitValue = 0f;
        [Persistent]
        public float transmitBonus = 0f;
        [Persistent]
        public string subjectID = String.Empty;
        [Persistent]
        public string title = String.Empty;
        [Persistent]
        public bool triggered = false;
        [Persistent]
        public uint container = 0;

        public ScienceAIData()
        {

        }

        public ScienceAIData(float amount, float xmitValue, float xmitBonus, string id, string dataName)
        {
            dataAmount = amount;
            baseTransmitValue = xmitValue;
            transmitBonus = xmitBonus;
            subjectID = id;
            title = dataName;
        }

        public ConfigNode GetConfigNode()
        {
            ConfigNode resultNode = new ConfigNode();
            resultNode.AddValue("data", dataAmount);
            resultNode.AddValue("subjectID", subjectID);
            resultNode.AddValue("xmit", baseTransmitValue);
            resultNode.AddValue("xmitBonus", transmitBonus);
            resultNode.AddValue("title", title);
            resultNode.AddValue("triggered", false);
            resultNode.AddValue("container", container);
            return resultNode;
        }

        public ScienceAIData(ScienceData data)
        {
            labValue = data.labValue;
            dataAmount = data.dataAmount;
            baseTransmitValue = data.baseTransmitValue;
            transmitBonus = data.transmitBonus;
            subjectID = data.subjectID;
            title = data.title;
            triggered = data.triggered;
            container = data.container;
        }

        public ScienceData CreateScienceData()
        {
            return new ScienceData(dataAmount, baseTransmitValue, transmitBonus, subjectID, title, triggered, container);
        }
    }
}
