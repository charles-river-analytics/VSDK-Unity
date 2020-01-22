using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace VRTK.SecondaryControllerGrabActions
{
    /// <summary>
    /// Allows for an item to have multiple secondary grab attach behaviours.
    /// Behaviors are picked based on the secondary grabber's location. This is useful for creating interactable objects with multiple functions.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) Updated: December 2019
    /// </summary>
    public class MultiSecondaryGrabAction : VRTK_BaseGrabAction
    {
        #region
        [Tooltip("Pairs of mechanics and interaction regions")]
        public LocalizedSecondaryGrabAction[] localizedSecondaryGrabMechanics;
        [Tooltip("The mechanic to use if the grab point was outside any of the regions")]
        public VRTK_BaseGrabAction defaultGrabAction;
        #endregion

        #region Control Variables
        protected VRTK_BaseGrabAction currentAction = null;
        #endregion

        #region Base Class Overrides
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            currentAction = defaultGrabAction;
            foreach(LocalizedSecondaryGrabAction localizedMechanic in localizedSecondaryGrabMechanics)
            {
                if(localizedMechanic.regionCollider.bounds.Contains(secondaryGrabPoint.position))
                {
                    currentAction = localizedMechanic.secondaryGrabMechanic;
                    break;
                }
            }
            currentAction.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            base.Initialise(currentGrabbdObject, primaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
        }

        public override void ResetAction()
        {
            if(currentAction != null)
            {
                currentAction.ResetAction();
                currentAction = null;
            }
        }

        public override void ProcessFixedUpdate()
        {
            if(currentAction != null)
            {
                currentAction.ProcessFixedUpdate();
            }
        }

        public override void ProcessUpdate()
        {
            if (currentAction != null)
            {
                currentAction.ProcessUpdate();
            }
        }

        public override void OnDropAction()
        {
            if(currentAction != null)
            {
                currentAction.OnDropAction();
                currentAction = null;
            }
        }
        #endregion
    }

    /// <summary>
    /// Container for pairing a grab attach script with a collider.
    /// </summary>
    [Serializable]
    public class LocalizedSecondaryGrabAction
    {
        [Tooltip("The collider the player grabs to activate this mechanic")]
        public Collider regionCollider;
        [Tooltip("The action to active for this region")]
        public VRTK_BaseGrabAction secondaryGrabMechanic;
    }
}
