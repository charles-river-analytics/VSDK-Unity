using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Reaction to check if the caller is inside the given interaction area to check. This allows you
    /// to indicate that an event or interaction must happen inside the given the interaction area.
    /// 
    /// Coupled with InteractableObject (must be the caller) and InteractionAreas.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class CheckInsideInteractionAreaReaction : GenericReaction
    {
        #region PublicVariable
        public InteractionArea interactionAreaToCheck;
        #endregion

        #region EventHandling
        public event EventHandler<EventArgs> ObjectCompletedActionInInteractionArea;

        public virtual void OnObjectCompletedActionInInteractionArea(VRTK_InteractableObject originalObject, EventArgs e)
        {
            if (ObjectCompletedActionInInteractionArea != null)
            {
                ObjectCompletedActionInInteractionArea(originalObject, e);
            }
        }
        #endregion

        #region GenericReactionOverride
        public override void StartReaction(object o, EventArgs e)
        {
            VRTK_InteractableObject interactableObject = o as VRTK_InteractableObject;

            if (interactableObject != null)
            {
                if(interactableObject.StoredInteractionAreas.Contains(interactionAreaToCheck))
                {
                    // Send an event so it can be daisy chained with other reactions
                    OnObjectCompletedActionInInteractionArea(interactableObject, e);
                }
            }
        }
        #endregion
    }
}