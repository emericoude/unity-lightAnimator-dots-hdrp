using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Emeric.LightAnimator
{
    public static class Extensions
    {
        public static float GetRange(this float2 target)
        {
            return (Mathf.Abs(target.x) + Mathf.Abs(target.y));
        }

        public static float GetRange(this Vector2 target)
        {
            return (Mathf.Abs(target.x) + Mathf.Abs(target.y));
        }
    }
}
