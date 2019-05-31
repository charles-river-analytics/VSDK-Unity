using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// Interfaces directly with the SenseGlove SDK to determine current gestures.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class SDK_SenseGloveGestureLibrary : SDK_BaseGestureLibrary
    {
        #region PublicVariables
        public float bentAngle = -75.0f;
        public float pinchDistance = 0.05f;
        #endregion

        #region PrivateVariables
#if VRTK_DEFINE_SDK_SENSE_GLOVE
        private SenseGlove_VirtualHand leftHand;
        private SenseGlove_VirtualHand rightHand;
#endif
        #endregion

        #region SDK_BaseGestureLibraryImplementation
        public override Vector3 GetHandNormal(Hand handId)
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            SenseGlove_VirtualHand currentHand = handId == Hand.Left ? leftHand : rightHand;

            if (currentHand != null)
            {
                return currentHand.wristTransfrom.up * -1;
            }
#endif

            return Vector3.zero;
        }

        public override Vector3 GetHandPosition(Hand handId)
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            SenseGlove_VirtualHand currentHand = handId == Hand.Left ? leftHand : rightHand;

            if (currentHand != null)
            {
                return currentHand.wristTransfrom.position;
            }
#endif

            return Vector3.zero;
        }

        public override bool IsFingerBent(Hand handId, Finger fingerId)
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            SenseGlove_VirtualHand currentHand = handId == Hand.Left ? leftHand : rightHand;

            if (currentHand != null || currentHand.senseGlove == null)
            {
                return false;
            }

            float currentFingerAngle = currentHand.senseGlove.GloveData.TotalGloveAngles()[(int)fingerId].z;

            return currentFingerAngle < bentAngle;
#else
            return false;
#endif
        }

        public override bool IsHandClosed(Hand handId)
        {
            return IsFingerBent(handId, Finger.Thumb) &&
                   IsFingerBent(handId, Finger.Index) &&
                   IsFingerBent(handId, Finger.Middle) &&
                   IsFingerBent(handId, Finger.Ring) &&
                   IsFingerBent(handId, Finger.Pinky);
        }

        public override bool IsHandDetected(Hand handId)
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            SenseGlove_VirtualHand currentHand = handId == Hand.Left ? leftHand : rightHand;

            if (currentHand != null)
            {
                return currentHand.senseGlove.IsConnected;
            }
#endif

            return false;
        }

        public override bool IsHandOpen(Hand handId)
        {
            return !IsFingerBent(handId, Finger.Thumb) &&
                   !IsFingerBent(handId, Finger.Index) &&
                   !IsFingerBent(handId, Finger.Middle) &&
                   !IsFingerBent(handId, Finger.Ring) &&
                   !IsFingerBent(handId, Finger.Pinky);
        }

        /// <summary>
        /// Returns true if the index and thumb are touching in the classic pinch gesture.
        /// </summary>
        /// <remark>The glove must be properly calibrated in order for this to work correctly.</remark>
        /// <param name="handId">The hand to check</param>
        /// <returns>True if the thumb/index are close enough to each other</returns>
        public override bool IsHandPinched(Hand handId)
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            SenseGlove_VirtualHand currentHand = handId == Hand.Left ? leftHand : rightHand;

            if (currentHand != null || currentHand.senseGlove == null)
            {
                return false;
            }

            Vector3[][] handPositions = currentHand.senseGlove.GloveData.handPositions;

            // 0 for thumb finger index, divide by 1000 to get the position from mm to m
            Vector3 thumbFingertip = handPositions[0][handPositions[0].Length - 1] / 1000.0f;

            // 1 for index finger index, divide by 1000 to get the position from mm to m
            Vector3 indexFingerTip = handPositions[1][handPositions[1].Length - 1] / 1000.0f;

            return Vector3.Distance(thumbFingertip, indexFingerTip) < pinchDistance;
#else
            return false;
#endif
        }
        #endregion

        #region Unity Functions
#if VRTK_DEFINE_SDK_SENSE_GLOVE
        void Start()
        {

            leftHand = VRTK_SDKManager.instance.loadedSetup.handSDK.GetLeftHand().GetComponent<SenseGlove_VirtualHand>();
            rightHand = VRTK_SDKManager.instance.loadedSetup.handSDK.GetRightHand().GetComponent<SenseGlove_VirtualHand>();
        }
#endif
        #endregion
    }
}