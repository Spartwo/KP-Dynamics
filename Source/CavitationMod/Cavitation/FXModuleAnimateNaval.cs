using Cavitation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cavitation
{
    public class FXModuleAnimateNaval : PartModule
    {
        ModuleSplashedThrust Engine;

        public override void OnStart(StartState state)
        {
            if (!HighLogic.LoadedSceneIsFlight) return;

            Engine = part.FindModuleImplementing<ModuleSplashedThrust>();
            
        }

        public void Update()
        {
            
        }
    }
}
