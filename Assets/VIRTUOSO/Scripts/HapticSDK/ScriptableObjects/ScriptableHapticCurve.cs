using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Defines what a single curve on a Haptic Pattern is. The class consists of three AnimationCurves that
    /// define how the curve will traverse the height and angle of the Body Coordinate System as well as the intensity
    /// value at each of the inflection point. Since these are mathematical curves, there cannot be two points defined 
    /// at the same time, hence the need for a set of curves in order to define more complex haptic patterns.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [CreateAssetMenu(fileName = "New Haptic Curve", menuName = "VIRTUOSO/Haptics/Create Haptic Curve")]
    public class ScriptableHapticCurve : ScriptableObject
    {
        #region PublicVariables
        public AnimationCurve heightCurve = new AnimationCurve();
        public AnimationCurve angleCurve = new AnimationCurve();
        public AnimationCurve intensityCurve = new AnimationCurve();
        // Used by the Haptic Pattern UI, if this is marked true, it will be deleted by discard
        public bool temporaryCurve = false;
        #endregion

        #region PublicAPI
        public float EndTime
        {
            get
            {
                return (heightCurve.keys.Length > 0) ? heightCurve.keys[heightCurve.length - 1].time : 0;
            }
        }

        /// <summary>
        /// Provides the height and angle for the curve at the given time
        /// </summary>
        /// <param name="currentTime">The timing information for the hit</param>
        /// <returns>The BodyCoordinateHit that provides the height and angle of the hit</returns>
        public BodyCoordinateHit GetHitLocationAtTime(float currentTime)
        {
            return new BodyCoordinateHit(Mathf.Clamp01(heightCurve.Evaluate(currentTime)), 
                                         Mathf.Clamp(angleCurve.Evaluate(currentTime), 0.0f, 360.0f));
        }

        /// <summary>
        /// Provides the intensity value for a given time
        /// </summary>
        /// <param name="currentTime">The timing information for the hit</param>
        /// <returns>The intensity value evaluated at the given time, clamped between 0 and 1</returns>
        public float GetIntensityAtTime(float currentTime)
        {
            return Mathf.Clamp01(intensityCurve.Evaluate(currentTime));
        }

        /// <summary>
        /// Adds a keyframe to the Haptic Pattern at the specified time.
        /// </summary>
        /// <param name="time">The time for the keyframe</param>
        /// <param name="location">The hit angle and height</param>
        /// <param name="intensity">The intensity between 0 and 1</param>
        public void AddKey(float time, BodyCoordinateHit location, float intensity)
        {
            heightCurve.AddKey(time, location.hitHeight);
            angleCurve.AddKey(time, location.hitAngle);
            intensityCurve.AddKey(time, intensity);
        }

        /// <summary>
        /// Adjusts the values of a specific keyframe
        /// </summary>
        /// <param name="keyIndex">The index for the keyframe to update</param>
        /// <param name="newHeight">The new height value for the keyframe</param>
        /// <param name="newAngle">The new angle value for the keyframe</param>
        /// <param name="newIntensity">The new intensity of the keyframe</param>
        /// <param name="newTime">The new timing value for the keyframe</param>
        /// <returns>The index of the keyframe that was changed, timing information may change this value from the passed in keyIndex</returns>
        public int UpdateKey(int keyIndex, float newHeight, float newAngle, float newIntensity, float newTime)
        {
            int newIndex = heightCurve.MoveKey(keyIndex, new Keyframe(newTime, newHeight));
            angleCurve.MoveKey(keyIndex, new Keyframe(newTime, newAngle));
            intensityCurve.MoveKey(keyIndex, new Keyframe(newTime, newIntensity));

            return newIndex;
        }

        /// <summary>
        /// Removes a keyframe from the Haptic Pattern.
        /// </summary>
        /// <param name="keyIndex">The index for the keyframe to remove</param>
        public void RemoveKey(int keyIndex)
        {
            heightCurve.RemoveKey(keyIndex);
            angleCurve.RemoveKey(keyIndex);
            intensityCurve.RemoveKey(keyIndex);
        }
        #endregion

        #region PatternMethods
        private void AddNewKeys(ref AnimationCurve curveWithLessKeys, ref AnimationCurve moreKeys)
        {
            int keyDifference = moreKeys.length - curveWithLessKeys.length;

            for (int n = 0; n < keyDifference; n++)
            {
                curveWithLessKeys.AddKey(moreKeys.keys[curveWithLessKeys.length + n].time, 0f);
            }
        }

        private void KeepCurveInRange(ref AnimationCurve curve, float minValue, float maxValue)
        {
            for (int n = 0; n < curve.keys.Length; n++)
            {
                // Keep value within the proper range
                if (curve.keys[n].value < minValue)
                {
                    curve.MoveKey(n, AdjustKeyFrameValue(curve.keys[n], minValue));
                }
                else if (curve.keys[n].value > maxValue)
                {
                    curve.MoveKey(n, AdjustKeyFrameValue(curve.keys[n], maxValue));
                }

                // Keep time value above zero as well
                if (curve.keys[n].time < 0)
                {
                    curve.MoveKey(n, AdjustKeyFrameTime(curve.keys[n], 0));
                }
            }
        }

        private void KeepCurvesInSync()
        {
            for (int n = 0; n < heightCurve.keys.Length; n++)
            {
                if (heightCurve.keys[n].time != angleCurve.keys[n].time)
                {
                    angleCurve.MoveKey(n, AdjustKeyFrameTime(angleCurve.keys[n], heightCurve.keys[n].time));
                }

                if (heightCurve.keys[n].time != intensityCurve.keys[n].time)
                {
                    intensityCurve.MoveKey(n, AdjustKeyFrameTime(intensityCurve.keys[n], heightCurve.keys[n].time));
                }
            }
        }

        private Keyframe AdjustKeyFrameValue(Keyframe key, float value)
        {
            return new Keyframe(key.time, value, key.inTangent, key.outTangent);
        }

        private Keyframe AdjustKeyFrameTime(Keyframe key, float time)
        {
            return new Keyframe(time, key.value, key.inTangent, key.outTangent);
        }
        #endregion

        #region UnityFunctions
        private void OnValidate()
        {
            // Keep curves the same length
            if (angleCurve.length < heightCurve.length)
            {
                AddNewKeys(ref angleCurve, ref heightCurve);
            }
            else if (intensityCurve.length < heightCurve.length)
            {
                AddNewKeys(ref intensityCurve, ref heightCurve);
            }
            else if(angleCurve.length > heightCurve.length)
            {
                AddNewKeys(ref heightCurve, ref angleCurve);
            }
            else if(intensityCurve.length > heightCurve.length)
            {
                AddNewKeys(ref heightCurve, ref intensityCurve);
            }

            // Make sure the curves have the proper ranges
            KeepCurveInRange(ref heightCurve, 0.0f, 1.0f);
            KeepCurveInRange(ref angleCurve, 0.0f, 360.0f);
            KeepCurveInRange(ref intensityCurve, 0.0f, 1.0f);

            // Make sure each infliction point on the curves are aligned among time, angle + intensity matches height's timing
            KeepCurvesInSync();
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());

            return Path.GetFileNameWithoutExtension(assetPath);
#else
            return "Haptic Curve";
#endif

        }
        #endregion
    }
}