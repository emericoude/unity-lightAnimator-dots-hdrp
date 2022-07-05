using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable] [GenerateAuthoringComponent]
public struct FlickeringLight : IComponentData
{                                                // Default values suggestion
    public float2 TimeRangeBeforeChangingTarget; // 0.01f to 0.05f
    public float2 LightIntensityRange;           // 0f to 50f
    public float LightIntensityChangeStep;       // 0f
    public float LightIntensityChangeSpeed;      // 1f

    //Do not change
    public float ChangeTargetTimer;
    public float TargetIntensity;
    public float TargetVolume;

    public FlickeringLight(float2 timeRange, float2 lightIntensityRange, float step, float speed, float timer)
    {
        TimeRangeBeforeChangingTarget = timeRange;
        LightIntensityRange = lightIntensityRange;
        LightIntensityChangeStep = step;
        LightIntensityChangeSpeed = speed;
        ChangeTargetTimer = timer;
        TargetIntensity = 10f;
        TargetVolume = 1f;
    }

    #region PRESETS

    public static FlickeringLight Ambient(float timeBeforeInitiatingFlicker = 0f)
    {
        return new FlickeringLight(
            new float2(0.05f, 0.1f),
            new float2(8f, 12f),
            0f,
            5f,
            timeBeforeInitiatingFlicker
        );
    }

    public static FlickeringLight Spooky(float timeBeforeInitiatingFlicker = 0f)
    {
        return new FlickeringLight(
            new float2(0.01f, 0.05f),
            new float2(-10f, 8f), 
            2f, 
            10f, 
            timeBeforeInitiatingFlicker 
        );
    }

    public static FlickeringLight Alert(float timeBeforeInitiatingFlicker = 0f)
    {
        return new FlickeringLight(
            new float2(0.5f, 0.5f),
            new float2(0f, 15f),
            15f,
            99f,
            timeBeforeInitiatingFlicker
        );
    }

    #endregion
}
