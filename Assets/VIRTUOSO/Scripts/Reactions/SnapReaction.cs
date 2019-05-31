using System;
using System.Collections; 
using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// This sets up a predefined area where an existing interactable object can be snapped to. Since it's a reaction,
    /// any interaction area's finish event can trigger the snap condition. By default, the snap will occur to the 
    /// transform on the GameObject where it is attached. One can specify any arbitrary transform or ignore rotation
    /// if needed.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class SnapReaction : GenericReaction
    {
        #region PublicVariables
        [Tooltip("When checked, the object will snap by becoming a child of the given transform. If this is unchecked and no joint is " +
            "attached to the gameobject, the script will default to snap by parenting.")]
        public bool snapByParenting = false;
        [Tooltip("The amount of time it takes for the object being snapped to move into the new snapped position and rotation.")]
        public float snapDuration = 0f;
        [Tooltip("When checked, the object will not move to a specific transform but stay snapped where it was released.")]
        public bool snapToReleaseArea = false;
        [Tooltip("Set this to any transform if there is a specific rotation or location you want the object to snap to. If set, it " +
            "will override all other transform settings.")]
        public Transform specifiedTransform;
        [Tooltip("When checked, the object will only move to the snap location, but not rotate in the direction of the transform.")]
        public bool ignoreRotation = false;
        [Tooltip("The amount of force required to break the joint, if snapping by joint.")]
        public float jointBreakForce = 0.0f;
        [Tooltip("The normalized direction of force required to break the joint, if snapping by joint.")]
        public Vector3 jointBreakDirection = Vector3.zero;
        #endregion

        #region PrivateVariables
        private Joint attachmentJoint;
        private Transform originalParent;
        private GameObject snappedObject;
        private float additionalJointBreakForce;

        // Amount of frames that must pass before an object can be unsnapped by breaking the joint
        // This helps with a few cases where the tool wants to unsnap right after snapping
        private const int FRAMES_BEFORE_UNSNAP = 60;
        // The angle amount (in degrees) that the current force on the joint must reach in order to break
        private const float angleToBreakSnap = 90.0f;
        private int snappedAtFrame = 0;
        #endregion

        #region UnityFunctions
        protected void Awake()
        {
            attachmentJoint = GetComponent<Joint>();

            if (!snapByParenting && attachmentJoint == null)
            {
                snapByParenting = true;
                Debug.LogWarning("No joint on SnapReaction script attached to " + name + ". Switching to snap by parenting.");
            }

            jointBreakDirection = jointBreakDirection.normalized;
        }

        protected void Update()
        {
            // Since Unity's API causes a joint to be removed when broken, check for a 'break' here and simply remove the connecting body
            if(attachmentJoint?.connectedBody != null && Time.frameCount - snappedAtFrame > FRAMES_BEFORE_UNSNAP)
            {
                float angleDifference = Vector3.Angle(jointBreakDirection, attachmentJoint.currentForce);

                if (angleDifference < angleToBreakSnap)
                {
                    // The greater the angle difference, the greater the extra force is needed to 'break' the joint
                    float additionalForce = jointBreakForce * (angleDifference / 90.0f);

                    if(attachmentJoint.currentForce.z > (additionalForce + jointBreakForce))
                    {
                        attachmentJoint.connectedBody = null;
                    }
                }
            }
        }
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            InteractionAreaEventArgs interactionAreaEventArgs = e as InteractionAreaEventArgs;

            if (interactionAreaEventArgs != null)
            {
                // See if there is more data that is needed for this reaction
                if(interactionAreaEventArgs.hasMoreReactionInfo)
                {
                    SnapEventArgSender snapEventSender = interactionAreaEventArgs.interactionObject.GetComponent<SnapEventArgSender>();

                    if (snapEventSender != null)
                    {
                        SnapReactionEventArgs snapEventArgs = snapEventSender.GetEventArgs() as SnapReactionEventArgs;
                        SnapObject(snapEventArgs.interactionObject, snapEventArgs.snapLocation, snapEventArgs.keepObjectGrabbable);
                    }
                    else
                    {
                        SnapObject(interactionAreaEventArgs.interactionObject);
                    }
                }
                else
                {
                    SnapObject(interactionAreaEventArgs.interactionObject);
                }
            }
        }

        public override void StopReaction(object o, EventArgs e)
        {
            UnsnapObject();
        }
        #endregion

        #region SnapFunctions
        protected virtual void SnapObject(GameObject interactionObject, Transform snapOffset = null, bool objectIsGrabbable = false)
        {
            VRTK_InteractableObject currentInteractableObject = interactionObject.GetComponentInParent<VRTK_InteractableObject>();

            //If the item is in a snappable position and this drop zone isn't snapped and the collider is a valid interactable object
            if (currentInteractableObject != null)
            {
                snappedObject = currentInteractableObject.gameObject;
                snappedAtFrame = Time.frameCount;

                // Make sure the object being snapped doesn't start to interfere with other objects
                if (!objectIsGrabbable)
                {
                    currentInteractableObject.GetComponentInChildren<Collider>().enabled = false;
                }

                StartCoroutine(UpdateTransformDimensions(currentInteractableObject, snapDuration, snapOffset));
            }
        }

        protected virtual void UnsnapObject()
        {
            if (attachmentJoint != null)
            {
                attachmentJoint.connectedBody = null;
            }
            else
            {
                snappedObject.transform.parent = originalParent;

                originalParent = null;
            }

            snappedObject = null;
        }

        protected virtual IEnumerator UpdateTransformDimensions(VRTK_InteractableObject ioCheck, float duration, Transform snapOffset = null)
        {
            
            Transform ioTransform = ioCheck.transform;
            Quaternion startRotation = ioTransform.rotation;
            Vector3 startScale = ioTransform.localScale;
            Vector3 startPosition = ioTransform.position;

            bool storedKinematicState = ioCheck.isKinematic;
            ioCheck.isKinematic = true;
            
            Transform endTransform;

            if (specifiedTransform != null)
            {
                endTransform = specifiedTransform;
            } 
            else if (snapToReleaseArea)
            {
                endTransform = ioTransform;
            }
            else
            {
                endTransform = gameObject.transform;
            }

            float elapsedTime = 0f;

            while (elapsedTime <= duration && !snapToReleaseArea)
            {
                elapsedTime += Time.deltaTime;

                if (ioTransform != null)
                {
                    ioTransform.position = Vector3.Lerp(startPosition, endTransform.position, (elapsedTime / duration));

                    if (snapOffset != null)
                    {
                        ioTransform.position += snapOffset.localPosition;
                    }

                    if (!ignoreRotation)
                    {
                        ioTransform.rotation = Quaternion.Lerp(startRotation, endTransform.rotation, (elapsedTime / duration));
                    }
                }

                yield return null;
            }

            //Force all to the last setting in case anything has moved during the transition
            ioTransform.position = endTransform.position;

            if (snapOffset != null)
            {
                ioTransform.position += snapOffset.localPosition;
            }

            if (!ignoreRotation)
            {
                ioTransform.rotation = endTransform.rotation;
            }

            ioCheck.isKinematic = storedKinematicState;
            ioCheck.SaveCurrentState();

            SetSnapJoint(ioCheck);
        }

        protected virtual void SetSnapJoint(VRTK_InteractableObject interactableObject)
        {
            Rigidbody snapTo = interactableObject.GetComponent<Rigidbody>();

            // No rigidbody on the object, use parenting instead
            if (snapTo != null && !snapByParenting)
            {
                // Remove all forces on the object
                snapTo.useGravity = false;
                snapTo.velocity = Vector3.zero;
                snapTo.angularVelocity = Vector3.zero;

                if (attachmentJoint == null)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "SnapInteractionArea:" + name, "Joint", "the same", " because the `Snap Type` is set to `Use Joint`"));
                    return;
                }

                attachmentJoint.connectedBody = snapTo;
            }
            else
            {
                originalParent = interactableObject.transform.parent;

                interactableObject.transform.parent = transform;
            }
        }
        #endregion
    }
} 