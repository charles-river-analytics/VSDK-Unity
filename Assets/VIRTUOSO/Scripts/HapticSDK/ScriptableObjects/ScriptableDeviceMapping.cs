using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Takes a BodyCoordinate body part and creates an index mapping between the spaces availble from the mapping
    /// and the actuators on the device. The IndexMapping class assumes that the actuators can be referenced from
    /// an index value represented by an int.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [CreateAssetMenu(fileName = "New Device Mapping", menuName = "VIRTUOSO/Haptics/Create Device Mapping")]
    public class ScriptableDeviceMapping : ScriptableObject
    {
        #region PublicVariables
        public ScriptableBodyCoordinate affectedBodyArea;
        public IndexMapping[] mapping;
        #endregion

        #region Unity Functions
        void OnValidate()
        {
            if(affectedBodyArea != null)
            {
                Array.Resize(ref mapping, affectedBodyArea.affectedAreaList.Count);
            }
            else
            {
                mapping = new IndexMapping[0];
            }
        }
        #endregion

        [Serializable]
        public class IndexMapping
        {
            public int[] indexMapping;
        }
    }   
}