using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Sends a SnapReactionEventArgs when requested by an InteractableObject.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class SnapEventArgSender : EventArgSender
    {
        #region PublicVariables
        [Tooltip("A specific location on the object where the snap should occur.")]
        public Transform snapTransform;
        [Tooltip("When true, the snapped GameObject's collider will not be disabled when snapped.")]
        public bool canGrabObjectAfterSnap;
        #endregion

        #region EventArgSenderImplementation
        public override EventArgs GetEventArgs()
        {
            SnapReactionEventArgs snapEventArgs = new SnapReactionEventArgs()
            {
                interactionObject = gameObject,
                snapLocation = snapTransform,
                keepObjectGrabbable = canGrabObjectAfterSnap
            };

            return snapEventArgs;
        }
        #endregion
    }
}