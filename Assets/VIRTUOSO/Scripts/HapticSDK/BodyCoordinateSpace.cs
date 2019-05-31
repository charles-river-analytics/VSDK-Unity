using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// The Body Coordinate Space is a section on a body part that can be triggered by some haptic
    /// actuator. It uses the same body coordinate space of a normalized height and angle.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [Serializable]
    public class BodyCoordinateSpace
    {
        #region PublicVariables
        [Range(0, 1), Delayed]
        public float startHeight = 0;
        [Range(0, 1), Delayed]
        public float endHeight = 1;
        [Range(0, 360), Delayed]
        public float startAngle = 0;
        [Range(0, 360), Delayed]
        public float endAngle = 360;
        #endregion

        #region PublicAPI
        public bool HitInsideSpace(BodyCoordinateHit hitLocation)
        {
            if (hitLocation.hitHeight >= startHeight &&
                hitLocation.hitHeight <= endHeight &&
                hitLocation.hitAngle >= startAngle &&
                hitLocation.hitAngle <= endAngle)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Unity Functions
        public void OnValidate()
        {
            // StartHeight always needs to be less, so flip these values if that's not the case
            if (startHeight > endHeight)
            {
                float tempHeight = endHeight;

                endHeight = startHeight;
                startHeight = tempHeight;
            }

            // Similiarly, StartAngle also needs to be less than the end angle
            if (startAngle > endAngle)
            {
                float tempAngle = endAngle;

                endAngle = startAngle;
                startAngle = tempAngle;
            }
        }
        #endregion
    }
}