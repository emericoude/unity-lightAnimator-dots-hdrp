using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Emeric.LightAnimator
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Preset List", menuName = "Scriptable Objects/Light Animator/Preset List")]
    public class AnimatedLightPresets : SingletonScriptableObject<AnimatedLightPresets>
    {        
        public List<AnimatedLightPreset> Presets { get { return _presets; } }
        [SerializeField] private List<AnimatedLightPreset> _presets;

        private void OnValidate()
        {
            _presets.RemoveAll(AnimatedLightPreset => AnimatedLightPreset == null);
        }

        public void AddPresetElement(AnimatedLightPreset element)
        {
            _presets.Add(element);
        }
    }
}
