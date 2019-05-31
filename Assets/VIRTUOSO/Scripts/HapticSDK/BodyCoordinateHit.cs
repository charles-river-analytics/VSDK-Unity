using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Structure that holds the normalized height and the angle of a hit.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public struct BodyCoordinateHit
    {
        #region PublicVariables
        [Range(0, 1)]
        public float hitHeight;
        [Range(0, 360)]
        public float hitAngle;
        #endregion

        #region Constructors
        public BodyCoordinateHit(float height, float angle)
        {
            hitHeight = height;
            hitAngle = angle;
        }

        public BodyCoordinateHit(BodyCoordinateHit otherHit)
        {
            hitHeight = otherHit.hitHeight;
            hitAngle = otherHit.hitAngle;
        }
        #endregion

        #region DefaultOverrides
        public override string ToString()
        {
            return "(" + hitHeight + ", " + hitAngle + ")";
        }
        #endregion
    }
}