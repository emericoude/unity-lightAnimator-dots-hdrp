using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Emeric.LightAnimator
{
    /// <summary>
    /// Removes light animation presets from the AnimatedLightPresets.Instance.Presets when they are deleted.
    /// </summary>
    /// <remarks>This cannot be done through OnDestroy() or OnDisable() for scriptable objects, hence this script.</remarks>
    public class AnimatedLightPresetDeleter : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(AnimatedLightPreset))
            {
                var deletionTarget = (AnimatedLightPreset)AssetDatabase.LoadMainAssetAtPath(path);

                if (AnimatedLightPresets.Instance != null)
                {
                    if (AnimatedLightPresets.Instance.Presets.Contains(deletionTarget))
                    {
                        Debug.Log($"Successfully removed {deletionTarget.name} from the list of presets.");
                        AnimatedLightPresets.Instance.Presets.Remove(deletionTarget);
                    }
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}

