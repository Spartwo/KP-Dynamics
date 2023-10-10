using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using ModuleWheels;

namespace Traction
{
    public class ModuleSwapSteering : PartModule
    {
        ModuleWheelMotor PivotMotor;
        ModuleWheelSteering PivotSteer;
        ModuleWheelMotorSteering TankSteer; 
        
        //No group preferrably
        [KSPField(
            isPersistant = true,
            guiActive = true,
            guiActiveEditor = true,
            guiName = "Steering Type"),
            UI_ChooseOption(controlEnabled = true, affectSymCounterparts = UI_Scene.None,
            options = new string[] { "Pivot", "Differential" })]
        public string currentType = "Pivot";

        private string storedType = "Pivot";

        #region Part Actions
        [KSPAction("Toggle Steering Type")]
        public void ToggleAction(KSPActionParam param)
        {
            currentType = (currentType == "Pivot") ? "Differential" : "Pivot";
            AlignSteering(currentType == "Pivot");
        }

        [KSPAction("Enable Differential Steering")]
        public void MotorAction(KSPActionParam param)
        {
            currentType = "Differential";
            AlignSteering(false);
        }

        [KSPAction("Enable Pivot Steering")]
        public void PivotAction(KSPActionParam param)
        {
            currentType = "Pivot";
            AlignSteering(true);
        }
        #endregion

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            PivotMotor = part.GetComponent<ModuleWheelMotor>();
            PivotSteer = part.GetComponent<ModuleWheelSteering>();
            TankSteer = part.GetComponent<ModuleWheelMotorSteering>();

            AlignSteering(currentType == "Pivot");

        }

        public void Update()
        {
            if (storedType != currentType)
            {
                AlignSteering(currentType == "Pivot");
            }
        }

        private void AlignSteering(bool Pivot)
        {
            storedType = currentType;
            if (Pivot) 
            {
                PivotMotor.enabled = true;
                PivotSteer.enabled = true;
                TankSteer.enabled = false;
            }
            else 
            {
                PivotMotor.enabled = false;
                PivotSteer.enabled = false;
                TankSteer.enabled = true;
            }
            UpdateUI(Pivot);
        }

        private void UpdateUI(bool Input)
        {
            bool Pivot = Input;
            bool Motor = !Input;

            //Pivot Motor Fields
            PivotMotor.Fields[nameof(PivotMotor.motorEnabled)].guiActive = Pivot;
            PivotMotor.Fields[nameof(PivotMotor.motorInverted)].guiActive = Pivot;
            PivotMotor.Fields[nameof(PivotMotor.tractionControlScale)].guiActive = Pivot;
            PivotMotor.Fields[nameof(PivotMotor.driveOutput)].guiActive = Pivot;


            PivotMotor.Fields[nameof(PivotMotor.motorEnabled)].guiActiveEditor = Pivot;
            PivotMotor.Fields[nameof(PivotMotor.motorInverted)].guiActiveEditor = Pivot;
            PivotMotor.Fields[nameof(PivotMotor.tractionControlScale)].guiActiveEditor = Pivot;
            PivotMotor.Fields[nameof(PivotMotor.driveOutput)].guiActiveEditor = Pivot;

            //Pivot Steering Fields
            PivotSteer.Fields[nameof(PivotSteer.steeringEnabled)].guiActive = Pivot;
            PivotSteer.Fields[nameof(PivotSteer.steeringInvert)].guiActive = Pivot;

            PivotSteer.Fields[nameof(PivotSteer.steeringEnabled)].guiActiveEditor = Pivot;
            PivotSteer.Fields[nameof(PivotSteer.steeringInvert)].guiActiveEditor = Pivot;

            //Motor Steering Fields
            TankSteer.Fields[nameof(TankSteer.steeringInvert)].guiActive = Motor;
            TankSteer.Fields[nameof(TankSteer.steeringEnabled)].guiActive = Motor;
            TankSteer.Fields[nameof(TankSteer.motorEnabled)].guiActive = Motor;
            TankSteer.Fields[nameof(TankSteer.motorInverted)].guiActive = Motor;
            TankSteer.Fields[nameof(TankSteer.tractionControlScale)].guiActive = Motor;
            TankSteer.Fields[nameof(TankSteer.driveOutput)].guiActive= Motor;

            TankSteer.Fields[nameof(TankSteer.steeringInvert)].guiActiveEditor = Motor;
            TankSteer.Fields[nameof(TankSteer.steeringEnabled)].guiActiveEditor = Motor;
            TankSteer.Fields[nameof(TankSteer.motorEnabled)].guiActiveEditor = Motor;
            TankSteer.Fields[nameof(TankSteer.motorInverted)].guiActiveEditor = Motor;
            TankSteer.Fields[nameof(TankSteer.tractionControlScale)].guiActiveEditor = Motor;
            TankSteer.Fields[nameof(TankSteer.driveOutput)].guiActiveEditor = Motor;

            //AG Changes

            RefreshAssociatedWindows(part);
        }
        public static void RefreshAssociatedWindows(Part part)
        {
            IEnumerator<UIPartActionWindow> window = FindObjectsOfType(typeof(UIPartActionWindow)).Cast<UIPartActionWindow>().GetEnumerator();
            while (window.MoveNext())
            {
                if (window.Current == null) continue;
                if (window.Current.part == part)
                {
                    window.Current.displayDirty = true;
                }
            }
            window.Dispose();
        }
    }
}
