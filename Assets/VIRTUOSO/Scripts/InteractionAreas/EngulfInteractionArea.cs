using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// Interaction area that requires a given collider be completly consumed by the collider
    /// this script is placed on. For example, a needle needs to be inserted into a person,
    /// not just the tip of it, so you would use this script to verify they have completly
    /// entered the correct zone.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class EngulfInteractionArea : InteractionArea
    {
        public Collider colliderToEngulf;

        protected Collider myCollider;

        #region UnityFunctions
        protected override void Awake()
        {
            base.Awake();

            myCollider = GetComponent<Collider>();

            if (colliderToEngulf == null)
            {
                Debug.LogWarning("No collider given to " + name + ". Disabling script");
                enabled = false;
            }
        }

        protected override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (currentInteractionObject != null && collider == colliderToEngulf)
            {
                // Use event
                OnObjectUsedInteractionArea(SetInteractionAreaEvent(collider.gameObject));
            }
        }

        protected override void OnTriggerStay(Collider collider)
        {
            base.OnTriggerStay(collider);

            if (currentInteractionObject != null && collider == colliderToEngulf)
            {
                if (myCollider.bounds.Contains(colliderToEngulf.bounds.min) &&
                    myCollider.bounds.Contains(colliderToEngulf.bounds.max) &&
                    !currentInteractionObject.IsGrabbed())
                {
                    // Finish event
                    OnObjectFinishedInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));
                }
            }
        }

        protected override void OnTriggerExit(Collider collider)
        {
            if (currentInteractionObject != null)
            {
                OnObjectInterruptInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));
            }

            base.OnTriggerExit(collider);
        }
        #endregion
    }
}