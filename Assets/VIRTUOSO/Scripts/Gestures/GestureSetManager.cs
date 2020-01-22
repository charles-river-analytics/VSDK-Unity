using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Gestures
{
    /// <summary>
    /// The Manager class for GestureSets. This class is responsible for dis/enabling the different GestureSets
    /// by name.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2019
    /// </summary>
    public class GestureSetManager : MonoBehaviour
    {
        #region PrivateVariables
        private Dictionary<string, GestureSet> knownGestureSets;
        private const float SDK_SETUP_WAIT_TIME = 3.0f;
        #endregion

        #region PublicAPI
        public void EnableGestureSet(string gestureSetName)
        {
            if (VRTK_SDKManager.instance.loadedSetup.handSDK.IsConnected && knownGestureSets.ContainsKey(gestureSetName))
            {
                knownGestureSets[gestureSetName].ChangeGestureSetState(true);
            }
        }

        public void DisableGestureSet(string gestureSetName)
        {
            if (knownGestureSets.ContainsKey(gestureSetName))
            {
                knownGestureSets[gestureSetName].ChangeGestureSetState(false);
            }
        }
        #endregion

        #region PrivateFunctions
        private IEnumerator EnableDefaultGestureSet(List<GestureSet> defaultGestureSets)
        {
            // Wait for the SDK to set up
            while (!VRTK_SDKManager.instance.loadedSetup.handSDK.IsConnected)
            {
                yield return null;

                // Enable the default gesture sets
                foreach (GestureSet gestureSet in defaultGestureSets)
                {
                    gestureSet.ChangeGestureSetState(true);
                }
            }
        }
        #endregion

        #region UnityFunctions
        private void Start()
        {
            knownGestureSets = new Dictionary<string, GestureSet>();

            GestureSet[] allGestureSetsInScene = GetComponentsInChildren<GestureSet>();
            List<GestureSet> defaultGestureSets = new List<GestureSet>();

            foreach (GestureSet gestureSet in allGestureSetsInScene)
            {
                knownGestureSets.Add(gestureSet.setName, gestureSet);

                if (gestureSet.startOnAwake)
                {
                    defaultGestureSets.Add(gestureSet);
                }

                // Disable all sets at the start
                DisableGestureSet(gestureSet.setName);
            }

            EnableDefaultGestureSet(defaultGestureSets);
        }
        #endregion
    }
}