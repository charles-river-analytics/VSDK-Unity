using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

namespace CharlesRiverAnalytics.Virtuoso.Gestures
{
    /// <summary>
    /// A class container for defining a gesture. This class follows the booleans that are provided
    /// by the SDK_BaseGestureLibrary for gesture recognition. A gesture here is defined as a combination
    /// of bools that must be true (though all others do not necessarily have to be false).
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// Last Modified: Dan Duggan (dduggan@cra.com) October 2018: Added code related to handedness of gestures
    /// </summary>
    [CreateAssetMenu(fileName = "New Gesture", menuName = "VIRTUOSO/Create Gesture")]
    public class Gesture : ScriptableObject
    {
        #region Public Variables
        [Tooltip("This gesture is occuring if every item in this list is true")]
        public List<GestureCondition> gestureConditionList;
        [Tooltip("If true, the gesture can only be performed with the hand specified in specificHand")]
        public bool handSpecific = false;
        [Tooltip("When handSpecific is true, this field specifies which hand the gesture must be performed with")]
        public SDK_BaseGestureLibrary.Hand specificHand;

        public enum BasicGesture
        {
            IsThumbBent,
            IsIndexFingerBent,
            IsMiddleFingerBent,
            IsRingFingerBent,
            IsPinkyFingerBent,
            IsHandOpen,
            IsHandClosed,
            IsHandPinching
        }
        #endregion

        #region Public API
        public virtual bool IsGestureOccuring(SDK_BaseGestureLibrary.Hand handToCheck)
        {
            // if not detected, it cannot be gesturing
            if (!VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary().IsHandDetected(handToCheck))
            {
                return false;
            }

            bool isGestureOccuring = true;

            if (handSpecific && specificHand != handToCheck)
            {
                return false;
            }

            for (int gestureConditionIndex = 0; gestureConditionIndex < gestureConditionList.Count; gestureConditionIndex++)
            {
                GestureCondition condition = gestureConditionList[gestureConditionIndex];
                bool thisConditionIsMet = true;
                thisConditionIsMet = GetBoolValue(condition.featureToCheck, handToCheck) == condition.featureValue;
                isGestureOccuring = isGestureOccuring && thisConditionIsMet;
                // Quick return, if at any time one is false, then it would not execute
                if (!isGestureOccuring)
                {
                    return isGestureOccuring;
                }
            }

            return isGestureOccuring;
        }

        public virtual bool GetBoolValue(BasicGesture givenGesture, SDK_BaseGestureLibrary.Hand handIndex)
        {
            SDK_BaseGestureLibrary currentLibrary = VRTK.VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary();
            if (currentLibrary == null)
            {
                // preventing a null exception, but this really should use some error handling
                // since currently it doesn't differentiate between a gesture not happening and a hand not existing
                // does not print a warning because it would flood the log
                return false;
            }

            switch (givenGesture)
            {
                case BasicGesture.IsThumbBent:
                    return currentLibrary.IsFingerBent(handIndex, SDK_BaseGestureLibrary.Finger.Thumb);
                case BasicGesture.IsIndexFingerBent:
                    return currentLibrary.IsFingerBent(handIndex, SDK_BaseGestureLibrary.Finger.Index);
                case BasicGesture.IsMiddleFingerBent:
                    return currentLibrary.IsFingerBent(handIndex, SDK_BaseGestureLibrary.Finger.Middle);
                case BasicGesture.IsRingFingerBent:
                    return currentLibrary.IsFingerBent(handIndex, SDK_BaseGestureLibrary.Finger.Ring);
                case BasicGesture.IsPinkyFingerBent:
                    return currentLibrary.IsFingerBent(handIndex, SDK_BaseGestureLibrary.Finger.Pinky);
                case BasicGesture.IsHandOpen:
                    return currentLibrary.IsHandOpen(handIndex);
                case BasicGesture.IsHandClosed:
                    return currentLibrary.IsHandClosed(handIndex);
                case BasicGesture.IsHandPinching:
                    return currentLibrary.IsHandPinched(handIndex);
                default:
                    return false;
            }
        }
        #endregion
    }

    /// <summary>
    /// Encapsulates data that determines whether or not the gesture is active
    /// </summary>
    [System.Serializable]
    public class GestureCondition
    {
        public Gesture.BasicGesture featureToCheck;
        [Tooltip("This is the value that the gesturebool should be")]
        public bool featureValue = true;
    }
}