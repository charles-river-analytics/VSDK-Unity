using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// A utility class to hold any math functions that are needed in VIRTUOSO.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class VirtMath : MonoBehaviour
    {
        /// <summary>
        /// Normalizes the given value to a value between 0 and 1.
        /// </summary>
        /// <param name="value">The value to be normalized</param>
        /// <param name="min">The minimum value that the value can take on</param>
        /// <param name="max">The maximum value that the value can take on</param>
        /// <returns></returns>
        public static float Normalize(float value, float min, float max)
        {
            return (value - min) / (max - min); ;
        }

        /// <summary>
        /// Determine the signed angle between two vectors, with normal 'n'
        /// as the rotation axis. Direction is clockwise for positive angles 
        /// and negative for counter-clockwise.
        /// </summary>
        /// Source: https://forum.unity.com/threads/need-vector3-angle-to-return-a-negtive-or-relative-value.51092/ - Tinus
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Helper function to keep a value between two extremes
        /// </summary>
        /// <param name="value">The number to be wrapped</param>
        /// <param name="minValue">The minimum value in the range</param>
        /// <param name="maxValue">The maximum value in the range</param>
        /// <returns>The number between the min and max value</returns>
        public static float WrapBetweenRange(float value, float minValue, float maxValue)
        {
            return (((value - minValue) % (maxValue - minValue)) + (maxValue - minValue)) % (maxValue - minValue) + minValue;
        }
    }
}