using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Emeric.RandomLibrary;

namespace Emeric.LightAnimator
{   
    /// <summary>
    /// Animates all of the lights which have a AnimatedLight component attached based on their per-object settings.
    /// </summary>
    public partial class AnimatedLightSystem : SystemBase
    {
        private RandomSystem _randomSystem;
        private const float _maxLightVolume = 1f;

        protected override void OnCreate()
        {
            base.OnCreate();

            _randomSystem = World.GetExistingSystem<RandomSystem>();
        }

        protected override void OnUpdate()
        {            
            float deltaTime = Time.DeltaTime;

            UpdateAnimatedLightTargets(deltaTime);
            UpdateLights(deltaTime);
            UpdateLightsAudio();
        }

        /// <summary>
        /// AnimatedLight: Countsdown its timer and Generates new targets when its timer reaches 0.
        /// </summary>
        /// <remarks> Targets are: \n- Timer \n- Target Intensity \n- Change Speed</remarks>
        /// <param name="deltaTime">Pass in Time.DeltaTime</param>
        private void UpdateAnimatedLightTargets(float deltaTime)
        {
            var randomArray = _randomSystem.RandomArray;

            Entities
                .WithNativeDisableParallelForRestriction(randomArray)
                .ForEach((int nativeThreadIndex, ref AnimatedLight animationSettings) => {

                    animationSettings.ChangeTargetTimer -= deltaTime;

                    if (animationSettings.ChangeTargetTimer <= 0)
                    {
                        //Create a new seed
                        Unity.Mathematics.Random random = randomArray[nativeThreadIndex];

                        //Reset the timer
                        animationSettings.ChangeTargetTimer = random.NextFloat(
                            animationSettings.TimeRangeBeforeChangingTarget.x,
                            animationSettings.TimeRangeBeforeChangingTarget.y
                        );

                        randomArray[nativeThreadIndex] = random; //update seed

                        //Create new random intensity. It will be modified by other settings.
                        float randomTargetIntensity = random.NextFloat(
                            animationSettings.LightIntensityRange.x,
                            animationSettings.LightIntensityRange.y
                        );

                        randomArray[nativeThreadIndex] = random; //update seed

                        //Create a random step value.
                        float randomIntensityStep = random.NextFloat(
                            animationSettings.LightIntensityChangeStep.x,
                            animationSettings.LightIntensityChangeStep.y
                        );

                        randomArray[nativeThreadIndex] = random; //update seed

                        //Add the step in the direction of the change
                        if (randomTargetIntensity < animationSettings.TargetIntensity)
                            randomTargetIntensity -= randomIntensityStep;
                        else
                            randomTargetIntensity += randomIntensityStep;

                        //Set the target intensity (clamping within range)
                        animationSettings.TargetIntensity = Mathf.Clamp(
                            randomTargetIntensity,
                            animationSettings.LightIntensityRange.x,
                            animationSettings.LightIntensityRange.y
                        );

                        //Set a new speed until it reaches target again
                        animationSettings.LightIntensityChangeSpeed = random.NextFloat(
                            animationSettings.LightIntensityChangeSpeedRange.x,
                            animationSettings.LightIntensityChangeSpeedRange.y
                        );

                        randomArray[nativeThreadIndex] = random; //update seed
                    }

                }).ScheduleParallel();
        }

        /// <summary>
        /// Animates the lights' intensities and color based on their respective settings.
        /// </summary>
        /// <param name="deltaTime">Pass in Time.DeltaTime</param>
        private void UpdateLights(float deltaTime)
        {
            Entities
                .WithoutBurst()
                .ForEach((HDAdditionalLightData light, in AnimatedLight animationSettings) => {

                    if (animationSettings.SetColor)
                    {
                        Color targetColor = new Color(
                            animationSettings.ColorToSet.x,
                            animationSettings.ColorToSet.y,
                            animationSettings.ColorToSet.z,
                            animationSettings.ColorToSet.w
                        );

                        if (animationSettings.LerpToColor) //Lerp to the color
                        {
                            light.color = Vector4.MoveTowards(light.color, targetColor, deltaTime);
                        }
                        else //Set the color
                        {
                            light.color = targetColor;
                        }
                    }

                    light.intensity = Mathf.MoveTowards(
                        light.intensity,
                        animationSettings.TargetIntensity,
                        animationSettings.LightIntensityChangeSpeed * deltaTime
                    );

            }).Run();
        }

        /// <summary>
        /// Animates the lights' audio sources' volumes based on the light intensity.
        /// </summary>
        /// <remarks> Volume is 0 when intensity is 0. Volume is 1 when intensity is equal to its current animation settings' max intensity. </remarks>
        private void UpdateLightsAudio()
        {
            //Update light's audio volume
            Entities
                .WithoutBurst()
                .ForEach((AudioSource audio, in HDAdditionalLightData light, in AnimatedLight animationSettings) => {

                    //Get a ratio from 0 to 1 of the intensity of the light
                    float lightIntensityRatio = Mathf.InverseLerp(
                        0f,
                        animationSettings.LightIntensityRange.y,
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
