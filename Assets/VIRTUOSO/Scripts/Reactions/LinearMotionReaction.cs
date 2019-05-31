using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Event Payload 
    /// Extends the InteractionAreaEvent so it can get a value to lerp between.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Jan 2019
    /// </summary>
    public class LinearMotionReactionArgs : InteractionAreaEventArgs
    {
        public float lerpAmount;

        public static LinearMotionReactionArgs SetInteractionAreaEvent(GameObject interactableObject, float lerpValue)
        {
            LinearMotionReactionArgs e = new LinearMotionReactionArgs
            {
                interactionObject = interactableObject,
                lerpAmount = lerpValue
            };

            return e;
        }
    }

    /// <summary>
    /// A reaction to linearly move an object based on it's position at the start and a given
    /// direction and magnitude.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Jan 2019
    /// </summary>
    public class LinearMotionReaction : GenericReaction
    {
        #region PublicVariables
        [Tooltip("The object whose position will be affected by this reaction. Defaults to the GameObject this is attached to.")]
        public GameObject rootObject;
        [Tooltip("The change in local position the object must traverse.")]
        public float stopMagnitude;
        [Tooltip("The direction the stop position is in. Based on the attached object's transform.")]
        public Vector3 stopDirection = Vector3.back;
        #endregion

        #region PrivateVariables
        private Vector3 orignalRootPosition;
        private Vector3 traversedRootPosition;
        #endregion

        #region Unity Functions
        protected void OnEnable()
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            orignalRootPosition = rootObject.transform.position;

            traversedRootPosition = rootObject.transform.position + (stopDirection.normalized * stopMagnitude);
        }
        #endregion

        #region GenericReactionOverrideImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            LinearMotionReactionArgs args = e as LinearMotionReactionArgs;

            rootObject.transform.position = Vector3.Lerp(orignalRootPosition, traversedRootPosition, args.lerpAmount);
        }

        [HideMethodFromInspector]
        public override void StopReaction(object o, EventArgs e)
        {
            // No-op
        }
        #endregion
    }
}