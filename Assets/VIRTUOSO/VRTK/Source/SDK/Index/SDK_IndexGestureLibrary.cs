using System;
using UnityEngine;
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
using Valve.VR;
#endif

namespace VRTK
{
    /// <summary>
    /// Interfaces directly with the SteamVR SDK to determine current gestures on the Index Controllers.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class SDK_IndexGestureLibrary : SDK_BaseGestureLibrary
    {
        #region PublicVariables
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
        // Note: These are defined in the default SteamVR Action when compiled and should be part of any set if planning on using VIRTUOSO Gestures
        public SteamVR_Action_Skeleton leftSkeletonAction = SteamVR_Input.GetSkeletonAction("SkeletonLeftHand");
        public SteamVR_Action_Skeleton rightSkeletonAction = SteamVR_Input.GetSkeletonAction("SkeletonRightHand");
        public SteamVR_Action_Boolean pinchAction = SteamVR_Input.GetBooleanAction("GrabPinch");
#endif
        #endregion

        #region ConstantVariables
        private const float FINGER_CLOSE_BEND_THRESHOLD = 0.9f;
        // In using the Index controllers, the thumbCurl value does not actually go up to 1 but .75
        private const float THUMB_CLOSE_BEND_THRESHOLD = 0.7f;
        private const float FINGER_OPEN_THRESHOLD = 0.1f;
        #endregion

        #region GestureLibraryFunctions
        /// <summary>
        /// Returns the direction of the user's palm as they are holding the controller
        /// </summary>
        public override Vector3 GetHandNormal(Hand handId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            // Use the controller SDK since the Index is also a controller in SteamVR
            if(handId == Hand.Left)
            {
                return VRTK_SDKManager.instance.loadedSetup.actualLeftController.transform.right;
            }
            else
            {
                return -1 * VRTK_SDKManager.instance.loadedSetup.actualRightController.transform.right;
            }
#else
            return Vector3.zero;
#endif
        }

        // Returns wrist position
        public override Vector3 GetHandPosition(Hand handId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            // Use the controller SDK since the Index is also a controller in SteamVR
            GameObject controllerObject = handId == Hand.Left ? VRTK_SDKManager.instance.loadedSetup.actualLeftController :
                                                                VRTK_SDKManager.instance.loadedSetup.actualRightController;

            return controllerObject.transform.position;
#else
            return Vector3.zero;
#endif
        }

        public override bool IsFingerBent(Hand handId, Finger fingerId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            float curlValue;
            SteamVR_Action_Skeleton actionToUse = handId == Hand.Left ? leftSkeletonAction : rightSkeletonAction;

            switch (fingerId)
            {
                case Finger.Thumb:
                    return actionToUse.thumbCurl >= THUMB_CLOSE_BEND_THRESHOLD;
                case Finger.Index:
                    curlValue = actionToUse.indexCurl;
                    break;
                case Finger.Middle:
                    curlValue = actionToUse.middleCurl;
                    break;
                case Finger.Ring:
                    curlValue = actionToUse.ringCurl;
                    break;
                case Finger.Pinky:
                    curlValue = actionToUse.pinkyCurl;
                    break;
                default:
                    curlValue = 0;
                    break;
            }

            return curlValue >= FINGER_CLOSE_BEND_THRESHOLD;
#else
            return false;
#endif
        }

        public override bool IsHandClosed(Hand handId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            SteamVR_Action_Skeleton actionToUse = handId == Hand.Left ? leftSkeletonAction : rightSkeletonAction;
            bool isClose = true;

            foreach(Finger finger in (Finger[])Enum.GetValues(typeof(Finger)))
            {
                isClose = isClose && IsFingerBent(handId, finger);
            }

            return isClose;
#else
            return false;
#endif
        }

        public override bool IsHandDetected(Hand handId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            SteamVR_Action_Skeleton actionToUse = handId == Hand.Left ? leftSkeletonAction : rightSkeletonAction;

            return actionToUse.poseIsValid;
#else
            return false;
#endif
        }

        public override bool IsHandOpen(Hand handId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            SteamVR_Action_Skeleton actionToUse = handId == Hand.Left ? leftSkeletonAction : rightSkeletonAction;
            bool isOpen = true;

            for (int n = 0; n < actionToUse.fingerCurls.Length; n++)
            {
                isOpen = isOpen && (actionToUse.fingerCurls[n] < FINGER_OPEN_THRESHOLD);
            }
            
            return isOpen;
#else
            return false;
#endif
        }

        public override bool IsHandPinched(Hand handId)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
            return pinchAction.state;
#else
            return false;
#endif
        }
        #endregion
    }
}