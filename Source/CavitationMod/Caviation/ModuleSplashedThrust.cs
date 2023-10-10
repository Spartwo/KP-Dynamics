using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;

namespace Gladia
{
    public class ModuleSplashedThrust : ModuleEnginesFX
    {
        // This class extends the stock engine module to provide in-water only engines

        [KSPField(isPersistant = true)]
        public bool initialFlameout;

        [KSPField(isPersistant = true)]
        public float trueMaxThrust;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            // if (isReversed)
            //setReverseTransform();
            //updateGUI();
            trueMaxThrust = base.GetMaxThrust();
        }

        public override void FXUpdate()
        {
            base.FXUpdate();

            //Check if the thrust transform is submerged
            //if (part.checkSplashed())
            if (base.CheckTransformsUnderwater())
            {
                base.status = "Nominal";
                base.finalThrust = base.currentThrottle * trueMaxThrust;
                base.maxThrust = base.finalThrust;
                //base.multIsp = 1f;
            }
            else 
            {
                base.status = "Above Waterline";
                base.finalThrust = 0;//  (base.currentThrottle * 0.01f) * (trueMaxThrust);
                base.maxThrust = base.finalThrust;
                //base.multIsp = 0.01f;
            }

        }

    }
}
