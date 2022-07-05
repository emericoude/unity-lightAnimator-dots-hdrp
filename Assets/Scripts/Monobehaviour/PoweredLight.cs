using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that turn on/off lights when the power generator turn on/off
/// </summary>
public class PoweredLight : Powered
{
    [Header("Light")]
    [SerializeField] protected Light _light;
    protected Color _initialLightColor;

    [Header("Light Flickering")]
    [SerializeField] protected LightFlickerSetting _defaultFlicker;
    protected LightFlickerSetting _currentFlicker;
    protected float _lightIntensityTarget;

    [Header("Audio")]
    [SerializeField] protected AudioSource _ambientSource;
    protected float _ambientVolumeTarget;
    protected const float _maxAmbientVolume = 1f;
    protected const float _ambientVolumeChangeSpeed = 30f;
    protected const float _maxLightValue = 30f;

    protected void Awake()
    {
        //base.Awake();

        if (_light == null)
            _light = GetComponent<Light>();

        if (_ambientSource == null)
            _ambientSource = GetComponent<AudioSource>();

        _initialLightColor = _light.color;

        if (_ambientSource != null){
            _ambientSource.playOnAwake = false;
            StartCoroutine(PlayBuzzSourceOnAwakeRandom());
        }
    }

    protected virtual void Start(){
        if (_defaultFlicker != null){
            ResetLightFlicker();
            StartCoroutine(FlickeringRepeating());
        }
    }

    protected virtual void Update()
    {
        if (_currentFlicker != null){
            _light.intensity = Mathf.MoveTowards(_light.intensity, _lightIntensityTarget, _currentFlicker.ChangeSpeed * Time.deltaTime);
            
            if (_ambientSource != null)
                _ambientSource.volume = Mathf.MoveTowards(_ambientSource.volume, _ambientVolumeTarget, _ambientVolumeChangeSpeed * Time.deltaTime);
        }
    }

    public override void PowerOff(){}

    public override void PowerOn(){}

    protected IEnumerator FlickeringRepeating(){
        float newTarget = UnityEngine.Random.Range(_currentFlicker.MinLightIntensity, _currentFlicker.MaxLightIntensity);

        if (newTarget < _lightIntensityTarget)
            newTarget -= _currentFlicker.ChangeStep;
        else 
            newTarget += _currentFlicker.ChangeStep;

        _lightIntensityTarget = Mathf.Clamp(newTarget, _currentFlicker.MinLightIntensity, _currentFlicker.MaxLightIntensity);

        float lightIntensityRatio = Mathf.InverseLerp(0f, _maxLightValue, _light.intensity);
        _ambientVolumeTarget = Mathf.Lerp(0f, _maxAmbientVolume, lightIntensityRatio);
        _ambientVolumeTarget *= _currentFlicker.VolumeModifier;
        //Debug.Log($"Ratio: {lightIntensityRatio}, target: {_ambientVolumeTarget}");

        float randomWaitTime = UnityEngine.Random.Range(_currentFlicker.MinTimeBeforeChange, _currentFlicker.MaxTimeBeforeChange);
        yield return new WaitForSeconds(randomWaitTime);
        
        StartCoroutine(FlickeringRepeating());
    }

    private IEnumerator PlayBuzzSourceOnAwakeRandom(){
        float randomTime = UnityEngine.Random.Range(0f, 1f);

        yield return new WaitForSeconds(randomTime);

        _ambientSource.Play();
    }

    protected virtual IEnumerator CoSetLightFlickerSettingsOverDuration(LightFlickerSetting newFlicker, float duration, LightFlickerSetting flickerToResetToAtEnd = null){
        _currentFlicker = newFlicker;

        yield return new WaitForSeconds(duration);

        if (flickerToResetToAtEnd == null)
            ResetLightFlicker();
        else
            _currentFlicker = flickerToResetToAtEnd;
    }

    #region UTILITIES

        public virtual void SetLightFlickerSettings(LightFlickerSetting newFlicker){
            _currentFlicker = newFlicker;
        }

        public virtual void ResetLightFlicker(){
            _currentFlicker = _defaultFlicker;
        }

        public void SetLightColor(Color color){
            if (_light != null)
                _light.color = color;
        }

        public void ResetLightColor(){
            if (_light != null)
                _light.color = _initialLightColor;
        }

        public void SetLightFlickerSettingsOverDuration(LightFlickerSetting newFlicker, float duration, LightFlickerSetting flickerToResetToAtEnd = null){
            StartCoroutine(CoSetLightFlickerSettingsOverDuration(newFlicker, duration, flickerToResetToAtEnd));
        }

    #endregion
}
