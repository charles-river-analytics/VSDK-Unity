using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Holds offset rotation and position data for the Grab Attachments.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class GrabOffset : MonoBehaviour
    {
        #region PublicVariables
        [HideInInspector]
        public string handOrControllerSDKName;
        public Vector3 rotationOffset;
        public Vector3 positionOffset;
        [HideInInspector]
        public Transform offsetTransform;
        #endregion
    }
}