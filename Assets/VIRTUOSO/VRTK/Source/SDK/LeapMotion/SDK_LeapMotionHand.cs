#if VRTK_DEFINE_SDK_LEAP_MOTION
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
#endif
namespace VRTK
{
    /// <summary>
    /// The Leap Motion SDK script provides a bridge to SDK methods that deal with the Leap Motion device. The end
    /// developer can use this script to easily access the Leap Motion service provider, which provides all the 
    /// information about the hands.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    [SDK_Description("Leap Motion", SDK_LeapMotionDefines.ScriptingDefineSymbol , null, "Standalone")]
    public class SDK_LeapMotionHand
#if VRTK_DEFINE_SDK_LEAP_MOTION
        : SDK_BaseHand
#else
        : SDK_FallbackHand
#endif
    {
#if VRTK_DEFINE_SDK_LEAP_MOTION
        protected LeapServiceProvider cachedLeapServiceProvider;

        public override void SetHandCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedHandControllerObject = null;
            }

            LeapServiceProvider serviceProvider = VRTK_SharedMethods.FindEvenInactiveComponent<LeapServiceProvider>();

            if(serviceProvider != null)
            {
                cachedHandControllerObject = serviceProvider.gameObject;
                cachedLeapServiceProvider = serviceProvider;

                // The root for the leap is one of the service provider's grandparents since that is how the Leap Motion prefab is set up
                cachedRootTransform = VRTK_SharedMethods.FindEvenInactiveComponent<XRHeightOffset>().transform;

                // The gesture library is a 'great uncle' in terms of hierarchy from the service provider so use the root to find the library
                cachedGestureLibrary = cachedRootTransform.GetComponentInChildren<SDK_LeapMotionGestureLibrary>();
            }
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
            return GetHand(Chirality.Left);
        }

        public override GameObject GetRightHand()
        {
            return GetHand(Chirality.Right);
        }

        public override GameObject GetNeededParent()
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;

            if (sdkManager != null)
            {
                return sdkManager.loadedSetup.actualHeadset;
            }

            return null;
        }

        public override int GetHandCount()
        {
            return cachedLeapServiceProvider.CurrentFixedFrame.Hands.Count;
        }

        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
            // Noop - Implement for base class
        }

        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            // Noop - Implement for base class
        }

        public override Transform GetRootTransform()
        {
            if(cachedRootTransform == null)
            {
                SetHandCaches();
            }

            return cachedRootTransform;
        }

        private GameObject GetHand(Chirality handedness)
        {
            HandModelBase[] handList = GetHandController().GetComponentsInChildren<HandModelBase>();

            foreach (HandModelBase currentHand in handList)
            {
                if (currentHand.Handedness == handedness && currentHand.isActiveAndEnabled)
                {
                    return currentHand.gameObject;
                }
            }

            return null;
        }
#endif
    }

}