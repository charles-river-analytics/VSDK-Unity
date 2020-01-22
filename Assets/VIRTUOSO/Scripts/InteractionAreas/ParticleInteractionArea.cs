using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using System.Collections;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// An Interaction Area for detecting collisions with particles from a particle system. In order for this to receive the 
    /// Particle Collision event, the particle system must enable collision and check the 'Send Message' option. 
    /// 
    /// Note: This IA only sends out the ObjectEnteredInteractionArea event.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ParticleInteractionArea : InteractionArea
    {
        #region PrivateVariables
        // Since no exit event is sent, clear the currentInteractionObject after a few second delay
        private const float CLEAR_INTERACTABLE_DELAY = 2.0f;
        private bool coroutineStarted = false;
        #endregion

        #region PrivateFunctions
        private IEnumerator EndParticleCollision()
        {
            yield return new WaitForSeconds(CLEAR_INTERACTABLE_DELAY);

            currentInteractionObject = null;

            coroutineStarted = false;
        }
        #endregion

        #region UnityFunctions
        void OnParticleCollision(GameObject other)
        {
            VRTK_InteractableObject ioCheck = ValidInteractableObject(other.GetComponentInParent<VRTK_InteractableObject>()?.gameObject);

            if (ioCheck != null && ioCheck != currentInteractionObject)
            {
                currentInteractionObject = ioCheck;

                OnObjectEnteredInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));

                if (!coroutineStarted)
                {
                    coroutineStarted = true;

                    // Since there is no particle exit event, clear the interaction objects after a set delay
                    StartCoroutine(EndParticleCollision());
                }
            }
        }
        #endregion
    }
}