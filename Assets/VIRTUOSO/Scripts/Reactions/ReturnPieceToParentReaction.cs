using CharlesRiverAnalytics.Virtuoso.GrabAction;
using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using UnityEngine;
using UnityEngine.Animations;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// A reaction to undo the effects of the RemovePieceGrabAction script. The reaction resets the parent for
    /// the object, disables its colliders, sets gravity off, and doesn't allow the object to be grabbed.
    /// 
    /// To use this script, it must be a child of the InteractableObject with the RemovePieceGrabAction.
    /// 
    /// This script is tightly coupled with InteractionAreaEventArgs (meaning an IA must call this reaction).
    /// This script is tightly coupled with the RemovePieceGrabAction (it undoes the action).
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ReturnPieceToParentReaction : GenericReaction
    {
        #region PublicVariables
        public Vector3 localPosition;
        #endregion

        #region GenericReactionOverride
        public override void StartReaction(object o, EventArgs e)
        {
            InteractionAreaEventArgs interactionAreaEventArgs = e as InteractionAreaEventArgs;

            if(interactionAreaEventArgs != null)
            {
                // Look at the GrabAction to find if the piece that activated the reaction is the one that was originally removed
                RemovePieceGrabAction removeAction = gameObject.GetComponentInParent<RemovePieceGrabAction>();

                if(removeAction.secondaryInteractableToRemove.gameObject == interactionAreaEventArgs.interactionObject)
                {
                    // See if there is a Parent Constraint, if so, uses that to set the parent
                    ParentConstraint parentConstraint = interactionAreaEventArgs.interactionObject.GetComponent<ParentConstraint>();

                    if(parentConstraint != null)
                    {
                        ConstraintSource parentConstraintSource = new ConstraintSource();
                        parentConstraintSource.sourceTransform = removeAction.transform;
                        parentConstraintSource.weight = 1.0f;

                        parentConstraint.AddSource(parentConstraintSource);
                        parentConstraint.SetTranslationOffset(0, localPosition);
                        parentConstraint.constraintActive = true;
                    }
                    // Otherwise, default to transform parent
                    else
                    {
                        interactionAreaEventArgs.interactionObject.transform.parent = removeAction.transform;
                    }

                    // Deactive the collider and rigidbody on the secondary IO so that the main object can be grabbed again
                    interactionAreaEventArgs.interactionObject.GetComponentInChildren<Collider>().enabled = false;
                    interactionAreaEventArgs.interactionObject.GetComponentInChildren<Rigidbody>().useGravity = false;
                    interactionAreaEventArgs.interactionObject.GetComponentInChildren<VRTK_InteractableObject>().isGrabbable = false;

                    // After this has been returned, deactive the IA so that it cannot be triggered again
                    gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}