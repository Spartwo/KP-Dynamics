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
    public class ModuleExclusiveResourceConverter : ModuleResourceConverter
    {
        // This class extends the stock resource convertor so that only one type can run at a time
       

        public override void StartResourceConverter()
        {
            StopOtherConverters();
            base.StartResourceConverter();
        }

        private void StopOtherConverters ()
        {
            ModuleExclusiveResourceConverter[] otherConverters = part.GetComponents<ModuleExclusiveResourceConverter>();
            foreach (ModuleExclusiveResourceConverter e in otherConverters) 
            {
                e.StopResourceConverter();
            }
        }

    }
}
