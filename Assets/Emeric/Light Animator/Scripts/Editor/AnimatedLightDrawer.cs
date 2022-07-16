using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using Sirenix.OdinInspector.Editor.Drawers;


namespace Emeric.LightAnimator
{
    public class AnimatedLightDrawer : OdinValueDrawer<AnimatedLight>
    {
        private ColorChangeMethods _colorChangeMethod;
        private Color _color;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            AnimatedLight myTarget = this.ValueEntry.SmartValue;
            GUIContent content = new GUIContent();

            if (label == null)
            {
                label = new GUIContent();
                label.text = "Animation State Settings";
            }

            SirenixEditorGUI.BeginBox(label);

            //INTENSITY RANGE
            content.text = Constants.Label_LightIntensityRange;
            content.tooltip = Constants.Tooltip_LightIntensityRange;
            SirenixEditorFields.MinMaxSlider(
                content,
                ref myTarget.LightIntensityRange.x,
                ref myTarget.LightIntensityRange.y,
                -50f,
                50f,
                true
            );

            //SPEED RANGE
            content.text = Constants.Label_LightIntensitySpeedRange;
            content.tooltip = Constants.Tooltip_LightIntensitySpeedRange;
            SirenixEditorFields.MinMaxSlider(
                content,
                ref myTarget.LightIntensityChangeSpeedRange.x,
                ref myTarget.LightIntensityChangeSpeedRange.y,
                0f,
                myTarget.LightIntensityRange.GetRange() * 10
            );

            //STEP RANGE
            content.text = Constants.Label_LightIntensityChangeStepRange;
            content.tooltip = Constants.Tooltip_LightIntensityChangeStepRange;
            SirenixEditorFields.MinMaxSlider(
                content,
                ref myTarget.LightIntensityChangeStep.x,
                ref myTarget.LightIntensityChangeStep.y,
                0f,
                myTarget.LightIntensityRange.GetRange()
            );

            //COLOR METHOD

            if (myTarget.SetColor == false)
            {
                _colorChangeMethod = ColorChangeMethods.Keep;
            }
            else if (myTarget.LerpToColor == false)
            {
                _colorChangeMethod = ColorChangeMethods.Set;
            }
            else
            {
                _colorChangeMethod = ColorChangeMethods.Lerp;
            }

            content.text = Constants.Label_ColorChangeMethod;
            content.tooltip = Constants.Tooltip_ColorChangeMethod;
            _colorChangeMethod = (ColorChangeMethods)SirenixEditorFields.EnumDropdown(content, _colorChangeMethod);
            switch (_colorChangeMethod)
            {
                case ColorChangeMethods.Keep:
                    myTarget.SetColor = false;
                    break;
                case ColorChangeMethods.Set:
                    myTarget.SetColor = true;
                    myTarget.LerpToColor = false;
                    break;
                case ColorChangeMethods.Lerp:
                    myTarget.SetColor = true;
                    myTarget.LerpToColor = true;
                    break;
                default:
                    SirenixEditorGUI.ErrorMessageBox("Missing implementation for Color Change Method.");
                    break;
            }

            //COLOR FIELD
            if (_colorChangeMethod != ColorChangeMethods.Keep)
            {
                content.text = Constants.Label_ColorToSet;
                content.tooltip = Constants.Tooltip_ColorToSet;

                _color = new Color(
                    myTarget.ColorToSet.x,
                    myTarget.ColorToSet.y,
                    myTarget.ColorToSet.z,
                    myTarget.ColorToSet.w
                );

                _color = SirenixEditorFields.ColorField(content, _color);

                myTarget.ColorToSet = new float4(
                    _color.r,
                    _color.g,
                    _color.b,
                    _color.a
                );
            }

            //TIMER RANGE
            content.text = Constants.Label_TimerRange;
            content.tooltip = Constants.Tooltip_TimerRange;
            SirenixEditorFields.MinMaxSlider(
                content,
                ref myTarget.TimeRangeBeforeChangingTarget.x,
                ref myTarget.TimeRangeBeforeChangingTarget.y,
                0f,
                2f
            );

            //Initial Timer
            content.text = Constants.Label_ChangeTargetTimer;
            content.tooltip = Constants.Tooltip_ChangeTargetTimer;
            myTarget.ChangeTargetTimer = SirenixEditorFields.FloatField(content, myTarget.ChangeTargetTimer);

            this.ValueEntry.SmartValue = myTarget;
            SirenixEditorGUI.EndBox();
        }
    }
}
