using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Emeric.LightAnimator
{
    //For more details, see my authoring component counterpart
    
    //[GenerateAuthoringComponent]
    [Serializable]
    public struct AnimatedLight : IComponentData
    {
        public float2 LightIntensityRange;
        public float TargetIntensity;
        public float2 LightIntensityChangeSpeedRange;
        public float LightIntensityChangeSpeed;
        public float2 LightIntensityChangeStep;

        public bool SetColor;
        public bool LerpToColor;
        public float4 ColorToSet;

        public float2 TimeRangeBeforeChangingTarget;
        public float ChangeTargetTimer;

        public AnimatedLight(float2 lightIntensityRange, float targetIntensity, float2 lightIntensityChangeSpeedRange, float lightIntensityChangeSpeed, float2 lightIntensityChangeStep, bool setColor, bool lerpToColor, float4 colorToSet, float2 timeRangeBeforeChangingTarget, float changeTargetTimer)
        {
            LightIntensityRange = lightIntensityRange;
            TargetIntensity = targetIntensity;
            LightIntensityChangeSpeedRange = lightIntensityChangeSpeedRange;
            LightIntensityChangeSpeed = lightIntensityChangeSpeed;
            LightIntensityChangeStep = lightIntensityChangeStep;

            SetColor = setColor;
            LerpToColor = lerpToColor;
            ColorToSet = colorToSet;

            TimeRangeBeforeChangingTarget = timeRangeBeforeChangingTarget;
            ChangeTargetTimer = changeTargetTimer;
        }
    }
}
