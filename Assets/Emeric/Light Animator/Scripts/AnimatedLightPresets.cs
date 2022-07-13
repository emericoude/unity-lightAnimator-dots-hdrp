using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Emeric.LightAnimator
{
    [CreateAssetMenu(fileName = "New Light Animation Preset List", menuName = "Scriptable Objects/Light Animator/Preset List")]
    [FilePath("Emeric/Light Animator/Presets", FilePathAttribute.Location.ProjectFolder)]
    public class AnimatedLightPresets : ScriptableSingleton<AnimatedLightPresets>
    {        
        public IEnumerable<AnimatedLightPreset> Presets { get { return _presets; } }
        [SerializeField] private List<AnimatedLightPreset> _presets;
        
        /*
        [HideInInspector] public static List<string> PresetNames { get; private set; } = new List<string>();

        private void OnValidate()
        {
            List<string> presetNames = new List<string>();
            
            for (int i = 0; i < Presets.Count; i++)
            {
                presetNames.Add(Presets[i].name);
            }

            PresetNames = presetNames;
        }
        */
    }
}
