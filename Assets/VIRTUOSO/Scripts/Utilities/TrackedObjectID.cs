using System.Collections;
using System.Text;
using UnityEngine;
#if VRTK_DEFINE_SDK_STEAMVR
using Valve.VR;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Allows the end user to use a Vive Tracker's hardware ID to assign to the digital objects, instead of
    /// trying to find the index. This will automatically add the TrackedObject script to the same GameObject
    /// this script is attached to and assign the correct ID. 
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// Updated: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    public class TrackedObjectID : MonoBehaviour
    {
#if VRTK_DEFINE_SDK_STEAMVR
        #region PublicVariables
        public string trackerHardwareID;
        public int trackerPopupIndex;
        #endregion

        #region ProtectedVariables
        protected SteamVR_TrackedObject trackedObject;
        #endregion

        #region UnityFunctions
        protected void Start()
        {
            StartCoroutine(AddTrackedObject());
        }
        #endregion

        #region TrackerFunctions
        /// <summary>
        /// Adds a SteamVR TrackedObject script to the game object and sets the index based on the hardware ID
        /// </summary>
        /// <returns></returns>
        IEnumerator AddTrackedObject()
        {
            // Must make sure not to add this before SteamVR has been fully initalized otherwise will get an AssertionFailed error
            yield return new WaitForEndOfFrame();

            AssignIndex();

            // Set the origin to the currently active SDK
            trackedObject.origin = VRTK.VRTK_SDKManager.instance.loadedSetup.transform;
        }

        /// <summary>
        /// Takes the SteamVR TrackedObject and finds the current SteamVR index based on the hardware ID.
        /// </summary>
        public void AssignIndex()
        {
            trackedObject = gameObject.GetComponent<SteamVR_TrackedObject>();

            if (trackedObject == null)
            {
                trackedObject = gameObject.AddComponent<SteamVR_TrackedObject>();
            }

            // Set the index to none to make sure it doesn't get assigned to the wrong index
            trackedObject.index = SteamVR_TrackedObject.EIndex.None;

            // Make sure the SteamVR context is valid, if not, open a new SteamVR instance so you can pull the tracker data
            if (OpenVR.System == null)
            {
                EVRInitError initError = EVRInitError.None;
                OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Background);
            }

            // Traverse through all the connected IDs looking for this Hardware's current ID
            // Note OpenVR 0 is reserved for the HMD, so skip those index since it will never be the right one
            for (int n = 1; n < Constants.MAX_OPENVR_OBJECTS; n++)
            {
                string currentID = GetHardwareIDFromIndex(n);

                if (currentID == trackerHardwareID)
                {
                    trackedObject.index = (SteamVR_TrackedObject.EIndex)n;

                    break;
                }
            }
        }

        public static string GetHardwareIDFromIndex(int index)
        {
            ETrackedPropertyError serialNumberError = new ETrackedPropertyError();
            StringBuilder serialNumberAsString = new StringBuilder();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_SerialNumber_String, serialNumberAsString, OpenVR.k_unMaxPropertyStringSize, ref serialNumberError);

            return serialNumberAsString.ToString().ToUpper();
        }
        #endregion
#endif
    }
}