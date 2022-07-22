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

    /// <summary>
    /// Defines the animation state that will be active on startup for an animated light.
    /// </summary>
    /// <remarks>This is the ecs-authoring component counterpart of the AnimatedLight script.</remarks>
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
        [MinMaxSlider(0f, "@this.GetLightIntensityRangeForOverride()", true)]
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

        [ShowIf("@this._settingOptions == SettingOptions.Custom")]
        public AnimatedLight CustomSettings;

        #endregion
        #region Other Functionality

        //[SerializeField] private bool DebugWithGizmos = false;

        private void OnValidate()
        {
            if (_preset == null)
                _preset = AnimatedLightPresets.Instance.Presets.FirstOrDefault();
        }

        #endregion
        #region Conversion

        /// <summary>
        /// ECS-related function that handles the convertion from Authoring to ecs-data. Logic called is based on this AnimatedLight's settings.
        /// </summary>
        /// <remarks>This is called by Unity at the proper time.</remarks>
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

        /// <summary>
        /// Logic called on convertion if the selected setting is "Custom". 
        /// </summary>
        /// <remarks>Copies the custom settings to the component.</remarks>
        /// <param name="component">The component that will be converted to data.</param>
        private void ConvertFromCustom(ref AnimatedLight component)
        {
            component = CustomSettings;
        }

        /// <summary>
        /// Logic called on convertion if the selected setting is "Preset". 
        /// </summary>
        /// <remarks>Copies the preset's settings to the component.</remarks>
        /// <param name="component">The component that will be converted to data.</param>
        private void ConvertFromPreset(ref AnimatedLight component)
        {
            component = _preset.Settings;
        }

        /// <summary>
        /// Logic called on convertion if the selected setting is "Preset With Override". 
        /// </summary>
        /// <remarks>Copies the preset's settings to the component. Then overrides any setting that should be overriden (based on options selected in inspector).</remarks>
        /// <param name="component">The component that will be converted to data.</param>
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

        /// <remarks>Used by Odin Attributes</remarks>
        /// <returns>
        /// If this animated light is set to PresetWithOverride AND:<br/>
        /// - and it is overriding the intensity range: The light intensity override's range; <br/>
        /// - and if it is NOT override the intensity range: the preset's light intensity range; <br/><br/>
        /// Otherwise: 0.
        /// </returns>
        private float GetLightIntensityRangeForOverride()
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

        /// <returns>True if this animated light is set to PresetWithOverride AND has the color override enabled; otherwise, false.</returns>
        private bool CanDisplayColorOverride()
        {
            return _settingOptions == SettingOptions.PresetWithOverride && //If in OVERRIDE menu
                    _overrides.HasFlag(PresetOverrideOptions.Color); //If COLOR OVERRIDE toggled
        }

        /// <remarks>Used by odin attributes to disable/enable the color field and display tooltips in cases where it would be useless to have this override.</remarks>
        /// <returns>
        /// True if it the color override can be displayed AND if: <br/>
        /// - This animated light is overriding the color change method and the override is set to "Keep"; <br/>
        /// OR <br/>
        /// - This animated light is not overriding color change method and the preset's setColor is false; <br/><br/>
        /// Otherwise, false.
        /// </returns>
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