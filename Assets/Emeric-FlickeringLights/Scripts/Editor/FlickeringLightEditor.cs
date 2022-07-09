using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;

namespace Emeric {

    
    [CustomEditor(typeof(FlickeringLightAuthoring))]
    [CanEditMultipleObjects]
    [Serializable]
    public class FlickeringLightEditor : OdinEditor
    {      
        
    }
}

