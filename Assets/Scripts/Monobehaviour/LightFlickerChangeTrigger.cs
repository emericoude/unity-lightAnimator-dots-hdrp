using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerChangeTrigger : MonoBehaviour
{
    [SerializeField] private bool _disableSelfOnTrigger = true;
    [SerializeField] private LightFlickerSetting _lightFlickerToSet;
    [SerializeField] private List<PoweredLight> _lightsToFlicker = new List<PoweredLight>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            if (_lightsToFlicker.Count > 0){
                foreach (PoweredLight light in _lightsToFlicker){
                    if (light.isActiveAndEnabled){}
                        light.SetLightFlickerSettings(_lightFlickerToSet);
                }

                if (_disableSelfOnTrigger)
                    gameObject.SetActive(false);
            }
        }
    }
}
