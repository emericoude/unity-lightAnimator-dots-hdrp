using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace Emeric {

    [CustomEditor(typeof(FlickeringLightTriggerChangeAuthoring))]
    [CanEditMultipleObjects]
    public class FlickeringLightTriggerChangeEditor : OdinEditor
    {
        //Empty Odin editor to ensure odin takes care of this script (otherwise it wouldn't work)
    }
}
