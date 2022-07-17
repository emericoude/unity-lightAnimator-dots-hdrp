using UnityEngine;
using Sirenix.OdinInspector;

namespace Emeric.LightAnimator
{
    [CreateAssetMenu(fileName = "New Light Animation Preset", menuName = "Scriptable Objects/Light Animator/Preset")]
    public class AnimatedLightPreset : ScriptableObject
    {
        public AnimatedLight Settings { get { return _settings; } private set { _settings = value; } }
        [SerializeField] private AnimatedLight _settings;

        private void Awake()
        {
            if (!AnimatedLightPresets.Instance.Presets.Contains(this))
            {
                AnimatedLightPresets.Instance.AddPresetElement(this);
            }
        }
    }
}
