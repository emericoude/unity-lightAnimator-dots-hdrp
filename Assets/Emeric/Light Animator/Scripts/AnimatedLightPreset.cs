using UnityEngine;

namespace Emeric.LightAnimator
{
    [CreateAssetMenu(fileName = "New Light Animation Preset", menuName = "Scriptable Objects/Light Animator/Preset")]
    public class AnimatedLightPreset : ScriptableObject
    {
        public AnimatedLight Settings { get { return _settings; } private set { _settings = value; } }
        [SerializeField] private AnimatedLight _settings;
    }
}
