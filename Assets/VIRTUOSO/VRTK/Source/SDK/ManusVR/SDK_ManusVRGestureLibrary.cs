using UnityEngine;
#if VRTK_DEFINE_SDK_MANUS_VR
using Assets.ManusVR.Scripts;
#endif

namespace VRTK
{
    /// <summary>
    /// Interfaces directly with the ManusVR SDK to determine current gestures.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    public class SDK_ManusVRGestureLibrary : SDK_BaseGestureLibrary
    {
        #region PublicVariables
#if VRTK_DEFINE_SDK_MANUS_VR
        public static HandData handData;
        public static TrackingManager handTrackingData;
        public static HandManager handManager;
#endif
        /// <summary>
        /// Since the raw data for the Manus is a single list, the indices for the fingers are sequential by not off by one 
        /// </summary>
        public enum ManusFingers
        {
            Thumb = 9,
            Index = 7,
            Middle = 5,
            Ring = 3,
            Pinky = 1,
            Undefined = -1
        }
        #endregion

        #region ConstantVariables
        private const float FINGER_CLOSE_BEND_THRESHOLD = 0.7f;
        private const float FINGER_OPEN__BEND_THRESHOLD = 0.3f;
        private const float PINCH_DISTANCE_THRESHOLD = .05f;
        private const int DISTAL_PHALANGE_INDEX = 3;
        #endregion

        #region UnityFunctions
        public void Start()
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            handData = GetComponentInParent<HandData>();
            handTrackingData = GetComponentInParent<TrackingManager>();
            handManager = GetComponentInParent<HandManager>();
#endif
        }
        #endregion

        #region GestureLibraryFunctions
        // Returns wrist normal
        public override Vector3 GetHandNormal(Hand handId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            // Traverse the Hand array to find the left/right hand
            for (int n = 0; n < handManager.Hands.Count; n++)
            {
                if(handManager.Hands[n].DeviceType == HandToDeviceType(handId))
                {
                    // The wrist vector's forward (Z axis) vector points outwards towards the back of the hand, multiply by -1 to get
                    // the vector towards the palm
                    return -1 * handManager.Hands[n].WristTransform.forward;
                }
            }
#endif
            return Vector3.zero;
        }

        // Returns wrist position
        public override Vector3 GetHandPosition(Hand handId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            return (handId == Hand.Left) ? handTrackingData.LeftArm.transform.position : handTrackingData.RightArm.transform.position;
#else
            return Vector3.zero;
#endif
        }

        public override bool IsFingerBent(Hand handId, Finger fingerId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            manus_hand_t hand = GetHand(HandToDeviceType(handId));
            ManusFingers fingerIndex = FingerIndiceToManusFinger(fingerId);

            return hand.raw.finger_sensor[(int)fingerIndex] >= FINGER_CLOSE_BEND_THRESHOLD;
#else
            return false;
#endif
        }

        public override bool IsHandClosed(Hand handId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            return handData.HandClosed(HandToDeviceType(handId));
#else
            return false;
#endif
        }

        public override bool IsHandDetected(Hand handId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            return handData.ValidOutput(HandToDeviceType(handId));
#else
            return false;
#endif
        }

        public override bool IsHandOpen(Hand handId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            return handData.HandOpened(HandToDeviceType(handId));
#else
            return false;
#endif
        }

        /// <summary>
        /// For the ManusVR, hand pinched is determined by looking only at the index finger and thumb.
        /// Since the Manus already assumes that the bone structure is following UE4's format, we
        /// don't expect this portion to change unless they update the Manus API.
        /// </summary>
        public override bool IsHandPinched(Hand handId)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            // Traverse the Hand array to find the left/right hand
            for (int n = 0; n < handManager.Hands.Count; n++)
            {
                if (handManager.Hands[n].DeviceType == HandToDeviceType(handId))
                {
                    // Note the farthest finger from the bone is stored at index 3, currently don't know how to query this without hardcoding
                    Transform indexFingerTipTransform = handManager.Hands[n].FingerTransforms[(int)Finger.Index][DISTAL_PHALANGE_INDEX];
                    Transform thumbTransform = handManager.Hands[n].FingerTransforms[(int)Finger.Thumb][DISTAL_PHALANGE_INDEX];

                    float distanceInUnityMetersBetweenFingerTips = Vector3.Distance(indexFingerTipTransform.position, thumbTransform.position);

                    return distanceInUnityMetersBetweenFingerTips <= PINCH_DISTANCE_THRESHOLD;
                }
            }
#endif

            return false;
        }
#endregion

#region HelperFunctions
#if VRTK_DEFINE_SDK_MANUS_VR
        /// <summary>
        /// Converts the Hand enum to the enum that the Manus uses for Left/Right hand
        /// </summary>
        /// <param name="givenHandValue">The Hand enum used in the Hand SDK</param>
        /// <returns>The device_type_t enum that holds an enum the Manus can read</returns>
        public static device_type_t HandToDeviceType(Hand givenHandValue)
        {
            return (givenHandValue == Hand.Left) ? device_type_t.GLOVE_LEFT : device_type_t.GLOVE_RIGHT;
        }

        /// <summary>
        /// Returns the Manus hand object based on the given enum request.
        /// </summary>
        /// <param name="device">The enum for left/right hand</param>
        /// <returns>The Manus hand object</returns>
        public static manus_hand_t GetHand(device_type_t device)
        {
            manus_hand_t tempHand = new manus_hand_t();

            Manus.ManusGetHand(handData.Session, device, out tempHand);

            return tempHand;
        }

        /// <summary>
        /// Translates the finger index into the correct enum for the Manus API raw values.
        /// </summary>
        /// <param name="givenValue"></param>
        /// <returns></returns>
        public static ManusFingers FingerIndiceToManusFinger(Finger givenFingerValue)
        {
            switch (givenFingerValue)
            {
                // THUMB_INDEX            
                case Finger.Thumb:
                    return ManusFingers.Thumb;
                //INDEX_INDEX
                case Finger.Index:
                    return ManusFingers.Index;
                //MIDDLE_INDEX
                case Finger.Middle:
                    return ManusFingers.Middle;
                //RING_INDEX                    
                case Finger.Ring:
                    return ManusFingers.Ring;
                //PINKY_INDEX
                case Finger.Pinky:
                    return ManusFingers.Pinky;
                default:
                    return ManusFingers.Undefined;
            }
        }
#endif
#endregion
    }
}