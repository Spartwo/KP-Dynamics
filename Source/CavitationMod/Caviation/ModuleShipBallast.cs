using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using KSPAchievements;

namespace Gladia
{
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ModuleShipBallast : PartModule
    {
        // This class creates a system of ballast storage and expulsion for attached parts

        const string ballastGroupName = "Ballast";

        // CFG Values
        [KSPField(isPersistant = true)]
        float maxBuoyancy = 5;
        [KSPField(isPersistant = true)]
        float minBuoyancy = 0;
        [KSPField(isPersistant = true)]
        float maxSpeed = 5;
        [KSPField(isPersistant = true)]
        float maxDepth = 2000;
        [KSPField()]
        float pumpRate = 2;

        [KSPField(isPersistant = true)]
        float variantPumpRate = 0;
        [KSPField(isPersistant = true)]
        float ECRequirement = 2;

        // user settings

        float partBuoyancy;
        [KSPAxisField(isPersistant = true,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Target Depth",
            guiUnits = " m",
            groupName = ballastGroupName,
            groupDisplayName = ballastGroupName,
            axisMode = KSPAxisMode.Incremental,
            minValue = 0f,
            maxValue = 2000f,
            incrementalSpeed = 10f),
            UI_FloatRange(
                minValue = 0f,
                maxValue = 2000f,
                stepIncrement = 25f,
                scene = UI_Scene.All
            )]
         public float targetDepth = 0f;

         [KSPField(isPersistant = false,
             guiActive = true,
             guiActiveEditor = false,
             guiName = "Current Depth",
             guiUnits = " m",
             groupName = ballastGroupName,
             groupDisplayName = ballastGroupName
            )]
         public int currentDepth = 0;

        [KSPField(isPersistant = false,
            guiActive = true,
            guiActiveEditor = false,
            guiName = "Flooded",
            guiUnits = "%",
            groupName = ballastGroupName,
            groupDisplayName = ballastGroupName
           )]
        public int fillPercent = 0;

        [KSPField(isPersistant = true,
             guiActive = true,
             guiActiveEditor = true,
             guiName = "Pump",
             groupName = ballastGroupName,
             groupDisplayName = ballastGroupName),
             UI_Toggle(
                 enabledText = "Enabled",
                 disabledText = "Disabled",
                 scene = UI_Scene.All
             )]
         public bool pumpActive = false;

         [KSPField(isPersistant = false,
             guiActive = true,
             guiActiveEditor = false,
             guiName = "Status",
             groupName = ballastGroupName,
             groupDisplayName = ballastGroupName)]
        public string ballastStatus = "Idle";

        float smallestVariantMass = 0;
        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorVariantApplied.Add(OnVariantApplied);

            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorVariantApplied.Add(OnEditorVariantApplied);

            partBuoyancy = Mathf.Clamp(part.buoyancy, minBuoyancy, maxBuoyancy);

            var floatRange = (UI_FloatRange)Fields["targetDepth"].uiControlEditor;
            floatRange.maxValue = maxDepth;

            variantPumpRate = pumpRate;
            smallestVariantMass = part.mass;
        }

        public void Update()
        {
            //On physics update
            //gradual buoyancy update in flight
            if (HighLogic.LoadedSceneIsFlight)
            {
                currentDepth = (int)Math.Round(Math.Abs(part.orbit.altitude));
                //Adjust buoyancy center to lowest corner(maybe)
                //Vector3 CenterOfBuoyancy
                //Compare with target depth and adjust flooding status
                PumpAdjust();
            }
        }

        private void PumpAdjust()
        {
            if (part.checkSplashed())
            {
                // Calculate error
                float error = targetDepth - currentDepth;
                float absoluteError = Math.Abs(error);

                // Place a reductive speed curve to ease part to a halt starting at 100m and ending at 2m
                double verticalSpeedLimit = Mathf.Max(Mathf.Min(absoluteError / 20, maxSpeed), 0.25f);
                float incriment = (((variantPumpRate / 100) * maxBuoyancy) * ((float)verticalSpeedLimit / (float)10));
                double vesselSpeed = vessel.verticalSpeed;


                if (pumpActive)
                {
                    if (part.GroundContact)
                    {
                        ballastStatus = "Run Aground";
                    }
                    else if (error < -1)
                    {
                        ballastStatus = "Ascending";
                    }
                    else if (error > 1)
                    {
                        ballastStatus = "Descending";
                    }
                    else
                    {
                        ballastStatus = "Idle";
                    }

                    if (vesselSpeed >= verticalSpeedLimit) // ascending too fast
                    {
                        partBuoyancy -= incriment;
                    }
                    else if (vesselSpeed <= -verticalSpeedLimit && availableEC(ECRequirement)) // descending too fast
                    {
                        partBuoyancy += incriment;
                    }
                    else if (error > 0) // above target depth
                    {
                        partBuoyancy -= incriment;
                    }
                    else if (error < 0 && availableEC(ECRequirement)) // below target depth
                    {
                        partBuoyancy += incriment;
                    }

                    //If aiming for surface and near surface just flush the tanks entirely
                    if (partBuoyancy < maxBuoyancy && currentDepth < 10 && targetDepth == 0 && availableEC(ECRequirement))
                    {
                        partBuoyancy += incriment;
                    }

                    partBuoyancy = Mathf.Clamp(partBuoyancy, minBuoyancy, maxBuoyancy);
                    part.buoyancy = partBuoyancy;

                    //display fill percent
                    float difference =  maxBuoyancy - partBuoyancy;
                    fillPercent = Mathf.RoundToInt((difference/maxBuoyancy)*100);
                }
                else
                {
                    ballastStatus = "Insufficient Electricity";
                }
            }
            else
            {
                currentDepth = 0;
                ballastStatus = "Above Waterline";
            }
        }

        private void OnVariantApplied(Part appliedPart, PartVariant variant)
        {
            if (appliedPart == part)
            {
                float pumpChange = smallestVariantMass / (smallestVariantMass + variant.Mass);
                variantPumpRate = pumpChange * pumpRate;
                Debug.Log("Pump Rate" + variantPumpRate + "/s");
            }
        }
        private void OnEditorVariantApplied(Part appliedPart, PartVariant variant)
        {
            float pumpChange = smallestVariantMass / (smallestVariantMass + variant.Mass);
            variantPumpRate = pumpChange * pumpRate;
            Debug.Log("Edit Pump Rate" + variantPumpRate + "/s");
        }
        private Boolean availableEC(float ECDemand)
        {
            // Make sure that at least 95% of required ec is there
            float drainAmount = ECDemand * Time.fixedDeltaTime;
            double chargeAvailable = part.RequestResource("ElectricCharge", drainAmount, ResourceFlowMode.ALL_VESSEL);
            return chargeAvailable > drainAmount * 0.95f;
        }

        public override string GetInfo()
        {
            string returnString =   "Descent/Ascent Velocity: " + maxSpeed + "m/s" +
                                    "\nMaximum Depth: " + maxDepth + "m" +
                                    "\nBuoyancy Range: " + minBuoyancy + " - " + maxBuoyancy +
                                    "\nPump Rate: " + pumpRate + "%/s (Base)" +
                                    "\n\nRequires " + ECRequirement + "ec/s when pumping";
            return returnString;
        }
    }
}
