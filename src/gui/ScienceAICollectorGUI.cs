using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PersistentScienceCollectorAI.gui
{
    class ScienceAICollectorGUI
    {
        private static readonly ScienceAICollectorGUI instance = new ScienceAICollectorGUI();

        protected ApplicationLauncherButton AISButton = null;
        protected bool showWindow = false;
        protected Rect mainWindowRect = new Rect(0, 0, 800, 1);

        //public float UiScale { get; set; }
        //private GUISkin _skin;
        //private GUIStyle windowStyle;

        public void Init()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            /*if(_skin == null)
            {
                UiScale = 1f;
                _skin = GameObject.Instantiate(HighLogic.Skin) as GUISkin;
                //_skin.horizontalScrollbarThumb.fixedHeight = wScale(13);
                //_skin.horizontalScrollbar.fixedHeight = wScale(13);

                windowStyle = new GUIStyle(_skin.window);
                windowStyle.fontSize = (int)(_skin.window.fontSize * UiScale);
                windowStyle.padding = wScale(_skin.window.padding);
                windowStyle.margin = wScale(_skin.window.margin);
                windowStyle.border = wScale(_skin.window.border);
                windowStyle.contentOffset = wScale(_skin.window.contentOffset);

                _skin.window = windowStyle;
            }*/
        }

        /*protected int wScale(int v) { return Convert.ToInt32(Math.Round(v * UiScale)); }
        protected float wScale(float v) { return v * UiScale; }
        protected Rect wScale(Rect v)
        {
            return new Rect(wScale(v.x), wScale(v.y), wScale(v.width), wScale(v.height));
        }
        protected RectOffset wScale(RectOffset v)
        {
            return new RectOffset(wScale(v.left), wScale(v.right), wScale(v.top), wScale(v.bottom));
        }
        protected Vector2 wScale(Vector2 v)
        {
            return new Vector2(wScale(v.x), wScale(v.y));
        }*/

        public void OnGUI()
        {
            if (showWindow && AISButton != null && AISButton.isActiveAndEnabled)
            {
                mainWindowRect = GUILayout.Window(8940, mainWindowRect, DrawSettingsGUI, "PersistentScienceCollectorAI", HighLogic.Skin.window);
            }
        }

        private void DrawSettingsGUI(int windowID)
        {
            GUILayout.BeginVertical();
            foreach(Vessel v in FlightGlobals.Vessels)
            {
                ScienceAIVesselModule mod = v.GetComponent<ScienceAIVesselModule>();
                if (mod.active)
                {
                    //GUILayout.BeginArea(new Rect(0,0,800,1));
                    GUILayout.Label("Vessel: " + v.GetDisplayName() + "(" + mod.biome + ")");
                    foreach(ScienceAIExperiment experiment in mod.modules)
                    {
                        ScienceExperiment se = ResearchAndDevelopment.GetExperiment(experiment.experimentID);
                        GUILayout.Label(se.experimentTitle + " (" + experiment.inoperable + ")");
                    }
                    //GUILayout.EndArea();
                }
            }
            GUILayout.EndVertical();
            if (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            {
                GUI.DragWindow();
            }
        }

        private void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready && AISButton == null)
            {
                AISButton = ApplicationLauncher.Instance.AddModApplication(
                    ShowWindow,
                    HideAll,
                    null,
                    null,
                    null,
                    null,
                    (ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION),
                    GameDatabase.Instance.GetTexture("PersistentScienceCollectorAI/icon", false));
            }
        }

        private void ShowWindow()
        {
            showWindow = true;
        }

        private void HideAll()
        {
            showWindow = false;
        }

        private ScienceAICollectorGUI()
        {
            return;
        }

        public static ScienceAICollectorGUI Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
