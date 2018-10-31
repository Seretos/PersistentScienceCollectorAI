using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PersistentScienceCollectorAI
{
    class ModWindow
    {
        private static readonly ModWindow instance = new ModWindow();

        protected ApplicationLauncherButton AISButton = null;
        protected bool showWindow = false;
        protected Rect mainWindowRect = new Rect(0, 0, 400, 400);

        Vector2 scrollPosition;

        private ScienceAIVesselModule visibleResultModule = null;

        public void Init()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        public void OnGUI()
        {
            if (showWindow && AISButton != null && AISButton.isActiveAndEnabled)
            {
                mainWindowRect = GUILayout.Window(8940, mainWindowRect, DrawSettingsGUI, "PersistentScienceCollectorAI", HighLogic.Skin.window);
            }
        }

        private void DrawSettingsGUI(int windowID)
        {
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition, GUILayout.Width(400), GUILayout.Height(400));
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                ScienceAIVesselModule mod = v.GetComponent<ScienceAIVesselModule>();
                if (mod.active)
                {
                    if (GUILayout.Button(v.GetDisplayName()))
                    {
                        if (visibleResultModule == mod)
                            visibleResultModule = null;
                        else
                            visibleResultModule = mod;
                    }
                    if (visibleResultModule != null && visibleResultModule == mod)
                    {
                        foreach (ScienceAIData result in mod.results)
                        {
                            GUILayout.Label(result.title);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
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

        private ModWindow()
        {
            return;
        }

        public static ModWindow Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
