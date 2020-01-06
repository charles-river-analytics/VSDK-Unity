#if VRTK_DEFINE_SDK_MANUS_VR
using Assets.ManusVR.Scripts;
using System.Collections.Generic;
using UnityEngine;
#endif

namespace VRTK
{
    /// <summary>
    /// The Manus VR SDK script provides a bridge to SDK methods that deal with the Manus VR device. The end
    /// developer can use this script to easily access the Hand Data, which provides all the information that
    /// the Manus VR has about the hands.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    [SDK_Description("Manus VR", SDK_ManusVRDefines.ScriptingDefineSymbol, null, "Standalone")]
    public class SDK_ManusVRHand
#if VRTK_DEFINE_SDK_MANUS_VR
: SDK_BaseHand
#else
: SDK_FallbackHand
#endif
    {
#if VRTK_DEFINE_SDK_MANUS_VR
        protected HandData cachedHandData;

        public override bool IsConnected
        {
            // TODO
            get;
        }

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
            TrackingManager trackedHands = GetHandController().GetComponent<TrackingManager>();

            return trackedHands.LeftArm.gameObject;
        }

        public override GameObject GetRightHand()
        {
            TrackingManager trackedHands = GetHandController().GetComponent<TrackingManager>();

            return trackedHands.RightArm.gameObject;
        }

        public override int GetHandCount()
        {
            int count = 0;
            HandManager handManager = cachedHandControllerObject.GetComponent<HandManager>();

            if (handManager.RootBoneLeft.gameObject.activeInHierarchy)
            {
                count++;
            }
            if (handManager.RootBoneRight.gameObject.activeInHierarchy)
            {
                count++;
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

            HandData currentHandData = VRTK_SharedMethods.FindEvenInactiveComponent<HandData>();

            if (currentHandData != null)
            {
                cachedHandControllerObject = currentHandData.gameObject;
                cachedHandData = currentHandData;

                // For the Manus, the root is the same object that the hand data is stored on
                cachedRootTransform = currentHandData.transform;

                // The gesture library is set to a child object of the root object
                cachedGestureLibrary = currentHandData.GetComponentInChildren<SDK_ManusVRGestureLibrary>();
            }
        }
#endif
    }
}