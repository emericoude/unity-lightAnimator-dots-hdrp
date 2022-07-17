using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Linq;

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
            Color = 16,

            TimerRange = 32,
            InitialTimer = 64
        }

        [BoxGroup("Preset or custom?")]
        [EnumToggleButtons]
        [HideLabel]
        [SerializeField] private SettingOptions _settingOptions;

        [BoxGroup("Select a preset", centerLabel: true)]
        [HideIf("_settingOptions", SettingOptions.Custom)]
        [ValueDropdown("@AnimatedLightPresets.Instance.Presets", HideChildProperties = true, NumberOfItemsBeforeEnablingSearch = 5)]
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
        [MinMaxSlider(0f, "@Constants.Value_LightIntensitySpeedRangeMax", true)]
        [SerializeField] private Vector2 OverrideLightIntensityChangeSpeedRange;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.IntensityStepRange)")]
        [LabelText("@Constants.Label_LightIntensityChangeStepRange")]
        [PropertyTooltip("@Constants.Tooltip_LightIntensityChangeStepRange")]
        [MinMaxSlider(0f, "@this.GetLightIntensityRange()", true)]
        [SerializeField] private Vector2 OverrideLightIntensityChangeStep;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this._settingOptions == SettingOptions.PresetWithOverride && this._overrides.HasFlag(PresetOverrideOptions.ColorChangeMethod)")]
        [LabelText("@Constants.Label_ColorChangeMethod")]
        [PropertyTooltip("@Constants.Tooltip_ColorChangeMethod")]
        [EnumToggleButtons]
        [SerializeField] private ColorChangeMethods OverrideLerpToColor;

        [BoxGroup("Select options to override/Overrides", centerLabel: true)]
        [ShowIf("@this.CanDisplayColorOverride()")]
        [DisableIf("@this.CanDisableColorOverride()")]
        [InfoBox("@this._overrideColorInfoBoxText", VisibleIf = "@this.CanDisableColorOverride()", InfoMessageType = InfoMessageType.Info)]
        [HideLabel]
        [PropertyTooltip("@Constants.Tooltip_ColorToSet")]
        [SerializeField] private Color OverrideColorToSet = Color.white;
        private string _overrideColorInfoBoxText = "";

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

        #endregion
        #region Custom Settings

        //Custom Settings

        //[BoxGroup("Custom Settings")]
        [ShowIf("@this._settingOptions == SettingOptions.Custom")]
        public AnimatedLight CustomSettings;

        private void OnValidate()
        {
            if (_preset == null)
                _preset = AnimatedLightPresets.Instance.Presets.FirstOrDefault();
        }

        #endregion
        #region Conversion

        public void Convert(Entity __entity, EntityManager __dstManager, GameObjectConversionSystem __conversionSystem)
        {
            AnimatedLight component = default(AnimatedLight);
            
            switch (_settingOptions)
            {
                case SettingOptions.Custom:
                    ConvertFromCustom(ref component);
                    break;
                case SettingOptions.Preset:
                    ConvertFromPreset(ref component);
                    break;
                case SettingOptions.PresetWithOverride:
                    ConvertFromPresetWithOverride(ref component);
                    break;
                default:
                    Debug.LogError("Missing implementation for setting options method.");
                    break;
            }

            __dstManager.AddComponentData(__entity, component);
        }

        private void ConvertFromCustom(ref AnimatedLight component)
        {
            component = CustomSettings;
        }

        private void ConvertFromPreset(ref AnimatedLight component)
        {
            component = _preset.Settings;
        }

        private void ConvertFromPresetWithOverride(ref AnimatedLight component)
        {
            ConvertFromPreset(ref component);
            
            if (_overrides.HasFlag(PresetOverrideOptions.IntensityRange))
                component.LightIntensityRange = OverrideLightIntensityRange;

            if (_overrides.HasFlag(PresetOverrideOptions.IntensitySpeedRange))
                component.LightIntensityChangeSpeedRange = OverrideLightIntensityChangeSpeedRange;

            if (_overrides.HasFlag(PresetOverrideOptions.IntensityStepRange))
                component.LightIntensityChangeStep = OverrideLightIntensityChangeStep;

            if (_overrides.HasFlag(PresetOverrideOptions.ColorChangeMethod))
            {
                switch (OverrideLerpToColor)
                {
                    case ColorChangeMethods.Keep:
                        component.SetColor = false;
                        break;
                    case ColorChangeMethods.Set:
                        component.SetColor = true;
                        component.LerpToColor = false;
                        break;
                    case ColorChangeMethods.Lerp:
                        component.SetColor = true;
                        component.LerpToColor = true;
                        break;
                    default:
                        Debug.LogError("Unknown enum value during conversion.");
                        break;
                }
            }

            if (_overrides.HasFlag(PresetOverrideOptions.Color))
            {                
                component.ColorToSet = new float4(
                    OverrideColorToSet.r,
                    OverrideColorToSet.g,
                    OverrideColorToSet.b,
                    OverrideColorToSet.a
                );
            }

            if (_overrides.HasFlag(PresetOverrideOptions.TimerRange))
                component.TimeRangeBeforeChangingTarget = OverrideTimeRangeBeforeChangingTarget;

            if (_overrides.HasFlag(PresetOverrideOptions.InitialTimer))
                component.ChangeTargetTimer = OverrideChangeTargetTimer;
        }

        #endregion
        #region Utilities

        private float GetLightIntensityRange()
        {
            float range = 0;

            if (_settingOptions == SettingOptions.PresetWithOverride)
            {
                if (_overrides.HasFlag(PresetOverrideOptions.IntensityRange))
                {
                    range = OverrideLightIntensityRange.GetRange();
                }
                else
                {
                    range = _preset.Settings.LightIntensityRange.GetRange();
                }
            }

            return range;
        }

        private bool CanDisplayColorOverride()
        {
            return _settingOptions == SettingOptions.PresetWithOverride && //If in OVERRIDE menu
                    _overrides.HasFlag(PresetOverrideOptions.Color); //If COLOR OVERRIDE toggled
        }

        private bool CanDisableColorOverride()
        {
            if (CanDisplayColorOverride())
            {
                if (_overrides.HasFlag(PresetOverrideOptions.ColorChangeMethod) && OverrideLerpToColor == ColorChangeMethods.Keep)
                {
                    _overrideColorInfoBoxText = Constants.InfoText_IfOverrideColorMethodKeep;
                    return true;
                }
                else if (!_overrides.HasFlag(PresetOverrideOptions.ColorChangeMethod) && !_preset.Settings.SetColor)
                {
                    _overrideColorInfoBoxText = Constants.InfoText_IfPresetSetColorFalse;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}