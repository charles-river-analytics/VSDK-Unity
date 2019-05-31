using CharlesRiverAnalytics.Virtuoso.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Defines a haptic pattern to use with the Haptic SDK. A haptic pattern is composed of a set of haptic curves. Some
    /// additional information includes playback timing, which determines how often to tell the device to make a haptic pulse.
    /// Collision resolution deals with how two or more curves will resolve the intensity if they affect the same actuator.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [CreateAssetMenu(fileName = "New Haptic Pattern", menuName = "VIRTUOSO/Haptics/Create Haptic Pattern")]
    public class ScriptableHapticPattern : ScriptableObject
    {
        #region PublicVariables
        // The curves that make up the haptic pattern
        public List<ScriptableHapticCurve> curveList = new List<ScriptableHapticCurve>();
        // How to resolve the intensity when 2 or more curves intersect in a section of a body coordinate space
        public PatternCollisionResolution collisionResolution = PatternCollisionResolution.Average;
        // The curve index to use if curve priority is selected as the resolution
        public int curvePriorityIndex = 0;
        // How to time playing successive haptic pulses in the pattern
        public PlaybackTiming playbackTiming;
        // If using a custom playback timing, this is the value between successive haptic pulses
        public float customPlaybackTiming = 0.333f;
        // What happens when a pattern's evaluated height goes above the range
        public PatternOvershootResolution heightOvershootResolution = PatternOvershootResolution.Clamp;
        // What happens when a pattern's evaluated height goes above the range
        public PatternOvershootResolution angleOvershootResolution = PatternOvershootResolution.Wrap;
        // The last body hit with this pattern
        public BodyCoordinateHit hitOffset;
        // Whether to use the hitoffset when evaluating the haptic pattern being played
        public OffsetUse offsetUse = OffsetUse.Ignore;
        #endregion

        #region PublicProperties
        public float EndTime
        {
            get
            {
                return Mathf.Max(curveList.Select(C => (C.EndTime)).ToArray());
            }
        }
        #endregion

        #region PrivateVariables
        private float[] intensityEvaluationBuffer;
        private BodyCoordinateHit[] hitLocationBuffer;
        #endregion

        #region PublicAPI
        /// <summary>
        /// Provides the intensity values for all the curve values in the pattern for a specified time
        /// </summary>
        /// <param name="currentTime">The timing information</param>
        /// <returns>An array of intensity value for each curve in the pattern</returns>
        public float[] GetIntenstiyValuesAtCurrentTime(float currentTime)
        {
            // Get the intensity value for each curve
            for (int n = 0; n < curveList.Count; n++)
            {
                intensityEvaluationBuffer[n] = curveList[n].GetIntensityAtTime(currentTime);
            }

            return intensityEvaluationBuffer;
        }

        /// <summary>
        /// Provides the hit location (angle and height) values for all the curve values in the pattern for a specified time
        /// </summary>
        /// <param name="currentTime">The timing information</param>
        /// <returns>An array of BodyCoordinateHit value for each curve in the pattern</returns>
        public BodyCoordinateHit[] GetHitLocationsAtCurrentTime(float currentTime)
        {
            // Get the hit location for each curve
            for (int n = 0; n < curveList.Count; n++)
            {
                BodyCoordinateHit hitLocation = curveList[n].GetHitLocationAtTime(currentTime);

                // Adjust the hitLocation based on the offset
                switch (offsetUse)
                {
                    case OffsetUse.SetAtFirstPoint:
                        // Get the difference between the offset and the first point
                        float heightDifference = hitOffset.hitHeight - curveList[n].heightCurve[0].value;
                        float angleDifference = hitOffset.hitAngle - curveList[n].angleCurve[0].value;

                        // Apply the difference to the current hit location
                        hitLocation.hitHeight += heightDifference;
                        hitLocation.hitAngle += angleDifference;

                        break;
                }

                // Adjust the hitLocation based on the overshoot resolution for the height
                hitLocation.hitHeight = AdjustHitLocation(hitLocation.hitHeight, 0.0f, 1.0f, heightOvershootResolution);

                // Adjust the hitLocation based on the overshoot resolution for the angle
                hitLocation.hitAngle = AdjustHitLocation(hitLocation.hitAngle, 0.0f, 360.0f, angleOvershootResolution);

                hitLocationBuffer[n] = hitLocation;
            }

            return hitLocationBuffer;
        }

        /// <summary>
        /// Adds a keyframe to the pattern for a given curve
        /// </summary>
        /// <param name="curveIndex">The curve index to add the keyframe to</param>
        /// <param name="time">The timing for the new keyframe</param>
        /// <param name="location">The height/angle of the keyframe</param>
        /// <param name="intensity">The intensity value of the keyframe</param>
        public void AddKey(int curveIndex, float time, BodyCoordinateHit location, float intensity)
        {
            curveList[curveIndex].AddKey(time, location, intensity);
        }

        /// <summary>
        /// Removes a keyframe from a specified curve
        /// </summary>
        /// <param name="curveIndex">The index of the curve to remove a keyframe from</param>
        /// <param name="keyIndex">The index of the keyframe within the curve to remove</param>
        public void RemoveKey(int curveIndex, int keyIndex)
        {
            curveList[curveIndex].RemoveKey(keyIndex);
        }

        /// <summary>
        /// Provides the number of keyframes in a specific curve
        /// </summary>
        /// <param name="curveIndex">The index of the curve</param>
        /// <returns>-1 if an invalid curve index is given, otherwise the total number of keyframes in the curve</returns>
        public int GetKeyCountForCurve(int curveIndex)
        {
            if (curveIndex < 0 || curveIndex >= curveList.Count)
            {
                return -1;
            }
            else
            {
                return curveList[curveIndex].heightCurve.length;
            }
        }

        /// <summary>
        /// Adds an empty curve to the pattern through scripting. This will also save the curve to an .asset file so that the reference and changes
        /// made to this curve are not lost to serilization issues.
        /// </summary>
        /// <returns>The index of the new curve</returns>
        public int AddNewCurve(string filePath, string patternName)
        {
            curveList.Add(ScriptableObject.CreateInstance<ScriptableHapticCurve>());

#if UNITY_EDITOR
            // Save the curve so that it can be found in the UI list
            ScriptableObjectUtility.SaveScriptableObject(curveList[curveList.Count - 1], filePath, patternName + "_Curve" + (curveList.Count - 1));
#endif

            return curveList.Count - 1;
        }

        /// <summary>
        /// Adds a curve to the curveList. This method is used when the curve already has an .asset file associated
        /// with it and does not require tweaking it in any way.
        /// </summary>
        /// <param name="hapticCurve">The curve to add to the list</param>
        /// <returns>The index of the added curve</returns>
        public int AddSavedCurve(ScriptableHapticCurve hapticCurve)
        {
            curveList.Add(hapticCurve);

            return curveList.Count - 1;
        }

        /// <summary>
        /// Removes a specific curve from the pattern. If the curve is temporary, it will delete the
        /// associated .asset file as well.
        /// </summary>
        /// <param name="curveIndex">The index of the curve to remove in the curveList</param>
        public void RemoveCurve(int curveIndex)
        {
            if (curveIndex < 0 || curveIndex >= curveList.Count)
            {
                return;
            }

#if UNITY_EDITOR
            // If it is a temporary curve, delete the asset first since it is no longer needed
            if (curveList[curveIndex].temporaryCurve)
            {
                ScriptableObjectUtility.DeleteScriptableObject(curveList[curveIndex]);
            }
#endif

            curveList.RemoveAt(curveIndex);
        }

        /// <summary>
        /// Utility to get the length of a curve in the pattern for the specified curve
        /// </summary>
        /// <param name="curveIndex">The index of the curve in the curveList</param>
        /// <returns>-1 if the index is invalid, otherwise the last keyframe timing information for the curve</returns>
        public float GetCurveEndTime(int curveIndex)
        {
            if (curveIndex < 0 || curveIndex >= curveList.Count)
            {
                return -1;
            }

            return curveList[curveIndex].heightCurve[curveList[curveIndex].heightCurve.length - 1].time;
        }
        #endregion

        #region PrivateMethods
        private float AdjustHitLocation(float value, float minValue, float maxValue, PatternOvershootResolution overshootResolution)
        {
            switch (overshootResolution)
            {
                case PatternOvershootResolution.Discard:
                    if (value < minValue || value > maxValue)
                    {
                        // Return negative infinity as this number will never be reached and can easily be checked to skip
                        return float.NegativeInfinity;
                    }
                    break;
                case PatternOvershootResolution.Clamp:
                    if (value > maxValue)
                    {
                        return maxValue;
                    }
                    else if (value < minValue)
                    {
                        return minValue;
                    }
                    break;
                case PatternOvershootResolution.Wrap:
                    if (value < minValue || value > maxValue)
                    {
                        return VirtMath.WrapBetweenRange(value, minValue, maxValue);
                    }
                    break;
            }

            return value;
        }
        #endregion

        #region Unity Functions
        private void OnEnable()
        {
            intensityEvaluationBuffer = new float[curveList.Count];
            hitLocationBuffer = new BodyCoordinateHit[curveList.Count];
        }

        private void OnDisable()
        {
            // Don't keep a saved offset value since it's only set by a collision
            hitOffset = new BodyCoordinateHit();
        }

        private void OnValidate()
        {
            if (curvePriorityIndex < 0)
            {
                curvePriorityIndex = 0;
            }
            else if (curvePriorityIndex >= curveList.Count)
            {
                curvePriorityIndex = curveList.Count - 1;
            }
        }
        #endregion
    }
}