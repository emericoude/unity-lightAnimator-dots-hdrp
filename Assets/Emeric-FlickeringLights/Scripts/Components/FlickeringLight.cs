using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Emeric {

    public enum FlickeringLightSettings
    {
        Custom,
        Alert,
        Ambient,
        Off,
        Spooky,
    }
    
    //[GenerateAuthoringComponent]
    [Serializable]
    public struct FlickeringLight : IComponentData
    {             
        public float2 TimeRangeBeforeChangingTarget; // The time before changing the target intensity. Random within this range.
        public float2 LightIntensityRange;           // The target light intensity. Random within this range.
        public float LightIntensityChangeStep;       // A step added to the target intensity on change, used to add contrast. (e.g. Step = 3, Intensity goes from 3 to 5, step is added 5 to 8.) Value stays contained within the IntensityRange
        public float LightIntensityChangeSpeed;      // The amount of light intensity it can travel per second.

        public float ChangeTargetTimer; //Set at runtime by a random value from TimerangeBeforeChangingTarget. You can use this if you want to set a delay when the light setting is changed.

        public bool SetColor; //Whether to change the color
        public float4 ColorToSet; //The color to set if SetColor is true.

        //Do not change
        public float TargetIntensity; //Changed Internally
        public float TargetVolume; //Changed Internally

        public FlickeringLight(float2 timeRange, float2 lightIntensityRange, float step, float speed, float timer = 0f, bool setColor = false, float4 colorToSet = default)
        {
            TimeRangeBeforeChangingTarget = timeRange;
            LightIntensityRange = lightIntensityRange;
            LightIntensityChangeStep = step;
            LightIntensityChangeSpeed = speed;

            ChangeTargetTimer = timer;

            SetColor = setColor;
            ColorToSet = colorToSet;

            TargetIntensity = 10f;
            TargetVolume = 1f;
        }

        #region CASTING TO AUTHORING COUNTERPART
        //Honestly not even sure if that's legal

        public static void CopyFromNonAuthoring(FlickeringLight nonAuthoringSource, ref FlickeringLightAuthoring authoringTarget){
            authoringTarget.TimeRangeBeforeChangingTarget = nonAuthoringSource.TimeRangeBeforeChangingTarget;
            authoringTarget.LightIntensityRange = nonAuthoringSource.LightIntensityRange;
            authoringTarget.LightIntensityChangeStep = nonAuthoringSource.LightIntensityChangeStep;
            authoringTarget.LightIntensityChangeSpeed = nonAuthoringSource.LightIntensityChangeSpeed;

            authoringTarget.ChangeTargetTimer = nonAuthoringSource.ChangeTargetTimer;

            authoringTarget.SetColor = nonAuthoringSource.SetColor;
            //authoringTarget.ColorToSet = nonAuthoringSource.ColorToSet;

            authoringTarget.TargetIntensity = nonAuthoringSource.TargetIntensity;
            authoringTarget.TargetVolume = nonAuthoringSource.TargetVolume;
        }

        public static FlickeringLight CopyFromAuthoring(FlickeringLightAuthoring source){
            FlickeringLight temp = new FlickeringLight(
                source.TimeRangeBeforeChangingTarget,
                source.LightIntensityRange,
                source.LightIntensityChangeStep,
                source.LightIntensityChangeSpeed,
                source.ChangeTargetTimer,
                source.SetColor,
                new float4(
                    source.ColorToSet.r,
                    source.ColorToSet.g,
                    source.ColorToSet.b,
                    source.ColorToSet.a
                )
            );

            return temp;
        }

        #endregion
        #region PRESETS

        public static FlickeringLight Alert(float timeBeforeInitiatingFlicker = 0f, bool changeColor = false, float4 color = default)
        {
            return new FlickeringLight(
                timeRange:              new float2(0.5f, 0.5f),
                lightIntensityRange:    new float2(0f, 15f),
                step:                   15f,
                speed:                  99f,
                timer:                  timeBeforeInitiatingFlicker,
                setColor:               changeColor,
                colorToSet:             color
            );
        }

        public static FlickeringLight Ambient(float timeBeforeInitiatingFlicker = 0f, bool changeColor = false, float4 color = default)
        {
            return new FlickeringLight(
                timeRange:              new float2(0.3f, 0.5f),
                lightIntensityRange:    new float2(13f, 15f),
                step:                   0f,
                speed:                  1f,
                timer:                  timeBeforeInitiatingFlicker,
                setColor:               changeColor,
                colorToSet:             color
            );
        }
        
        public static FlickeringLight Off(float timeBeforeInitiatingFlicker = 0f, bool changeColor = false, float4 color = default, float turnOffSpeed = 99f)
        {
            return new FlickeringLight(
                timeRange:              new float2(0f, 0f),
                lightIntensityRange:    new float2(0f, 0f),
                step:                   0f,
                speed:                  turnOffSpeed,
                timer:                  timeBeforeInitiatingFlicker,
                setColor:               changeColor,
                colorToSet:             color
            );
        }

        public static FlickeringLight Spooky(float timeBeforeInitiatingFlicker = 0f, bool changeColor = false, float4 color = default)
        {
            return new FlickeringLight(
                timeRange:              new float2(0.01f, 0.05f),
                lightIntensityRange:    new float2(-8f, 8f),
                step:                   2f,
                speed:                  10f,
                timer:                  timeBeforeInitiatingFlicker,
                setColor:               changeColor,
                colorToSet:             color
            );
        }



        #endregion
    }

}