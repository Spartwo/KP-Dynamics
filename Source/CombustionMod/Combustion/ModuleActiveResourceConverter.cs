using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;

namespace Combustion
{
    public class ModuleActiveResourceConverter : ModuleResourceConverter
    {
        // This class extends the stock resource convertor so that only one type can run at a time
       
        /*public override void OnStart(StartState state)
        {
            //base.StartResourceConverter();
          UpdateUI();
        }*/

        private void UpdateUI()
        {
            //Pivot Motor Fields
            //base.Fields[nameof(base.StartResourceConverter)].guiActive = false;
            //base.Fields[nameof(base.StartResourceConverter)].guiActiveEditor = false;

            //base.Fields[nameof(base.StopResourceConverter)].guiActive = false;
            //base.Fields[nameof(base.StopResourceConverter)].guiActiveEditor = false;
        }

        public override void StopResourceConverter()
        {
            //base.StopResourceConverter();
            base.StartResourceConverter();
        }

    }
}
