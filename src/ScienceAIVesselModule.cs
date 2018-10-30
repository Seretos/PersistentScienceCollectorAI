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

        protected override void OnStart()
        {
        }

        public void Activate()
        {
            active = true;
        }

        public void Disable()
        {
            active = false;
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