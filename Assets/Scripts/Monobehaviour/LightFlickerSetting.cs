using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Light Flicker Setting", menuName = "Data/Light Flicker Setting")]
public class LightFlickerSetting : ScriptableObject
{
    [Header("Time Before Change\nThe min and maximum time before the light's target changes.")]
    public float MinTimeBeforeChange = 0.01f;
    public float MaxTimeBeforeChange = 0.05f;

    [Header("Light Intensity\nThe min and max light intensity the light can be at.")]
    public float MinLightIntensity = 0f;
    public float MaxLightIntensity = 50f;

    [Header("Change Speed\nThe speed at which the light will change.")]
    public float ChangeSpeed = 1f;

    [Header("Change Step\nIf higher than 0, the new target light intensity will be pushed by\nthis value towards its minimum or maximum,\n based on direction (clamped within min/max).\n\nUse this if you want to have more contrast in the light changes.")]
    public float ChangeStep = 0f;

    [Header("Volume Modifier\nThe buzz volume will be multipled by this value.")]
    [Range(0f, 2f)] public float VolumeModifier = 1f;
}
