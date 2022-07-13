using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;

namespace Emeric {
    public class FlickeringLightDrawer : OdinValueDrawer<FlickeringLight>
    {
        private bool _foldoutHeaderLightIntensity = true;
        private bool _foldoutHeaderTimer = true;
        private bool _isExpanded = true;
        private Color _colorToSetDisplay;

        protected override void DrawPropertyLayout(GUIContent label){
            FlickeringLight myTarget = this.ValueEntry.SmartValue;
            Rect rect = EditorGUILayout.GetControlRect();
            GUIContent content = new GUIContent();

            if (label == null){
                label = new GUIContent();
                label.text = "Flickering Light Setting";
            }
            
            SirenixEditorGUI.BeginBox(label);
            
            if (_isExpanded){
                content.text = "Light Settings";
                content.tooltip = "";
                _foldoutHeaderLightIntensity = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutHeaderLightIntensity, content);
                if (_foldoutHeaderLightIntensity){
                    #region Light Intensity Target Range

                        content.text = "Intensity Range";
                        content.tooltip = "The light intensity target will be defined from this range and cannot go outside of this range.";
                        
                        SirenixEditorFields.MinMaxSlider(
                            content,
                            ref myTarget.LightIntensityRange.x,
                            ref myTarget.LightIntensityRange.y,
                            -50f,
                            50f,
                            true
                        );

                    #endregion
                    #region Light Intensity Change Step

                        content.text = "Change Step";
                        content.tooltip = "This is added or substracted to or from the target intensity when it is changed.\n\nIt is added if the new target intensity has a larger value than the old; it is substracte if the new target intensity has a smaller value than the old.\n\nThis cannot go outside of the light intensity range.";
                        
                        myTarget.LightIntensityChangeStep = EditorGUILayout.Slider(
                            content,
                            myTarget.LightIntensityChangeStep,
                            0,
                            Mathf.Abs(myTarget.LightIntensityRange.x) + Mathf.Abs(myTarget.LightIntensityRange.y)
                        );

                    #endregion
                    #region Light Intensity Update Speed
                    
                    content.text = "Update Speed";
                    content.tooltip = "The amount of light intensity that can be updated per second.";
                    myTarget.LightIntensityChangeSpeed = SirenixEditorFields.FloatField(
                        content,
                        myTarget.LightIntensityChangeSpeed
                    );
                    
                    #endregion

                    EditorGUILayout.Space();

                    #region Set Color ?

                    content.text = "Set Color ?";
                    content.tooltip = "If true, the light's color will be changed to this color.";
                    object toggleGroup = new object();
                    myTarget.SetColor = SirenixEditorGUI.BeginToggleGroup(toggleGroup, ref myTarget.SetColor, ref myTarget.SetColor, content.text);
                    //myTarget.SetColor = EditorGUILayout.ToggleLeft(content, myTarget.SetColor);
                    
                    content.text = "";
                    content.tooltip = "The color to set when this setting is applied to a light.";
                    if (myTarget.SetColor){
                        _colorToSetDisplay = new Color(
                            myTarget.ColorToSet.x,
                            myTarget.ColorToSet.y,
                            myTarget.ColorToSet.z,
                            myTarget.ColorToSet.w
                        );

                        _colorToSetDisplay = SirenixEditorFields.ColorField(content, _colorToSetDisplay);
                        //_colorToSetDisplay = EditorGUILayout.ColorField(content, _colorToSetDisplay);
                        
                        myTarget.ColorToSet = new float4(
                            _colorToSetDisplay.r,
                            _colorToSetDisplay.g,
                            _colorToSetDisplay.b,
                            _colorToSetDisplay.a
                        );
                    }

                    SirenixEditorGUI.EndToggleGroup();

                    #endregion

                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                content.text = "Timer Settings";
                content.tooltip = "";
                _foldoutHeaderTimer = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutHeaderTimer, content);
                if (_foldoutHeaderTimer){
                    #region Initial Timer

                        content.text = "Initial Timer";
                        content.tooltip = "When changed to this setting, this is going to be the initial wait time before values start updating to this setting.";
                        myTarget.ChangeTargetTimer = SirenixEditorFields.FloatField(
                            content,
                            myTarget.ChangeTargetTimer
                        );

                    #endregion
                    #region Time Range Before Changing Target

                        content.text = "Timer Range";
                        content.tooltip = "Used to create a random timer within this range.\n\nWhen the timer reaches 0, the target light intensity is changed.";      
                        
                        SirenixEditorFields.MinMaxSlider(
                            content,
                            ref myTarget.TimeRangeBeforeChangingTarget.x,
                            ref myTarget.TimeRangeBeforeChangingTarget.y,
                            0f,
                            2f,
                            true
                        );

                    #endregion
                    
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Space();

                this.ValueEntry.SmartValue = myTarget;
            }

            SirenixEditorGUI.EndBox();
        }
            
    }
}
