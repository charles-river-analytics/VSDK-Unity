using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Reaction to apply a force to a GameObject. If the given game object does not a have a rigid body,
    /// then one is added to it automatically. If the object is set to be kinematic, then that option will be set
    /// to false so that the force can be applied.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class ForceReaction : GenericReaction
    {
        [Tooltip("The direction and force that should be applied to the GameObject.")]
        public Vector3 forceDirection = Vector3.forward;
        [Tooltip("How the force is applied to the GameObject. Please see Unity documentation for more information.")]
        public ForceMode forceMode = ForceMode.Impulse;
        [Tooltip("Optional parameter. Fill in if this script is not attached to the GameObject that will receive the force.")]
        public GameObject objectToApplyForce;

        #region UnityFunctions
        public void Awake()
        {
            if(objectToApplyForce == null)
            {
                objectToApplyForce = gameObject;
            }
        }
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            Rigidbody attachedRigidbody = objectToApplyForce.GetComponent<Rigidbody>();

            if(attachedRigidbody == null)
            {
                attachedRigidbody = objectToApplyForce.AddComponent<Rigidbody>();
            }

            attachedRigidbody.isKinematic = false;

            attachedRigidbody.AddForce(forceDirection, forceMode);
        }

        [HideMethodFromInspector]
        public override void StopReaction(object o, EventArgs e)
        {
            // No-op
        }
        #endregion
    }
}