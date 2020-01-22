using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.SecondaryControllerGrabActions
{
    /// <summary>
    /// Takes control of an animator temporarily while the player is grabbing the interactable object with a second hand.
    /// As a result, this controller could be used to fake physics-based interactions through animations, e.g. sliding the slide on a handgun.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) Updated: December 2019
    /// </summary>
    public class AnimatorControlGrabAction : VRTK_BaseGrabAction
    {
        #region Public Variables
        [Tooltip("The animator the action will take control of")]
        public Animator animator;
        [Tooltip("The name of the animation the action will scrub")]
        public string animationName;
        [Tooltip("The animation layer the animation is on")]
        public int animationLayer = 0;
        [Tooltip("The name of the trigger used to tell the animator to leave the overriden animation state")]
        public string exitTriggerName;
        [Tooltip("How far the controller can go before the normalized distance is counted as 1")]
        public float maximumDistance;
        [Tooltip("The distance at which the secondary grab attach is broken")]
        public float breakDistance;
        [Tooltip("This setting overrides the grab to hold option on the interactable object. Only works if the other object is NOT set to hold to grab")]
        public bool holdToSecondaryGrab = false;
        #endregion

        #region Control Vars
        // stores the starting location of the grab, so we can calculate the distance
        protected Vector3 initialGrabLocation;
        // this is used to prevent the animation from exiting early
        public static float maxAnimationProgress = 0.999f;
        #endregion

        #region Base Class Overrides
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            animator.ResetTrigger(exitTriggerName);
            animator.Play(animationName, animationLayer);
            animator.speed = 0;

            initialGrabLocation = currentSecondaryGrabbingObject.transform.position;
            

            currentSecondaryGrabbingObject.controllerEvents.GripReleased += ProcessRelease;

            base.Initialise(currentGrabbdObject, primaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
        }

        public override void ResetAction()
        {
            base.ResetAction();
            animator.SetTrigger(exitTriggerName);
            animator.speed = 1;
        }

        protected void ProcessRelease(object sender, ControllerInteractionEventArgs e)
        {
            if(holdToSecondaryGrab == true && grabbedObject != null)
            {
                grabbedObject.ForceStopSecondaryGrabInteraction();
            }
        }

        #endregion

        #region Unity Methods
        void FixedUpdate()
        {
            if (initialised)
            {
                Vector3 currentGrabberPosition = secondaryGrabbingObject.transform.position;
                float distance = Vector3.Distance(currentGrabberPosition, initialGrabLocation);
                if (distance > breakDistance)
                {
                    grabbedObject.ForceStopSecondaryGrabInteraction();
                }
                distance = Mathf.Clamp(distance, 0, maximumDistance);
                // set to 0.999 maximum so that the animation isn't left early
                float normalizedTime = Mathf.Clamp(distance / maximumDistance, 0, maxAnimationProgress);
                animator.Play(animationName, animationLayer, normalizedTime);
            }
        }
        #endregion
    }
}
