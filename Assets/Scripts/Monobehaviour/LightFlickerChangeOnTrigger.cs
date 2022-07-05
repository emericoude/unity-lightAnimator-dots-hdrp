using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerChangeOnTrigger : MonoBehaviour
{
    private enum LightFlickerSettings { 
        Custom,
        Ambient,
        Spooky,
        Alert
    }

    [Header("Settings")]
    [SerializeField] private LightFlickerSettings _flickerToSet;
    [SerializeField] private List<FlickeringLightAuthoring> _lightToFlickers = new List<FlickeringLightAuthoring>();
    [SerializeField] private FlickeringLight _customSettings;

    [Header("Trigger Options")]
    [SerializeField] private bool _activateOnTrigger = true;
    [SerializeField] private bool _disableSelfOnTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        
        if (_activateOnTrigger)
        {
            if (other.CompareTag("Player"))
            {
                if (_lightToFlickers.Count > 0)
                {
                    ApplyFlickerChange();
                }

                if (_disableSelfOnTrigger)
                {
                    this.enabled = false;
                }
            }
        }
    }

    public void ApplyFlickerChange()
    {
        FlickeringLight flickerToSet = GetFlickerToApply();

        for (int i = 0; i < _lightToFlickers.Count; i++)
        {
            SetLightFlicker(_lightToFlickers[i], flickerToSet);
        }
    }

    private FlickeringLight GetFlickerToApply()
    {
        switch (_flickerToSet)
        {
            case LightFlickerSettings.Custom:
                return _customSettings;

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

    private void SetLightFlicker(FlickeringLightAuthoring lightToChange, FlickeringLight settingsToApply)
    {
        //lightToChange = settingsToApply;

        lightToChange.TimeRangeBeforeChangingTarget = settingsToApply.TimeRangeBeforeChangingTarget;
        lightToChange.LightIntensityRange = settingsToApply.LightIntensityRange;
        lightToChange.LightIntensityChangeStep = settingsToApply.LightIntensityChangeStep;
        lightToChange.LightIntensityChangeSpeed = settingsToApply.LightIntensityChangeSpeed;
        lightToChange.ChangeTargetTimer = settingsToApply.ChangeTargetTimer;
    }
}
