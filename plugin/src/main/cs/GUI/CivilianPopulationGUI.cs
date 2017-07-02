﻿using System;
using CivilianPopulation.Domain;
using KSP.UI.Screens;
using UnityEngine;

namespace CivilianPopulation.GUI
{
    public class CivilianPopulationGUI
    {
		private CivilianPopulationService service;
        private TimeFormatter formatter;

		private ApplicationLauncherButton button = null;
		private bool windowShown = false;
		private Rect windowPosition = new Rect(Screen.width / 2 - 250, Screen.height / 2 - 250, 500, 300);
		private Vector2 scrollPosition;

		public CivilianPopulationGUI(CivilianPopulationService service)
        {
            this.service = service;
            this.formatter = new TimeFormatter();

			GameEvents.onGUIApplicationLauncherReady.Add(onAppLauncherReady);//when AppLauncher can take apps, give it OnAppLauncherReady (mine)
			GameEvents.onGUIApplicationLauncherDestroyed.Add(onAppLauncherDestroyed);//Not sure what this does
		}

        public void update()
        {
			if (windowShown)
			{
				windowPosition = GUILayout.Window(0, windowPosition, drawWindow, "Civilian Population");
			}
		}

		private void onAppLauncherReady()
		{
			if (ApplicationLauncher.Instance != null && button == null)
			{
				button = ApplicationLauncher.Instance.AddModApplication(
					windowToggle,
					windowToggle,
					null,
					null,
					null,
					null,
					ApplicationLauncher.AppScenes.ALWAYS,
					GameDatabase.Instance.GetTexture("CivilianPopulation/GUI/CivPopIcon", false)
				);
			}
		}

		private void onAppLauncherDestroyed()
		{
			if (ApplicationLauncher.Instance != null && button != null)
			{
				ApplicationLauncher.Instance.RemoveApplication(button);
			}
		}

		private void windowToggle()
		{
			windowShown = !windowShown;
		}

		private void drawWindow(int windowId)
		{
			GUILayout.BeginVertical();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(500), GUILayout.Height(300));

			GUILayout.BeginHorizontal();
            GUILayout.Label("Universal time : " + formatter.format(Planetarium.GetUniversalTime()));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
            GUILayout.Label("Time until taxes : " + formatter.format(service.getTimeUntilTaxes()));
			GUILayout.EndHorizontal();

			foreach (CivilianVessel vessel in service.getVessels())
			{
				if (vessel.getCivilianCount() > 0 || vessel.hasCivilianDock())
				{
					GUILayout.BeginHorizontal();
                    GUILayout.Label(vessel.getName() + " : " + vessel.getCivilianCount() + " Civilians");
					GUILayout.EndHorizontal();
                    if (vessel.hasCivilianDock()) {
						GUILayout.BeginHorizontal();
						GUILayout.Label(" -> Civilian dock : " + vessel.hasCivilianDock());
						if (GUILayout.Button("Spawn", GUILayout.Width(200f)))
						{
                            service.addNewCivilian(vessel.getId());
						}
						GUILayout.EndHorizontal();
					}
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Close", GUILayout.Width(200f)))
			{
				windowShown = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			UnityEngine.GUI.DragWindow();
		}

		private void log(string message)
		{
			Debug.Log(this.GetType().Name + message);
		}
	}
}