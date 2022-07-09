using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Emeric {

    public partial class FlickeringLightSystem : SystemBase
    {
        private RandomSystem _randomSystem;
        private const float _maxLightVolume = 1f;
        
        protected override void OnCreate(){
            _randomSystem = World.GetExistingSystem<RandomSystem>();
        }
        
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            var randomArray = _randomSystem.RandomArray;

            //Update light targets
            Entities
                .WithNativeDisableParallelForRestriction(randomArray)
                .ForEach((int nativeThreadIndex, ref FlickeringLight flicker) =>
                {
                    flicker.ChangeTargetTimer -= deltaTime;

                    if (flicker.ChangeTargetTimer <= 0)
                    {
                        Unity.Mathematics.Random random = randomArray[nativeThreadIndex];

                        //Get a new target intensity
                        float newTargetIntensiy = random.NextFloat(
                            flicker.LightIntensityRange.x,
                            flicker.LightIntensityRange.y
                        );

                        randomArray[nativeThreadIndex] = random; //update seed

                        //Add the step if present
                        if (newTargetIntensiy < flicker.TargetIntensity)
                            newTargetIntensiy -= flicker.LightIntensityChangeStep;
                        else
                            newTargetIntensiy += flicker.LightIntensityChangeStep;

                        //Set the target intensity and make sure it is within the min/max range
                        flicker.TargetIntensity = Mathf.Clamp(
                            newTargetIntensiy, 
                            flicker.LightIntensityRange.x, 
                            flicker.LightIntensityRange.y
                        );

                        //Reset the timer
                        flicker.ChangeTargetTimer = random.NextFloat(
                            flicker.TimeRangeBeforeChangingTarget.x,
                            flicker.TimeRangeBeforeChangingTarget.y
                        );

                        randomArray[nativeThreadIndex] = random; //update seed
                    }
                }).ScheduleParallel();

            //Update lights
            Entities
                .WithoutBurst()
                .ForEach((HDAdditionalLightData light, ref FlickeringLight flicker) => {

                    if (flicker.SetColor){
                        light.color = new Color(
                            flicker.ColorToSet.x,
                            flicker.ColorToSet.y,
                            flicker.ColorToSet.z,
                            flicker.ColorToSet.w
                        );
                    }

                    light.intensity = Mathf.MoveTowards(
                        light.intensity,
                        flicker.TargetIntensity,
                        flicker.LightIntensityChangeSpeed * deltaTime
                    );

            }).Run();

            //Update light's audio volume
            Entities
                .WithoutBurst()
                .ForEach((HDAdditionalLightData light, AudioSource audio, ref FlickeringLight flicker) => {

                    //Get a ratio from 0 to 1 of the intensity of the light
                    float lightIntensityRatio = Mathf.InverseLerp(
                        0f,
                        flicker.LightIntensityRange.y,
                        light.intensity
                    );

                    //Apply the ratio to the audio source's volume
                    //Used for ambient buzz noises
                    //I don't even think you can do that with DOTS yet
                    audio.volume = Mathf.Lerp(
                        audio.volume,
                        _maxLightVolume,
                        lightIntensityRatio
                    );

                }).Run();
        }
    }

}


