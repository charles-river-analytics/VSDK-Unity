using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// This class allows the user to create a tracked object without requiring the user to copy and paste the object across the VRTK prefab trees.
    /// It works by assigning an ID (starting from 0). This will pair the object with the tracked object with that index for the current setup.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) August 2018
    /// Updated: Nicolas Herrera (nherrera@cra.com), October 2018
    /// </summary>
    public class TrackedObjectFollower : MonoBehaviour
    {
        #region Public Variables
        public int sdkSelectionIndex = 0;
        public int trackerSelectionIndex = 0;
        #endregion

        #region Private Variables
        private Transform trackedObject;
        private bool initializedProperly = false;
        #endregion

        #region Unity Functions
        void OnEnabled()
        {
        	// The Start() code is slightly delayed to allow SteamVR/VRTK to finish initialization
            StartCoroutine(StartFollowingTracker());
        }

        void Start()
        {
            StartCoroutine(StartFollowingTracker());
        }
        
        void Update()
        {
            if(initializedProperly)
            {
                transform.SetPositionAndRotation(trackedObject.position, trackedObject.rotation);
            }
        }

        /// <summary>
        /// This function waits for VRTK to finish setup and then mirrors the tracker's position and rotation.
        /// </summary>
        /// <returns></returns>
        IEnumerator StartFollowingTracker()
        {
            VRTK.VRTK_SDKSetup currentSetup = VRTK.VRTK_SDKManager.instance.loadedSetup;

            while (currentSetup == null)
            {
                currentSetup = VRTK.VRTK_SDKManager.instance.loadedSetup;

                yield return null;
            }

            int maxTrackerId = currentSetup.actualTrackers.Count;
            if (trackerSelectionIndex >= maxTrackerId)
            {
                Debug.LogError("Unable to pair TrackedObjectFollower with object: tracker index is greater than max tracker index for current setup!");
                yield break;
            }

            trackedObject = currentSetup.actualTrackers[trackerSelectionIndex].transform;
            if (trackedObject != null)
            {
                initializedProperly = true;
            }
        }
        #endregion
    }
}
