using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// InteractionArea that responds to the turning of an interactable object. The IA is able to
    /// see two types of rotation: parallel and perpendicular. When set to parallel, the object is
    /// facing the same direction (using forward direction) as the IA, e.g. a screwdriver and a screw.
    /// With perpendicular, the object is at a 90 to the IA's forward direction, e.g. a ratchet and nut.
    /// 
    /// Some assumptions:
    /// For the objects doing the turning, assumes the handle portion goes with the Z-axis.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class TorqueInteractionArea : InteractionArea
    {
        #region PublicVariables
        [Tooltip("The number of complete rotations to tighten/untighten the object.")]
        public float completeRotationsToChangeState = 3.0f;
        [Tooltip("Allows the user to make less turns by amplifying the amount they turn.")]
        public float rotationMultipler = 1.0f;
        public TurnType turnRotationType = TurnType.Perpendicular;
        public ScrewDirection screwDirection = ScrewDirection.Clockwise;

        /// <summary>
        /// How the user must turn the controller/hand to affect the object
        /// </summary>
        public enum TurnType
        {
            /// <summary>
            /// The controller's roll is used to affect the object
            /// </summary>
            Parallel,
            /// <summary>
            /// The controller's angular velocity is used to affect the object
            /// </summary>
            Perpendicular
        }

        /// <summary>
        /// The direction that the object is going in.
        /// </summary>
        public enum ScrewDirection
        {
            /// <summary>
            /// Event is sent when the IA turns all the way in.
            /// </summary>
            Clockwise,
            /// <summary>
            /// Event is sent when the IA turns all the way out.
            /// </summary>
            CounterClockwise
        }
        #endregion

        #region PrivateVariables
        private float rotationAmountLeft;
        private float totalRotationNeeded;
        private Vector3 previousFrameForwardDirection;
        private Vector3 previousFrameRightDirection;
        private bool isTurnable = false;
        private const float ROTATION_EPSILON = 0.2f;
        #endregion

        #region TorqueMethods
        private void CalculateTurn()
        {
            float currentRotation = 0;

            switch (turnRotationType)
            {
                case TurnType.Parallel:
                    // Get the roll rotation of the interactable object
                    currentRotation = Vector3.SignedAngle(previousFrameRightDirection, currentInteractionObject.transform.right, previousFrameForwardDirection);
                    break;
                case TurnType.Perpendicular:
                    // Get the angle around the torque area 
                    currentRotation = Vector3.SignedAngle(previousFrameForwardDirection, currentInteractionObject.transform.forward, gameObject.transform.forward);
                    break;
                default:
                    break;
            }

            if (Mathf.Abs(currentRotation) > ROTATION_EPSILON)
            {
                rotationAmountLeft += (currentRotation * rotationMultipler);

                OnObjectUsedInteractionArea(LinearMotionReactionArgs.SetInteractionAreaEvent(currentInteractionObject.gameObject,
                                                                                                   1.0f - (rotationAmountLeft / totalRotationNeeded)
                                                                                                   ));
            }

            if (rotationAmountLeft <= 0 && screwDirection == ScrewDirection.Clockwise)
            {
                rotationAmountLeft = 0;

                OnObjectFinishedInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));

                ChangeScrewDirection();
            }
            else if(rotationAmountLeft >= totalRotationNeeded && screwDirection == ScrewDirection.CounterClockwise)
            {
                rotationAmountLeft = totalRotationNeeded;

                OnObjectFinishedInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));

                ChangeScrewDirection();
            }

            previousFrameForwardDirection = currentInteractionObject.transform.forward;
            previousFrameRightDirection = currentInteractionObject.transform.right;
        }

        private void ChangeScrewDirection()
        {
            screwDirection = (screwDirection == ScrewDirection.Clockwise) ? ScrewDirection.CounterClockwise : ScrewDirection.Clockwise;

            CalculateRotationAmount();
        }

        private void CalculateRotationAmount()
        {
            rotationAmountLeft = (screwDirection == ScrewDirection.Clockwise) ? totalRotationNeeded : 0.0f;
        }
        #endregion

        #region InteractionAreaOverrides
        public override void OnObjectEnteredInteractionArea(InteractionAreaEventArgs e)
        {
            base.OnObjectEnteredInteractionArea(e);

            if (currentInteractionObject != null)
            {
                // Get the starting directions to go off of
                previousFrameForwardDirection = currentInteractionObject.transform.forward;
                previousFrameRightDirection = currentInteractionObject.transform.right;

                isTurnable = true;
            }
        }

        public override void OnObjectExitedInteractionArea(InteractionAreaEventArgs e)
        {
            base.OnObjectExitedInteractionArea(e);

            isTurnable = false;
        }
        #endregion

        #region Unity Functions
        protected void OnEnable()
        {
            totalRotationNeeded = completeRotationsToChangeState * 360.0f;

            CalculateRotationAmount();
        }

        protected override void Update()
        {
            base.Update();

            if(isTurnable && currentInteractionObject != null)
            {
                CalculateTurn();
            }
        }
        #endregion
    }
}