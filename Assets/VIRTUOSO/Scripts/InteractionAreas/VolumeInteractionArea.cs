using CharlesRiverAnalytics.Virtuoso.Interfaces;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// The volume interaction area is a way for something to be filled and send an event when it reaches the threshold level.
    /// The volume does not need to be fluid based, such as with the filling of a wound with gauze. The user specifies how
    /// much volume needs to be filled and the interactable object must tell it how much it fills it by.
    ///  
    /// Written by: Nicolas Herrera, 2018
    /// </summary>
    public class VolumeInteractionArea : InteractionArea
    {
        #region PublicVariables
        [Tooltip("The total amount of volume that needs to be filled in cm^3.")]
        public float totalVolume = 1.0f;
        #endregion

        #region PrivateVariables
        private float currentVolumeFilled = 0.0f;
        private bool reachedTotalVolume = false;
        #endregion

        #region UnityFunctions
        protected override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);
 
            if(currentInteractionObject != null)
            {
                IVolume volumeFiller = currentInteractionObject as IVolume;

                if(volumeFiller != null)
                {
                    currentVolumeFilled += volumeFiller.GetVolumeAmount();

                    // TODO Make it look like the volume has changed (maybe in a shader) VolumeReaction -[VIRTUOSO-81]

                    if (!interactionStarted)
                    {
                        interactionStarted = true;

                        OnObjectUsedInteractionArea(SetInteractionAreaEvent(collider.gameObject));
                    }
                }
            }
        }
 
        protected override void OnTriggerStay(Collider collider)
        {
            base.OnTriggerStay(collider);

            if (currentInteractionObject != null )
            {
                if(currentVolumeFilled >= totalVolume && !reachedTotalVolume)
                {
                    OnObjectFinishedInteractionArea(SetInteractionAreaEvent(collider.gameObject));

                    // Allow this message to only be called once
                    reachedTotalVolume = true;
                }
            }
        }
        #endregion
    }
}

 