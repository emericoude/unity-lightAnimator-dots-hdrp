using Unity.Entities;
using UnityEngine;
using System;

namespace Emeric{

    [Serializable]
    //[GenerateAuthoringComponent] 
    [RequireComponent(typeof(FlickeringLightTriggerChangeTagAuthoring))]
    public class FlickeringLightTriggerChange : IComponentData
    {
        public bool DisableSelfOnTrigger;
        public Entity[] LightsToChange;
        public FlickeringLightSettings FlickerToSet;
        public FlickeringLight CustomSettings;

        public FlickeringLight GetFlickerToApply()
        {
            switch (FlickerToSet)
            {
                case FlickeringLightSettings.Custom:
                    return CustomSettings;

                case FlickeringLightSettings.Off:
                    return FlickeringLight.Off();

                case FlickeringLightSettings.Ambient:
                    return FlickeringLight.Ambient();

                case FlickeringLightSettings.Spooky:
                    return FlickeringLight.Spooky();

                case FlickeringLightSettings.Alert:
                    return FlickeringLight.Alert();

                default:
                    return FlickeringLight.Ambient();
            }
        }
    }
}


