using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Defines the division of a body part for the purpose of allowing a haptic device that
    /// has multiple actuators define what area of the body part it can affect. It can also
    /// be used by an object with a single actuator to define the area that can effect it by
    /// limiting the available area.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [CreateAssetMenu(fileName = "New Body Coordinate System", menuName = "VIRTUOSO/Haptics/Create Body Coordinate System")]
    public class ScriptableBodyCoordinate : ScriptableObject
    {
        #region PublicVariables
        public HumanBodyBones affectableBodyParts;
        public List<BodyCoordinateSpace> affectedAreaList;
        #endregion

        #region PublicAPI
        /// <summary>
        /// Use to tell if a specific body part was hit
        /// </summary>
        /// <param name="hitBone">The body part of the hit</param>
        /// <param name="hitLocation">The exact hit on the body coordinate system</param>
        /// <returns>True if the hit is in an active body coordinate space</returns>
        public bool HitInsideAffectedArea(HumanBodyBones hitBone, BodyCoordinateHit hitLocation)
        {
            // Check if it's on the same bone
            if (hitBone == affectableBodyParts)
            {
                for (int n = 0; n < affectedAreaList.Count; n++)
                {
                    // Check if it's an affect area on the same bone
                    if (affectedAreaList[n].HitInsideSpace(hitLocation))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int BodyCoordinateHitIndex(HumanBodyBones hitBone, BodyCoordinateHit hitLocation)
        {
            // Check if it's on the same bone
            if (hitBone == affectableBodyParts)
            {
                for (int n = 0; n < affectedAreaList.Count; n++)
                {
                    // Check if it's an affect area on the same bone
                    if (affectedAreaList[n].HitInsideSpace(hitLocation))
                    {
                        return n;
                    }
                }
            }

            return -1;
        }
        #endregion

        #region Unity Functions
        void OnValidate()
        {
            for(int n = 0; n < affectedAreaList.Count; n++)
            {
                if(affectedAreaList[n] != null)
                {
                    affectedAreaList[n].OnValidate();
                }
            }
        }
        #endregion
    }
}