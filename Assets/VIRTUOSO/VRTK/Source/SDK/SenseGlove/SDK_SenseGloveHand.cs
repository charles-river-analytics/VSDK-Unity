using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// The SenseGlove VR SDK script provides a bridge to SDK methods that deal with the SenseGlove device. The end
    /// developer can use this script to easily access the SenseGlove_Object to deal with the sense glove data.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), May 2019
    /// </summary>
    [SDK_Description("SenseGlove", SDK_SenseGloveDefines.ScriptingDefineSymbol, null, "Standalone")]
    public class SDK_SenseGloveHand
#if VRTK_DEFINE_SDK_SENSE_GLOVE
: SDK_BaseHand
#else
: SDK_FallbackHand
#endif
    {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
        public override GameObject GetHandController()
        {
            GameObject hand = GetSDKManagerHandController();

            if (hand == null)
            {
                SetHandCaches();

                return cachedHandControllerObject;
            }

            return hand;
        }

        public override GameObject GetLeftHand()
        {
            return GetHand(ConnectionMethod.NextLeftHand);
        }

        public override GameObject GetRightHand()
        {
            return GetHand(ConnectionMethod.NextRightHand);
        }

        private GameObject GetHand(ConnectionMethod connectionMethod)
        {
            for (int n = 0; n < cachedHandControllerObject.transform.childCount; n++)
            {
                SenseGlove_Object currentGlove = cachedHandControllerObject.transform.GetChild(n).GetComponent<SenseGlove_Object>();

                if (currentGlove != null)
                {
                    if (currentGlove.connectionMethod == connectionMethod)
                    {
                        return currentGlove.gameObject;
                    }
                }
            }

            return null;
        }

        public override int GetHandCount()
        {
            if (cachedRootTransform == null)
            {
                SetHandCaches();
            }

            int count = 0;

            for(int n = 0; n < cachedHandControllerObject.transform.childCount; n++)
            {
                SenseGlove_Object currentGlove = cachedHandControllerObject.transform.GetChild(n).GetComponent<SenseGlove_Object>();

                if (currentGlove != null)
                {
                    if(currentGlove.IsConnected)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public override Transform GetRootTransform()
        {
            if (cachedRootTransform == null)
            {
                SetHandCaches();
            }

            return cachedRootTransform;
        }

        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
            // Noop -  Implement for base class
        }

        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            // Noop -  Implement for base class
        }

        public override void SetHandCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedHandControllerObject = null;
            }

            SenseGlove_DeviceManager currentDeviceManager = VRTK_SharedMethods.FindEvenInactiveComponent<SenseGlove_DeviceManager>();

            if (currentDeviceManager != null)
            {
                cachedHandControllerObject = currentDeviceManager.gameObject;

                // For the SenseGlove, the root is the same object that the device manager is stored on
                cachedRootTransform = currentDeviceManager.transform;

                // The gesture library is set to a child object of the root object
                cachedGestureLibrary = currentDeviceManager.GetComponent<SDK_SenseGloveGestureLibrary>();
            }
        }
#endif
    }
}