using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PersistentScienceCollectorAI
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class Class1 : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Hallo");
        }
    }
}
