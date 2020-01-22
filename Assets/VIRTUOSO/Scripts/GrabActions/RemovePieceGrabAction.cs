using System;
using UnityEngine;
using UnityEngine.Animations;
using VRTK;
using VRTK.SecondaryControllerGrabActions;

namespace CharlesRiverAnalytics.Virtuoso.GrabAction
{
    /// <summary>
    /// Secondary Grab Action for Interactable Objects. This script should be used if you have an Interactable Object that
    /// must be removed from another Interactable Object (e.g. remove a lid from a jar). To undo this effect, use the
    /// ReturnPieceToParentReaction with an Interaction Area.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class RemovePieceGrabAction : VRTK_BaseGrabAction
    {
        #region PublicVariables
        public VRTK_InteractableObject secondaryInteractableToRemove;
        public GameObject returnToParentInteractionAreaObject;
        #endregion

        #region Events
        public event EventHandler<EventArgs> ObjectRemovedFromParent;

        public virtual void OnObjectRemovedFromParent(EventArgs e)
        {
            if (ObjectRemovedFromParent != null)
            {
                ObjectRemovedFromParent(this, e);
            }
        }
        #endregion

        #region BaseGrabActionOverride
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);

            ParentConstraint parentConstraint = secondaryInteractableToRemove.GetComponent<ParentConstraint>();

            // Make sure that the assigned Interactable Object is still a child of this object
            bool isStillChild = secondaryInteractableToRemove.transform.IsChildOf(transform) || (parentConstraint != null && parentConstraint.sourceCount > 0 && parentConstraint.GetSource(0).sourceTransform == transform);

            if (isStillChild)
            {
                // First, remove from the parent from the secondary object so that it is not affected by it's movement
                if (parentConstraint != null)
                {
                    // Note: We assume that the objects will only have 1 parent here
                    parentConstraint.RemoveSource(0);

                    parentConstraint.constraintActive = false;
                }
                else
                {
                    secondaryInteractableToRemove.transform.parent = null;
                }

                // Set the Grabbable field to true so it can be grabbed
                secondaryInteractableToRemove.isGrabbable = true;
                secondaryInteractableToRemove.GetComponentInChildren<Collider>().enabled = true;
                secondaryInteractableToRemove.GetComponent<Rigidbody>().useGravity = true;

                // Force grab the object with the secondary grabbing object (the controller/hand) so that it can move with that object's motion
                currentGrabbdObject.ForceStopSecondaryGrabInteraction();
                currentSecondaryGrabbingObject.interactTouch.ForceStopTouching();
                currentSecondaryGrabbingObject.ForceRelease();
                currentSecondaryGrabbingObject.interactTouch.ForceTouch(secondaryInteractableToRemove.gameObject);
                currentSecondaryGrabbingObject.AttemptGrab();

                OnObjectRemovedFromParent(new EventArgs());

                // Enable the GameObject holding the IA that will allow the object to be returned to the parent
                if (returnToParentInteractionAreaObject != null)
                {
                    returnToParentInteractionAreaObject.SetActive(true);
                }
            }
        }
        #endregion
    }
}