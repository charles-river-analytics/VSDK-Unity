using System.Collections;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// InteractionArea that requires you to visit the area a given number of times. A visit could either be upon colliding it or
    /// requiring the object to leave the area to count.
    ///  
    /// When combined with the MultiInteractionArea and set to have a count of 1, this functions as a movement based interaction.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class EnterExitInteractionArea : InteractionArea
    {
        #region PublicVariables
        [Tooltip("The number of times an object must enter and exit the interaction area.")]
        public int enterExitCount = 1;
        [Tooltip("When checked, the object counter will go up when the object is first entered. On uncheck, the object must leave the collider for it to increment.")]
        public bool triggerOnEnter = true;
        #endregion

        #region PrivateVariables
        private int interactionCounter = 0;
        #endregion

        #region InteractionAreaOverrides
        public override void OnObjectEnteredInteractionArea(InteractionAreaEventArgs e)
        { 
            base.OnObjectEnteredInteractionArea(e);

            if (currentInteractionObject != null && e.interactionObject == currentInteractionObject.gameObject)
            {
                InteractionEnter(currentInteractionObject.gameObject);
            }
        }

        public override void OnObjectExitedInteractionArea(InteractionAreaEventArgs e)
        {
            base.OnObjectExitedInteractionArea(e);

            InteractionExit(e.interactionObject);
        }
        #endregion

        #region EnterExitFunctions
        private void InteractionEnter(GameObject objectEntered)
        {
            if (interactionCounter == 0) 
            {
                OnObjectUsedInteractionArea(SetInteractionAreaEvent(objectEntered));
            }

            if (triggerOnEnter)
            {
                EnterExitCounter(objectEntered);
            }
        }

        private void InteractionExit(GameObject objectExited)
        {
            if (!triggerOnEnter)
            {
                EnterExitCounter(objectExited);
            }
        }

        private void EnterExitCounter(GameObject interactableObject)
        { 
            interactionCounter++;

            if (interactionCounter == enterExitCount)
            {
                OnObjectFinishedInteractionArea(SetInteractionAreaEvent(interactableObject));
            }
        }
        #endregion
    }
} 