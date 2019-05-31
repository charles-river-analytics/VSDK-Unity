using UnityEngine;
using System.Collections;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// InteractionArea that is based on time. The user specifies how long to make the timer. After a valid object
    /// enters the area, the timer will start. After the given amount of time, the finish event will go off.
    ///  
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class TimeInteractionArea : InteractionArea
    {
        #region PublicVariables
        [Tooltip("The amount of time (in milliseconds) it takes for the countdown to reach 0.")]
        public int timeDuration = 5000;
        #endregion

        #region ProtectedVariables
        // Maintain single reference to a coroutine so you can only handle one timer at a time
        protected Coroutine countdownTimeCoroutine;
        #endregion

        #region UnityFunctions
        // While any GameObject will send a trigger on this area, a check is made to make sure the gameobject attached to the collider inherits from VRTK_InteractableObject
        protected override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);
 
            if (currentInteractionObject != null)
            {
                if(countdownTimeCoroutine == null)
                {
                    OnObjectUsedInteractionArea(SetInteractionAreaEvent(collider.gameObject));

                    // Convert the given time in ms to seconds by dividing by the number of ms in a second
                    countdownTimeCoroutine = StartCoroutine(Countdown(timeDuration / Utilities.Constants.MS_TO_SECONDS));
                }
            }
        }

        // Same with OnTriggerEnter, only VRTK_InteractableObjects should actually do anything on this trigger
        protected override void OnTriggerExit(Collider collider)
        {
            if (currentInteractionObject != null)
            {
                if (countdownTimeCoroutine != null) 
                {
                    StopCoroutine(countdownTimeCoroutine);

                    countdownTimeCoroutine = null;

                    OnObjectInterruptInteractionArea(SetInteractionAreaEvent(collider.gameObject));
                }
            }

            base.OnTriggerExit(collider);
        }
        #endregion

        #region Coroutines
        IEnumerator Countdown(float duration)
        {
            yield return new WaitForSeconds(duration);

            if(currentInteractionObject != null)
            {
                OnObjectFinishedInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));
            } 
        }
        #endregion
    }
}