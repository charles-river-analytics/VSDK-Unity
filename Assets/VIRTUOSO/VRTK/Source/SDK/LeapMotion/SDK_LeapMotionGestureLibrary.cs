using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
    /// <summary>
    /// Interfaces directly with the Leap Motion SDK to determine current gestures.
    /// 
    /// Written by: Dan Duggan (dduggan@cra.com), 2018
    /// </summary>
    public class SDK_LeapMotionGestureLibrary : SDK_BaseGestureLibrary
    {

        #region Private Variables
#if VRTK_DEFINE_SDK_LEAP_MOTION
        private Leap.Unity.LeapXRServiceProvider leapServiceProvider;
        // Leap LERPs between poses to get a "grab strength" that is total grab at 1 and open hand at 0, this constant defines the tipping point
        private static float GRAB_STRENGTH_THRESHOLD = 0.5f;
        // Leap handles pinch similar to grab, see above
        private static float PINCH_STRENGTH_THRESHOLD = 0.5f;
        private static float MM_TO_M = 0.001f;
        private static Vector3 FLIP_Z_AXIS = new Vector3(1, 1, -1);
#endif
        #endregion

        #region Unity Functions
        public void OnEnable()
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Unity.LeapXRServiceProvider leapXRServiceProvider = FindObjectOfType<Leap.Unity.LeapXRServiceProvider>();
            if(leapXRServiceProvider != null)
            {
                leapServiceProvider = leapXRServiceProvider;
            } else
            {
                Debug.LogError("Leap Motion Gesture Library: no leap service provider found!");
            }
#endif
        }
        #endregion

        #region Gesture Library Functions
        public override bool IsHandDetected(Hand handId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Hand targetHand = GetLeapHand(handId);
            if (targetHand == null)
            {
                return false;
            }
            else
            {
                return true;
            }
#else
            return false;
#endif
        }

        public override Vector3 GetHandPosition(Hand handId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Hand targetHand = GetLeapHand(handId);
            if(targetHand == null)
            {
                return Vector3.zero;
            }
            else
            {
                return LeapToUnityVector(targetHand.PalmPosition);
            }
#else
            return Vector3.zero;
#endif
        }

        public override Vector3 GetHandNormal(Hand handId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Hand targetHand = GetLeapHand(handId);
            if (targetHand == null)
            {
                return Vector3.zero;
            }
            else
            {
                return LeapToUnityNormal(targetHand.PalmNormal);
            }
#else
            return Vector3.zero;
#endif
        }

        public override bool IsFingerBent(Hand handId, Finger fingerId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Hand currentHand = GetLeapHand(handId);
            if(currentHand == null)
            {
                return false;
            }

            Leap.Finger.FingerType leapFingerType = VirtuosoFingerToLeapFinger(fingerId);

            foreach(Leap.Finger finger in currentHand.Fingers)
            {
                if(finger.Type == leapFingerType)
                {
                    return !finger.IsExtended;
                }
            }

            // finger not found; therefor no
            return false;
#else
            throw new System.NotImplementedException();
#endif
        }

        public override bool IsHandClosed(Hand handId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION 
            Leap.Hand currentHand = GetLeapHand(handId);
            if(currentHand == null)
            {
                return false;
            }

            return currentHand.GrabStrength >= GRAB_STRENGTH_THRESHOLD;
#else
            throw new System.NotImplementedException();
#endif
        }

        public override bool IsHandOpen(Hand handId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Hand currentHand = GetLeapHand(handId);
            if (currentHand == null)
            {
                return false;
            }

            return currentHand.GrabStrength < GRAB_STRENGTH_THRESHOLD;
#else
            throw new System.NotImplementedException();
#endif
        }

        public override bool IsHandPinched(Hand handId)
        {
#if VRTK_DEFINE_SDK_LEAP_MOTION
            Leap.Hand currentHand = GetLeapHand(handId);
            if (currentHand == null)
            {
                return false;
            }

            return currentHand.PinchStrength >= PINCH_STRENGTH_THRESHOLD;
#else
            throw new System.NotImplementedException();
#endif
        }

        #endregion

        #region Helpers
#if VRTK_DEFINE_SDK_LEAP_MOTION
        /// <summary>
        /// Converts the Virtuoso enum for fingers to the Leap Motion enum for fingers
        /// </summary>
        protected Leap.Finger.FingerType VirtuosoFingerToLeapFinger(Finger finger)
        {
            switch(finger)
            {
                case Finger.Index:
                    {
                        return Leap.Finger.FingerType.TYPE_INDEX;
                    }
                case Finger.Middle:
                    {
                        return Leap.Finger.FingerType.TYPE_MIDDLE;
                    }
                case Finger.Ring:
                    {
                        return Leap.Finger.FingerType.TYPE_RING;
                    }
                case Finger.Pinky:
                    {
                        return Leap.Finger.FingerType.TYPE_PINKY;
                    }
                case Finger.Thumb:
                    {
                        return Leap.Finger.FingerType.TYPE_THUMB;
                    }
                default:
                    {
                        return Leap.Finger.FingerType.TYPE_UNKNOWN;
                    }
            }
        }

        /// <summary>
        /// Retrieves the hand specified by the Virtuoso Hand enum.
        /// This will return null if the specified hand is not available.
        /// </summary>
        protected Leap.Hand GetLeapHand(Hand handId)
        {
            if (leapServiceProvider == null)
            {
                return null;
            }

            Leap.Frame currentFrame = leapServiceProvider.CurrentFrame;
            Leap.Hand currentHand = null;

            foreach (Leap.Hand hand in currentFrame.Hands)
            {
                if (hand.IsLeft && handId == Hand.Left)
                {
                    currentHand = hand;
                    break;
                }
                else if (hand.IsRight && handId == Hand.Right)
                {
                    currentHand = hand;
                    break;
                }
            }

            return currentHand;
        }

        /// <summary>
        /// Converts by flipping across the Z axis and scaling from mm to m.
        /// </summary>
        protected Vector3 LeapToUnityVector(Leap.Vector leapVector)
        {
            // convert the raw structure
            Vector3 unityVector = Leap.Unity.UnityVectorExtension.ToVector3(leapVector);
            // scale
            unityVector *= MM_TO_M;
            // flip over Z
            unityVector = Vector3.Scale(unityVector, FLIP_Z_AXIS);
            return unityVector;
        }
        /// <summary>
        /// Converts by flipping but not scaling.
        /// </summary>
        protected Vector3 LeapToUnityNormal(Leap.Vector leapVector)
        {
            Vector3 unityVector = Leap.Unity.UnityVectorExtension.ToVector3(leapVector);
            return Vector3.Scale(unityVector, FLIP_Z_AXIS);
        }
#endif
#endregion
    }
}