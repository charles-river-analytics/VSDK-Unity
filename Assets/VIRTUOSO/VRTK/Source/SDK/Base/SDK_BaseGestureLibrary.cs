using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
    /// <summary>
    /// Defines the common functions all gesture library implementations must share.
    /// 
    /// Written by: Dan Duggan (dduggan@cra.com), 2018
    /// </summary>
    public abstract class SDK_BaseGestureLibrary : MonoBehaviour
    {
        #region Enums
        public enum Finger { Thumb, Index, Middle, Ring, Pinky }

        public enum Hand { Left, Right }
        #endregion

        #region Basic Gesture Functions
        /// <summary>
        /// Returns true if the hand is currently visible to the system.
        /// </summary>
        public abstract bool IsHandDetected(Hand handId);

        /// <summary>
        /// Returns true if the finger on the given hand is bent
        /// </summary>
        public abstract bool IsFingerBent(Hand handId, Finger fingerId);

        /// <summary>
        /// Returns true if the hand is in an open position
        /// </summary>
        public abstract bool IsHandOpen(Hand handId);

        /// <summary>
        /// Returns true if the hand is in a closed position (not necessarily
        /// the opposite of IsHandOpen since intermediate states are possible)
        /// </summary>
        public abstract bool IsHandClosed(Hand handId);

        /// <summary>
        /// Returns true if the index and thumb are touching in the classic
        /// pinch gesture.
        /// </summary>
        public abstract bool IsHandPinched(Hand handId);
        #endregion

        #region Gesture Helper Functions

        /// <summary>
        /// Retrieves the hand position for the given hand. It is recommended that implementations use the position of the palm, if available.
        /// </summary>
        public abstract Vector3 GetHandPosition(Hand handId);

        /// <summary>
        /// Retrieves the normal vector of the hand. It is recommended that the hand normal be the normal vector of the palm, if available.
        /// </summary>
        public abstract Vector3 GetHandNormal(Hand handId);
        #endregion
    }
}
