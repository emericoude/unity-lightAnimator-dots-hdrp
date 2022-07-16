using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Emeric.LightAnimator
{
    public enum ColorChangeMethods
    {
        Keep,
        Set,
        Lerp
    }

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
        [LabelText("@Constants.Label_LightIntensityRange")]
        [PropertyTooltip("@Constants.Tooltip_LightIntensityRange")]
        [MinMaxSlider(-50f, 50f, true)]
        [SerializeField] private Vector2 OverrideLightIntensityRange;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.IntensitySpeedRange)")]
        [LabelText("@Constants.Label_LightIntensitySpeedRange")]
        [PropertyTooltip("@Constants.Tooltip_LightIntensitySpeedRange")]
        [MinMaxSlider(0f, "@this.GetLightIntensityRange() * 10", true)]
        [SerializeField] private Vector2 OverrideLightIntensityChangeSpeedRange;

        [HideInInspector]
        [SerializeField] private float OverrideLightIntensityChangeSpeed;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.IntensityStepRange)")]
        [LabelText("@Constants.Label_LightIntensityChangeStepRange")]
        [PropertyTooltip("@Constants.Tooltip_LightIntensityChangeStepRange")]
        [MinMaxSlider(0f, "@this.GetLightIntensityRange()", true)]
        [SerializeField] private Vector2 OverrideLightIntensityChangeStep;

        [HideInInspector]
        [SerializeField] private bool OverrideSetColor;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.ColorChangeMethod)")]
        [LabelText("@Constants.Label_ColorChangeMethod")]
        [PropertyTooltip("@Constants.Tooltip_ColorChangeMethod")]
        [EnumToggleButtons]
        [SerializeField] private ColorChangeMethods OverrideLerpToColor;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@(this._settingOptions == SettingOptions.PresetWithOverride) && (this._overrides.HasFlag(PresetOverrideOptions.SetColor)) && (this.OverrideLerpToColor != ColorChangeMethods.Keep)")]
        [HideLabel]
        [PropertyTooltip("@Constants.Tooltip_ColorToSet")]
        [SerializeField] private Color OverrideColorToSet = Color.white;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.TimerRange)")]
        [LabelText("@Constants.Label_TimerRange")]
        [PropertyTooltip("@Constants.Tooltip_TimerRange")]
        [MinMaxSlider(0f, 2f, true)]
        [SerializeField] private Vector2 OverrideTimeRangeBeforeChangingTarget;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.InitialTimer)")]
        [LabelText("@Constants.Label_ChangeTargetTimer")]
        [PropertyTooltip("@Constants.Tooltip_ChangeTargetTimer")]
        [SerializeField] private float OverrideChangeTargetTimer;

        [HideInInspector]
        [SerializeField] private float OverrideTargetIntensity;

        #endregion
        #region Custom Settings

        //Custom Settings

        //[BoxGroup("Custom Settings")]
        [ShowIf("@this._settingOptions == SettingOptions.Custom")]
        public AnimatedLight CustomSettings;

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

        private float GetLightIntensityRange()
        {
            float range;

            if (_overrides.HasFlag(PresetOverrideOptions.IntensityRange))
            {
                range = OverrideLightIntensityRange.GetRange();
            }
            else
            {
                range = _preset.Settings.LightIntensityRange.GetRange();
            }

            return range;
        }
    }
}