using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Gestures
{
    /// <summary>
    /// An advanced gesture combines raw gestures with contextual information in a state machine to allow for multistep gestures
    /// that also include more complex elements like basic movement, palm angle, etc.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) October 2018
    /// Last updated: Nicolas Herrera (nherrera@cra.com), December 2019
    /// </summary>
    [CreateAssetMenu(menuName = "VIRTUOSO/Advanced Gesture")]
    public class AdvancedGesture : Gesture
    {
        #region Public Variables
        public List<AdvancedGestureState> advancedGestureStateList = new List<AdvancedGestureState>();
        [NonSerialized]
        protected int currentGestureStateIndex;
        [NonSerialized]
        public Vector3 prevLeftPalmPosition;
        [NonSerialized]
        public Vector3 prevRightPalmPosition;
        [NonSerialized]
        protected VRTK_InteractGrab cachedLeftHandGrabber = null;
        [NonSerialized]
        protected VRTK_InteractGrab cachedRightHandGrabber = null;
        [System.NonSerialized]
        public float gestureStartTime = 0;
        public bool allowedWhileHoldingObjects = false;
        // Adds a delay after a gesture is triggered to prevent it from turning on/off too quickly
        public float gestureCooldown;
        #endregion

        #region Control Variables
        [NonSerialized]
        protected float gestureCooldownStartTime = 0;
        protected float startupDelayTime = 1.0f;
        protected bool gestureHasStarted = false;
        protected bool cooldownHasStarted = false;
        #endregion

        #region Gesture Detection Code
        public override bool IsGestureOccuring(SDK_BaseGestureLibrary.Hand specificHand)
        {
            // if not detected, it cannot be gesturing
            if (VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary() != null && !VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary().IsHandDetected(specificHand))
            {
                return false;
            }

            // OpenMG has a habit of claiming gestures are occuring at the start of the scene before hands are even detected. 
            // this delay ensures that no gesture fire until the system is fully started
            if (Time.time < startupDelayTime)
            {
                return false;
            }

            // If this is not the specified hand, then the gesture cannot be occuring
            if (advancedGestureStateList[currentGestureStateIndex].coreGesture.handSpecific &&
               advancedGestureStateList[currentGestureStateIndex].coreGesture.specificHand != specificHand)
            {
                return false;
            }

            if (!allowedWhileHoldingObjects)
            {
                if (IsHoldingObject(SDK_BaseGestureLibrary.Hand.Left) || IsHoldingObject(SDK_BaseGestureLibrary.Hand.Right))
                {
                    return false;
                }
            }

            bool isCooldownActive = Time.time <= (gestureCooldownStartTime + gestureCooldown);

            // Condition for cooldown has not been met
            if (!isCooldownActive)
            {
                cooldownHasStarted = false;
            }

            // Check core gesture first
            bool isCurrentStateOccuring = advancedGestureStateList[currentGestureStateIndex].coreGesture.IsGestureOccuring(specificHand);

            // If the core gesture is occuring, then start checking the rest of the avanced gesture states
            if (isCurrentStateOccuring)
            {
                if (!gestureHasStarted)
                {
                    gestureHasStarted = true;
                    gestureStartTime = Time.time;
                }

                // Check every Advanced Gesture Condition for the current Gesture State
                foreach (AdvancedGestureCondition advCondition in advancedGestureStateList[currentGestureStateIndex].AdvancedGestureConditionList)
                {
                    isCurrentStateOccuring = isCurrentStateOccuring && advCondition.IsConditionOccuring(specificHand, this);
                }

                // Core gesture and all advanced Gesture States have been satisifed
                if (isCurrentStateOccuring && advancedGestureStateList.Count == currentGestureStateIndex + 1)
                {
                    // Gesture hasn't triggered the cool down yet, after this frame, that will change
                    if (!isCooldownActive && !cooldownHasStarted)
                    {
                        gestureCooldownStartTime = Time.time;
                        cooldownHasStarted = true;

                        return true;
                    }
                    // Gesture is in cooldown, but the user hasn't released the first occurrence of the gesture
                    else if (gestureStartTime <= gestureCooldownStartTime)
                    {
                        return true;
                    }
                    // Gesture is in cooldown, so the gesture cannot occur
                    else
                    {
                        return false;
                    }
                }
                // Core gesture has occured, but not every Gesture State has occured yet
                else if (isCurrentStateOccuring)
                {
                    currentGestureStateIndex++;
                    prevLeftPalmPosition = GetLeftHandPosition();
                    prevRightPalmPosition = GetRightHandPosition();
                }
                else
                {
                    if (advancedGestureStateList[currentGestureStateIndex].holdTime != 0 && advancedGestureStateList[currentGestureStateIndex].holdTime + gestureStartTime <= Time.time)
                    {
                        ResetGestureChain();
                    }

                    return false;
                }
            }
            // Core gesture is not occuring
            else
            {
                gestureHasStarted = false;

                return false;
            }

            // The advanced gesture is not occuring
            return false;
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Returns true if the specified hand is holding an object and false otherwise.
        /// </summary>
        public bool IsHoldingObject(SDK_BaseGestureLibrary.Hand hand)
        {
            Transform handRoot = VRTK_SDK_Bridge.GetHandSDK().GetRootTransform();
            if (handRoot == null)
            {
                // if the hand is not visible, the user isn't holding an object
                return false;
            }

            if (hand == SDK_BaseGestureLibrary.Hand.Left && IsInteractGrabValid(cachedLeftHandGrabber))
            {
                // if not null, an object is being held
                return cachedLeftHandGrabber.GetGrabbedObject() != null;
            }

            if (hand == SDK_BaseGestureLibrary.Hand.Right && IsInteractGrabValid(cachedRightHandGrabber))
            {
                // if not null, an object is being held
                return cachedRightHandGrabber.GetGrabbedObject() != null;
            }

            // either caches aren't set up, or hands are no longer being tracked
            VRTK_InteractGrab[] handGrabbers = handRoot.GetComponentsInChildren<VRTK_InteractGrab>();
            foreach (VRTK_InteractGrab grabber in handGrabbers)
            {
                if (!IsInteractGrabValid(grabber))
                {
                    continue;
                }
                if (grabber.controllerEvents is GestureControllerEvent)
                {
                    GestureControllerEvent gestureController = (GestureControllerEvent)grabber.controllerEvents;
                    if (gestureController.controllerHandId == SDK_BaseGestureLibrary.Hand.Left)
                    {
                        cachedLeftHandGrabber = grabber;
                    }
                    else if (gestureController.controllerHandId == SDK_BaseGestureLibrary.Hand.Right)
                    {
                        cachedRightHandGrabber = grabber;
                    }

                    if (gestureController.controllerHandId == hand)
                    {
                        return grabber.GetGrabbedObject() != null;
                    }
                }
            }
            // fell through because hand was not found
            return false;
        }

        /// <summary>
        /// Returns the position of the left hand, or Vector3.zero if unavailable
        /// </summary>
        public Vector3 GetLeftHandPosition()
        {
            SDK_BaseGestureLibrary currentLibrary = VRTK.VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary();
            if (currentLibrary == null)
            {
                Debug.LogWarning("Unable to find gesture library for current Hand SDK");
                return Vector3.zero;
            }
            else
            {
                return currentLibrary.GetHandPosition(SDK_BaseGestureLibrary.Hand.Left);
            }
        }

        /// <summary>
        /// Returns the position vector for the right hand or Vector3.zero if it is unavailable
        /// </summary>
        public Vector3 GetRightHandPosition()
        {
            SDK_BaseGestureLibrary currentLibrary = VRTK.VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary();
            if (currentLibrary == null)
            {
                Debug.LogWarning("Unable to find gesture library for current Hand SDK");
                return Vector3.zero;
            }
            else
            {
                return currentLibrary.GetHandPosition(SDK_BaseGestureLibrary.Hand.Right);
            }
        }

        /// <summary>
        /// Returns the euler rotation in degrees for the left hand or Vector3.zero if it is unavailable
        /// </summary>
        public Vector3 GetLeftHandNormal()
        {
            SDK_BaseGestureLibrary currentLibrary = VRTK.VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary();
            if (currentLibrary == null)
            {
                Debug.LogWarning("Unable to find gesture library for current Hand SDK");
                return Vector3.zero;
            }
            else
            {
                return currentLibrary.GetHandNormal(SDK_BaseGestureLibrary.Hand.Left);
            }
        }

        /// <summary>
        /// Returns the euler rotation in degrees for the right hand or Vector3.zero if it is unavailable
        /// </summary>
        public Vector3 GetRightHandNormal()
        {
            SDK_BaseGestureLibrary currentLibrary = VRTK.VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary();
            if (currentLibrary == null)
            {
                Debug.LogWarning("Unable to find gesture library for current Hand SDK");
                return Vector3.zero;
            }
            else
            {
                return currentLibrary.GetHandNormal(SDK_BaseGestureLibrary.Hand.Right);
            }
        }

        /// <summary>
        /// Sets the state machine for the advanced gesture back to start
        /// </summary>
        protected void ResetGestureChain()
        {
            currentGestureStateIndex = 0;
            gestureStartTime = 0;
            prevLeftPalmPosition = GetLeftHandPosition();
            prevRightPalmPosition = GetRightHandPosition();
        }

        protected bool IsInteractGrabValid(VRTK_InteractGrab interactGrab)
        {
            if (interactGrab == null)
            {
                interactGrab = null;
                return false;
            }

            if (interactGrab.gameObject.activeInHierarchy == false)
            {
                interactGrab = null;
                return false;
            }

            if (interactGrab.isActiveAndEnabled == false)
            {
                return false;
            }

            return true;
        }
        #endregion
    }

    #region Embedded Classes
    /// <summary>
    /// The advanced gesture state represents a step in an advanced gesture. It holds a number of gesture conditions that
    /// are each check to determine if the state is occuring as well as a single coreGesture that must be active for the
    /// state to be active.
    /// </summary>
    [Serializable]
    public class AdvancedGestureState
    {
        #region Class Enum
        public enum AdvancedConditionType
        {
            Movement,
            PalmAngle,
            SimultaneousAction,
            Hold
        }
        #endregion

        #region Public Variables and Properties
        // name is for readability but serves no other function
        public string gestureName = "GestureName";
        public Gesture coreGesture;
        [Tooltip("How long the advanced gesture will hold at this state before dropping back to the initial state.")]
        public float holdTime;
        // this property conglomerates the separate conditions. They are in separate lists for easier management
        public List<AdvancedGestureCondition> AdvancedGestureConditionList
        {
            get
            {
                List<AdvancedGestureCondition> conditions = new List<AdvancedGestureCondition>();
                foreach (MovementGestureCondition condition in movementConditionList)
                {
                    conditions.Add(condition);
                }
                foreach (HoldGestureCondition condition in holdConditionList)
                {
                    conditions.Add(condition);
                }
                foreach (PalmNormalCondition condition in palmConditions)
                {
                    conditions.Add(condition);
                }
                foreach (SimultaneousGesture condition in simultaneousGestureConditionList)
                {
                    conditions.Add(condition);
                }
                return conditions;
            }
        }

        public List<MovementGestureCondition> movementConditionList = new List<MovementGestureCondition>();
        public List<HoldGestureCondition> holdConditionList = new List<HoldGestureCondition>();
        public List<PalmNormalCondition> palmConditions = new List<PalmNormalCondition>();
        public List<SimultaneousGesture> simultaneousGestureConditionList = new List<SimultaneousGesture>();

        // used to decide the class for new conditions created in the editor
        public AdvancedConditionType editorNewConditionType;
        #endregion
    }

    /// <summary>
    /// The base class for gesture conditions is non-abstract because Unity serialization breaks with abstract class hierarchies
    /// </summary>
    [Serializable]
    public class AdvancedGestureCondition
    {
        /// <summary>
        /// Returns true if the condition is occuring and false otherwise, except in the base version which will always return false.
        /// This method is not marked as abstract because Unity cannot serialize classes that have abstract base classes. New Advanced Gesture Conditions
        /// should implement this method.
        /// </summary>
        public virtual bool IsConditionOccuring(SDK_BaseGestureLibrary.Hand specificHand, AdvancedGesture advancedGestureInfo)
        {
            return false;
        }
    }

    /// <summary>
    /// The movement gesture condition determines if the hand making the gesture has moved since the previous state.
    /// It has several options but it is noteable that none of those options allow for specific directions of travel.
    /// </summary>
    [Serializable]
    public class MovementGestureCondition : AdvancedGestureCondition
    {
        #region Embedded Enum
        // determines how the movement check is made
        public enum MovementOperator { GreaterThan, LessThan, EqualTo }
        #endregion

        #region Public Variables
        public MovementOperator distanceOperator;
        public float distanceFromPreviousGesture;
        public float distanceTolerance = 0.001f;
        #endregion

        #region AdvancedGestureCondition Override
        public override bool IsConditionOccuring(SDK_BaseGestureLibrary.Hand specificHand, AdvancedGesture advancedGestureInfo)
        {
            Vector3 palmPosition;
            Vector3 previousPalmPosition;

            if (specificHand == SDK_BaseGestureLibrary.Hand.Left)
            {
                previousPalmPosition = advancedGestureInfo.prevLeftPalmPosition;
                palmPosition = advancedGestureInfo.GetLeftHandPosition();
            }
            else
            {
                previousPalmPosition = advancedGestureInfo.prevRightPalmPosition;
                palmPosition = advancedGestureInfo.GetRightHandPosition();
            }

            if (palmPosition == Vector3.zero)
            {
                // vector3 zero is the 'null' for vectors
                return false;
            }

            float palmMovementDistance = Vector3.Distance(palmPosition, previousPalmPosition);
            // switch statement effectively controls which operator is used when comparing the actual palm movement amount to the desired palm movement amount
            switch (distanceOperator)
            {
                case MovementOperator.LessThan:
                    {
                        if (palmMovementDistance < distanceFromPreviousGesture)
                        {
                            return true;
                        }
                        break;
                    }
                case MovementOperator.GreaterThan:
                    {
                        if (palmMovementDistance > distanceFromPreviousGesture)
                        {
                            return true;
                        }
                        break;
                    }
                case MovementOperator.EqualTo:
                    {
                        if ((distanceFromPreviousGesture - distanceTolerance <= palmMovementDistance && palmMovementDistance <= distanceFromPreviousGesture + distanceTolerance))
                        {
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }
        #endregion
    }

    /// <summary>
    /// The palm vector condition passes/fails based on where the player's palm is facing relative to other objects
    /// </summary>
    [Serializable]
    public class PalmNormalCondition : AdvancedGestureCondition
    {
        #region Embedded Enums
        // no left/right vector at the moment, because the meaning of each varies on each hand (e.g. left on left is outward, left on right is inward)
        public enum VectorType { Up, Forward }
        public enum OtherVector { World, Hmd }
        #endregion

        #region Public Variables
        public OtherVector otherVectorToUse;
        public VectorType otherVectorDirection;
        public Vector3 eulerRotationFromOtherVector;
        public float tolerance = 0.15f;
        #endregion

        #region Advanced Gesture Condition Override
        public override bool IsConditionOccuring(SDK_BaseGestureLibrary.Hand specificHand, AdvancedGesture advancedGestureInfo)
        {
            SDK_BaseGestureLibrary currentLibrary = VRTK.VRTK_SDK_Bridge.GetHandSDK().GetGestureLibrary();
            if (currentLibrary == null)
            {
                Debug.LogWarning("No gesture library detected for the current Hand SDK");
                return false;
            }
            Vector3 palmNormal = currentLibrary.GetHandNormal(specificHand);
            if (palmNormal == Vector3.zero)
            {
                // hand tracking lost
                return false;
            }
            // otherVec is a normal vector that will be compared to the palm vector
            Vector3 otherVec;
            Quaternion rotationToUse = Quaternion.Euler(eulerRotationFromOtherVector);
            if (otherVectorToUse == OtherVector.World)
            {
                switch (otherVectorDirection)
                {
                    case VectorType.Forward:
                        {
                            otherVec = rotationToUse * Vector3.forward;
                            break;
                        }
                    case VectorType.Up:
                        {
                            otherVec = rotationToUse * Vector3.up;
                            break;
                        }
                    default:
                        {
                            otherVec = rotationToUse * Vector3.up;
                            break;
                        }
                }
            }
            else if (otherVectorToUse == OtherVector.Hmd)
            {
                Transform hmd = VRTK_SDK_Bridge.GetHeadset();
                switch (otherVectorDirection)
                {
                    case VectorType.Forward:
                        {
                            otherVec = hmd.TransformDirection(rotationToUse * Vector3.forward);
                            break;
                        }
                    case VectorType.Up:
                        {
                            otherVec = hmd.TransformDirection(rotationToUse * Vector3.up);
                            break;
                        }
                    default:
                        {
                            otherVec = hmd.TransformDirection(rotationToUse * Vector3.up);
                            break;
                        }
                }
            }
            else
            {
                // for whatever reason, otherVec is undefined. return false
                return false;
            }

            float dotProduct = Vector3.Dot(otherVec, palmNormal);
            // close to 1 -> similar direction
            return (dotProduct >= 1 - tolerance);
        }
        #endregion
    }

    /// <summary>
    /// The hold gesture condition passes if the core gesture is held for a specific amount of time
    /// </summary>
    [Serializable]
    public class HoldGestureCondition : AdvancedGestureCondition
    {
        #region Public Variables
        public float gestureHoldTime;
        #endregion

        #region Advanced Gesture Condition Override
        public override bool IsConditionOccuring(SDK_BaseGestureLibrary.Hand specificHand, AdvancedGesture advancedGestureInfo)
        {
            return Time.time >= advancedGestureInfo.gestureStartTime + gestureHoldTime;
        }
        #endregion
    }

    /// <summary>
    /// Checks if a specific gesture is being held by the other hand. Checking if another advanced gesutre is occuring is NOT recommended
    /// because it could lead to recursion.
    /// </summary>
    [Serializable]
    public class SimultaneousGesture : AdvancedGestureCondition
    {
        #region Public Variables
        public Gesture simultaneousGesture;
        #endregion

        #region Advanced Gesture Condition Override
        public override bool IsConditionOccuring(SDK_BaseGestureLibrary.Hand specificHand, AdvancedGesture advancedGestureInfo)
        {
            SDK_BaseGestureLibrary.Hand otherHand = specificHand == SDK_BaseGestureLibrary.Hand.Left ? SDK_BaseGestureLibrary.Hand.Right : SDK_BaseGestureLibrary.Hand.Left;
            return simultaneousGesture.IsGestureOccuring(otherHand);
        }
        #endregion
    }
    #endregion
}
