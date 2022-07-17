using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Emeric.LightAnimator
{
    /// <summary>
    /// Defines custom class extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Mathf.Abs(target.x - target.y)
        /// </summary>
        /// <remarks>Custom unity extension for the float2 type.</remarks>
        /// <param name="target">The target float2 to calculate the range from.</param>
        /// <returns>The range in between the two values of a float2.</returns>
        public static float GetRange(this float2 target)
        {
            return (Mathf.Abs(target.x - target.y));
        }

        /// <summary>
        /// Mathf.Abs(target.x - target.y)
        /// </summary>
        /// <remarks>Custom unity extension for the Vector2 type.</remarks>
        /// <param name="target">The target Vector2 to calculate the range from.</param>
        /// <returns>The range in between the two values of a Vector2.</returns>
        public static float GetRange(this Vector2 target)
        {
            return (Mathf.Abs(target.x - target.y));
        }
    }
}
