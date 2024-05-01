using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using UnityEngine.SceneManagement;

namespace ToggleTracking
{
    // This module is applied to parts such as extendable solar panels and radiators to toggle their ability to track the sun
    public class ModuleToggleTracking : PartModule
    {

        [KSPEvent(guiActive = true,
            guiActiveEditor = true,
            guiName = "Tracking")]
        public void EventToggleTracking() => ToggleTracking();


        [KSPAction("Toggle Tracking")]
        public void AGToggleTracking(KSPActionParam param) => ToggleTracking();

        [KSPAction("Disable Tracking")]
        public void AGDisableTracking(KSPActionParam param) => SetTracking(false);

        [KSPAction("Enable Tracking")]
        public void AGEnableTracking(KSPActionParam param) => SetTracking(true);

        [KSPField(isPersistant = true)]
        public bool trackingEnabled;

        ModuleDeployablePart tracker;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            try
            {
                tracker = part.GetComponent<ModuleDeployablePart>();
                SetToggleName();
                if (state != StartState.Editor)
                {
                    SetTracking(trackingEnabled);
                }
            }
            catch(Exception e)
            {
                Debug.Log($"Setup Error: {e}");
            }

        }

        public override void OnLoad(ConfigNode node)
        {
            try
            {
                tracker = part.GetComponent<ModuleDeployablePart>();
                SetToggleName();
                SetTracking(trackingEnabled);
            }
            catch (Exception e)
            {
                Debug.Log($"Load Error: {e}");
            }
        }

        private void SetTracking(bool newState)
        {
            trackingEnabled = newState;
            tracker.isTracking = newState; 
            SetToggleName();
        }

        private void ToggleTracking()
        {
            bool newState = !trackingEnabled;
            trackingEnabled = newState;
            tracker.isTracking = newState;
            SetToggleName();
        }

        private void SetToggleName()
        {
            Events["EventToggleTracking"].guiName = trackingEnabled ? "Disable Tracking" : "Enable Tracking";
        }
    }
}
