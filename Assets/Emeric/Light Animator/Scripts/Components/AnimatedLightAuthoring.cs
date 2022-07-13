using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using System;
using Sirenix.OdinInspector;

namespace Emeric.LightAnimator
{
    //[global::System.Runtime.CompilerServices.CompilerGenerated]
    [DisallowMultipleComponent]
    [Serializable]
    public class AnimatedLightAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        #region Menu Navigation

        private enum SettingOptions
        {
            Custom,
            Preset,
            PresetWithOverride
        }

        [Flags]
        private enum PresetOverrideOptions
        {
            IntensityRange = 1,
            IntensitySpeedRange = 2,
            IntensityStepRange = 4,

            ColorChangeMethod = 8,
            SetColor = 16,

            TimerRange = 32,
            InitialTimer = 64
        }

        private enum ColorChangeMethods
        {
            Set,
            Lerp
        }

        [BoxGroup("Preset or custom?")]
        [EnumToggleButtons]
        [HideLabel]
        [SerializeField] private SettingOptions _settingOptions;

        [BoxGroup("Select a preset", centerLabel: true)]
        [HideIf("_settingOptions", SettingOptions.Custom)]
        [ValueDropdown("@AnimatedLightPresets.instance.Presets")]
        [HideLabel]
        [SerializeField] private AnimatedLightPreset _preset;

        [BoxGroup("Select options to override", centerLabel: true)]
        [EnumToggleButtons]
        [HideLabel]
        [ShowIfGroup("Select options to override/_settingOptions", Value = SettingOptions.PresetWithOverride)]
        [SerializeField] private PresetOverrideOptions _overrides;

        #endregion
        #region Overrides

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.IntensityRange)")]
        [LabelText("Intensity Range")]
        [PropertyTooltip("This overrides the preset's light intensity range.\n\nThe light intensity target will be defined from this range and cannot go outside of this range.")]
        [MinMaxSlider(-50f, 50f, true)]
        [SerializeField] private Vector2 OverrideLightIntensityRange;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.IntensitySpeedRange)")]
        [LabelText("Speed Range")]
        [PropertyTooltip("This overrides the preset's light intensity change speed range.\n\nThe amount of light intensity that can change per second. Each time a new target intensity is assigned, a random speed value from this range is assigned.\n\nThe max value of this field equals to your intensity range in 0.1 second.")]
        [MinMaxSlider(0f, "@this.LightIntensityRangeAbsolute() * 10", true)]
        [SerializeField] private Vector2 OverrideLightIntensityChangeSpeedRange;

        [HideInInspector]
        [SerializeField] private float OverrideLightIntensityChangeSpeed;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.IntensityStepRange)")]
        [LabelText("Step Range")]
        [PropertyTooltip("This overrides the preset's light intensity change step range.\n\nA step added to or substracted from the target intensity.\n\nEach time a new target intensity is assigned, a random step value from this range is assigned.\n\nThe max value of this field equals your intensity range.")]
        [MinMaxSlider(0f, "@this.LightIntensityRangeAbsolute()", true)]
        [SerializeField] private Vector2 OverrideLightIntensityChangeStep;

        [HideInInspector]
        [SerializeField] private bool OverrideSetColor;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.ColorChangeMethod)")]
        [LabelText("Color Change Method")]
        [PropertyTooltip("This overrides the preset's color change method.\n\nSet: Sets the color on update.\n\nLerp: Changes the color gradually.")]
        [EnumToggleButtons]
        [SerializeField] private ColorChangeMethods OverrideLerpToColor;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.SetColor)")]
        [HideLabel]
        [PropertyTooltip("This overrides the preset's color.\n\nThe color for the light to target.")]
        [SerializeField] private Color OverrideColorToSet = Color.white;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.TimerRange)")]
        [LabelText("Time Change Range")]
        [PropertyTooltip("This overrides the preset's time range before chaning target.\n\nWhen this timer reaches 0, targets are changed.\n\nEach time a new target intensity is assigned, a random timer is assigned from this range.")]
        [MinMaxSlider(0f, 2f, true)]
        [SerializeField] private Vector2 OverrideTimeRangeBeforeChangingTarget;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.InitialTimer)")]
        [LabelText("Initial Timer")]
        [PropertyTooltip("This overrides the preset's initial timer.\n\nThe timer will be set to this value when the light is assigne this setting. Use this if you need lights with similar settings that need to alternate (e.g. police).")]
        [SerializeField] private float OverrideChangeTargetTimer;

        [HideInInspector]
        [SerializeField] private float OverrideTargetIntensity;

        #endregion
        #region Custom Settings

        //Custom Settings

        [BoxGroup("Custom Settings")]
        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        public AnimatedLight CustomSettings;

        /*
        [BoxGroup("Custom Settings")]
        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityRange.x", true)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityRange.y", true)]
        public float2 LightIntensityRange;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityChangeSpeedRange.x", true)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityChangeSpeedRange.y", true)]
        public float2 LightIntensityChangeSpeedRange;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityChangeSpeed")]
        public float LightIntensityChangeSpeed;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityChangeStep.x", true)]
        [RegisterBinding(typeof(AnimatedLight), "LightIntensityChangeStep.y", true)]
        public float2 LightIntensityChangeStep;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "SetColor")]
        public bool SetColor;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "LerpToColor")]
        public bool LerpToColor;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "ColorToSet.x", true)]
        [RegisterBinding(typeof(AnimatedLight), "ColorToSet.y", true)]
        [RegisterBinding(typeof(AnimatedLight), "ColorToSet.z", true)]
        [RegisterBinding(typeof(AnimatedLight), "ColorToSet.w", true)]
        public float4 ColorToSet;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "TimeRangeBeforeChangingTarget.x", true)]
        [RegisterBinding(typeof(AnimatedLight), "TimeRangeBeforeChangingTarget.y", true)]
        public float2 TimeRangeBeforeChangingTarget;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "ChangeTargetTimer")]
        public float ChangeTargetTimer;

        [ShowIfGroup("Custom Settings/_settingOptions", SettingOptions.Custom)]
        [RegisterBinding(typeof(AnimatedLight), "TargetIntensity")]
        public float TargetIntensity;
        */

        #endregion

        public void Convert(Entity __entity, EntityManager __dstManager, GameObjectConversionSystem __conversionSystem)
        {
            AnimatedLight component = default(AnimatedLight);
            /*
            component.LightIntensityRange = 1f;//LightIntensityRange;
            component.TargetIntensity = TargetIntensity;
            component.LightIntensityChangeSpeedRange = LightIntensityChangeSpeedRange;
            component.LightIntensityChangeSpeed = LightIntensityChangeSpeed;
            component.LightIntensityChangeStep = LightIntensityChangeStep;
            component.SetColor = SetColor;
            component.LerpToColor = LerpToColor;
            component.ColorToSet = ColorToSet;
            component.TimeRangeBeforeChangingTarget = TimeRangeBeforeChangingTarget;
            component.ChangeTargetTimer = ChangeTargetTimer;
            */
            __dstManager.AddComponentData(__entity, component);
        }

        private float LightIntensityRangeAbsolute()
        {
            return (Mathf.Abs(OverrideLightIntensityRange.x) + Mathf.Abs(OverrideLightIntensityRange.y));
        }
    }
}