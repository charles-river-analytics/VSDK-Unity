using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// An Interaction Area for tracking the movement speed of objects. The developer can set limits in the editor.
    /// If a limit is exceeded, then an event is sent.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class VelocityTrackerInteractionArea : InteractionArea
    {
        #region PublicVariables
        [Tooltip("The max limit the object can move in the x direction. If left at 0, it is untracked.")]
        public float xAxisSpeedLimit;
        [Tooltip("The max limit the object can move in the y direction. If left at 0, it is untracked.")]
        public float yAxisSpeedLimit;
        [Tooltip("The max limit the object can move in the z direction. If left at 0, it is untracked.")]
        public float zAxisSpeedLimit;
        #endregion

        #region PrivateVariables
        VRTK_VelocityEstimator currentInteractingVelocityEstimator;
        VRTK_VelocityEstimator addedVelocityEstimator;
        #endregion

        #region UnityFunctions
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if(CurrentInteractionObject != null)
            {
                // See if there is a velocity estimator, if not add one, since it is needed to track the user's velocity movements
                currentInteractingVelocityEstimator = CurrentInteractionObject.GetComponentInParent<VRTK_VelocityEstimator>();

                if(currentInteractingVelocityEstimator == null)
                {
                    // Add the velocity estimator to the GameObject holding the Rigidbody since it uses local positions to calculate its estimates
                    GameObject gameObjectRigidbody = CurrentInteractionObject.GetComponentInParent<Rigidbody>().gameObject;

                    if(gameObjectRigidbody != null)
                    {
                        addedVelocityEstimator = gameObjectRigidbody.AddComponent<VRTK_VelocityEstimator>();
                    }
                    // Otherwise, just add it to the current GameObject in the IA
                    else
                    {
                        addedVelocityEstimator = CurrentInteractionObject.gameObject.AddComponent<VRTK_VelocityEstimator>();
                    }

                    currentInteractingVelocityEstimator = addedVelocityEstimator;
                }
            }
        }

        protected override void OnTriggerStay(Collider collider)
        {
            base.OnTriggerStay(collider);

            if(currentInteractingVelocityEstimator != null)
            {
                Vector3 currentVelocity = currentInteractingVelocityEstimator.GetVelocityEstimate();

                // Check each axis for the velocity limit
                if(xAxisSpeedLimit != 0)
                {
                    if(Mathf.Abs(currentVelocity.x) > xAxisSpeedLimit)
                    {
                        OnObjectFinishedInteractionArea(new InteractionAreaEventArgs());
                    }
                }

                if(yAxisSpeedLimit != 0)
                {
                    if (Mathf.Abs(currentVelocity.y) > yAxisSpeedLimit)
                    {
                        OnObjectFinishedInteractionArea(new InteractionAreaEventArgs());
                    }
                }

                if(zAxisSpeedLimit != 0)
                {
                    if (Mathf.Abs(currentVelocity.z) > zAxisSpeedLimit)
                    {
                        OnObjectFinishedInteractionArea(new InteractionAreaEventArgs());
                    }
                }
            }
        }

        protected override void OnTriggerExit(Collider collider)
        {
            base.OnTriggerExit(collider);

            if(addedVelocityEstimator != null)
            {
                Destroy(addedVelocityEstimator);
            }

            currentInteractingVelocityEstimator = null;
        }
        #endregion
    }
}