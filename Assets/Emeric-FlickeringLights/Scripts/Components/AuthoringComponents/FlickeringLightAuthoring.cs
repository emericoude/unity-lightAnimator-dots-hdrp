using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Emeric{
    [DisallowMultipleComponent]
    [SerializableAttribute]
    public class FlickeringLightAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {        
        //Select preset from enum
        [Title("Choose a preset or custom setting.")]
        [EnumToggleButtons]
        public FlickeringLightSettings Presets;
        [SerializeField] 
        [HideInInspector]
        private FlickeringLightSettings _previousPreset;

        //Save the custom settings in a var to re-use in case we switch to a preset.
        [HideInInspector]
        public FlickeringLight CustomSetting;

        [Tooltip("The light intensity target will be defined from this range and cannot go outside of this range.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [MinMaxSlider(-50f, 50f, true)]
        [RegisterBinding(typeof(FlickeringLight), "LightIntensityRange.x", true)]
        [RegisterBinding(typeof(FlickeringLight), "LightIntensityRange.y", true)]
        public Vector2 LightIntensityRange;

        [Tooltip("This is added or substracted to or from the target intensity when it is changed.\n\nIt is added if the new target intensity has a larger value than the old; it is substracte if the new target intensity has a smaller value than the old.\n\nThis cannot go outside of the light intensity range.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [PropertyRange(0f, "@this.PossibleStepRange()")]
        [RegisterBinding(typeof(FlickeringLight), "LightIntensityChangeStep")]
        public float LightIntensityChangeStep;

        [Tooltip("The amount of light intensity that can be updated per second.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [RegisterBinding(typeof(FlickeringLight), "LightIntensityChangeSpeed")]
        public float LightIntensityChangeSpeed;

        [Tooltip("If true, the light's color will be changed to the color to set.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [RegisterBinding(typeof(FlickeringLight), "SetColor")]
        public bool SetColor;

        [Tooltip("The color to set the light to.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [ShowIf("SetColor")]
        [RegisterBinding(typeof(FlickeringLight), "ColorToSet.r", true)]
        [RegisterBinding(typeof(FlickeringLight), "ColorToSet.g", true)]
        [RegisterBinding(typeof(FlickeringLight), "ColorToSet.b", true)]
        [RegisterBinding(typeof(FlickeringLight), "ColorToSet.a", true)]
        public Color ColorToSet;

        [Tooltip("Used to create a random timer within this range.\n\nWhen the timer reaches 0, the target light intensity is changed.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [RegisterBinding(typeof(FlickeringLight), "TimeRangeBeforeChangingTarget.x", true)]
        [RegisterBinding(typeof(FlickeringLight), "TimeRangeBeforeChangingTarget.y", true)]
        [MinMaxSlider(0f, 2f, true)]
        public Vector2 TimeRangeBeforeChangingTarget;

        [Tooltip("Used as an initial wait time before values start updating.")]
        [ShowIfGroup("Presets", FlickeringLightSettings.Custom)]
        [LabelText("Initial Timer")]
        [RegisterBinding(typeof(FlickeringLight), "ChangeTargetTimer")]
        public float ChangeTargetTimer;

        [HideInInspector]
        [RegisterBinding(typeof(FlickeringLight), "TargetIntensity")]
        public float TargetIntensity;

        [HideInInspector]
        [RegisterBinding(typeof(FlickeringLight), "TargetVolume")]
        public float TargetVolume;

        #region EDITOR STUFF

        public void OnValidate()
        {            
            //If entering custom, load from custom data
            //If entering preset, set that preset
            if (Presets != _previousPreset){
                switch (Presets){
                    case FlickeringLightSettings.Custom:
                        CopyFromNonAuthoringToThis(CustomSetting);
                        break;
                    case FlickeringLightSettings.Alert:
                        CopyFromNonAuthoringToThis(FlickeringLight.Alert());
                        break;
                    case FlickeringLightSettings.Ambient:
                        CopyFromNonAuthoringToThis(FlickeringLight.Ambient());
                        break;
                    case FlickeringLightSettings.Off:
                        CopyFromNonAuthoringToThis(FlickeringLight.Off());
                        break;
                    case FlickeringLightSettings.Spooky:
                        CopyFromNonAuthoringToThis(FlickeringLight.Spooky());
                        break;
                    default:
                        Debug.LogError("Missing implementation for this preset.");
                        break;
                }

                _previousPreset = Presets;
            }

            //If Custom, save to custom data
            if (Presets == FlickeringLightSettings.Custom){
                CustomSetting = FlickeringLight.CopyFromAuthoring(this);
            }
        }

        public void CopyFromNonAuthoringToThis(FlickeringLight nonAuthoringSource){
            TimeRangeBeforeChangingTarget = nonAuthoringSource.TimeRangeBeforeChangingTarget;
            LightIntensityRange = nonAuthoringSource.LightIntensityRange;
            LightIntensityChangeStep = nonAuthoringSource.LightIntensityChangeStep;
            LightIntensityChangeSpeed = nonAuthoringSource.LightIntensityChangeSpeed;

            ChangeTargetTimer = nonAuthoringSource.ChangeTargetTimer;

            SetColor = nonAuthoringSource.SetColor;
            ColorToSet = new Color(
                nonAuthoringSource.ColorToSet.x,
                nonAuthoringSource.ColorToSet.y,
                nonAuthoringSource.ColorToSet.z,
                nonAuthoringSource.ColorToSet.w
            );

            TargetIntensity = nonAuthoringSource.TargetIntensity;
            TargetVolume = nonAuthoringSource.TargetVolume;
        }

        #endregion

        public void Convert(Entity __entity, EntityManager __dstManager, GameObjectConversionSystem __conversionSystem)
        {
            Emeric.FlickeringLight component = default(Emeric.FlickeringLight);
            component.TimeRangeBeforeChangingTarget = TimeRangeBeforeChangingTarget;
            component.LightIntensityRange = LightIntensityRange;
            component.LightIntensityChangeStep = LightIntensityChangeStep;
            component.LightIntensityChangeSpeed = LightIntensityChangeSpeed;
            component.ChangeTargetTimer = ChangeTargetTimer;
            component.SetColor = SetColor;
            component.ColorToSet = new float4(
                ColorToSet.r,
                ColorToSet.g,
                ColorToSet.b,
                ColorToSet.a
            );
            component.TargetIntensity = TargetIntensity;
            component.TargetVolume = TargetVolume;
            __dstManager.AddComponentData(__entity, component);
        }

        private float PossibleStepRange(){
            return (Mathf.Abs(LightIntensityRange.x) + Mathf.Abs(LightIntensityRange.y));
        }
    }
}


