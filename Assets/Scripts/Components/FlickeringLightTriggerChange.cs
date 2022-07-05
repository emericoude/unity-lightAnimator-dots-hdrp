using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public class FlickeringLightTriggerChange : IComponentData
{
    public enum LightFlickerSettings
    {
        Custom,
        Ambient,
        Spooky,
        Alert
    }

    public Entity[] LightsToChange;

    public LightFlickerSettings FlickerToSet;
    public FlickeringLight CustomSettings;

    public bool DisableSelfOnTrigger;

    public FlickeringLight GetFlickerToApply()
    {
        switch (FlickerToSet)
        {
            case LightFlickerSettings.Custom:
                return CustomSettings;

            case LightFlickerSettings.Ambient:
                return FlickeringLight.Ambient();

            case LightFlickerSettings.Spooky:
                return FlickeringLight.Spooky();

            case LightFlickerSettings.Alert:
                return FlickeringLight.Alert();

            default:
                return FlickeringLight.Ambient();
        }
    }
}
