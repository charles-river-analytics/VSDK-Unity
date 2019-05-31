using UnityEngine;
using System.Collections.Generic;
using VRTK;
using System;
using CharlesRiverAnalytics.Virtuoso.Utilities;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    #region EventPayload
    // Event Payload
    public delegate void InteractionAreaEventHandler(object sender, InteractionAreaEventArgs e);
    #endregion

    public enum InteractionTriggers
    {
        None,
        OnEnter,
        OnExit,
        OnUse,
        OnFinish,
        OnInterrupt
    }

    /// <summary>
    /// 
    /// Class that handles the interaction between the environment and a particular procedure. This class mainly handles the
    /// general event handling and makes sure there is a valid GameObject to use the interaction area. No GameObject should
    /// directly have this script, but use one of it's children since they implement the functionality for the
    /// interaction type.
    /// 
    /// For example, there can be an interaction area that requires an object be in the interaction area for a specific amount
    /// of time.
    /// 
    /// Based on VRTK_SnapDropZone 
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    [System.Serializable]
    public class InteractionArea : VirtuosoEvent
    {
        #region PublicVariables
        [Tooltip("A specified VRTK_PolicyList to use to determine which interactable objects will be able to interact with this specific area.")]
        public VRTK_PolicyList validObjectListPolicy;
        #endregion

        #region PublicProperties
        public VRTK_InteractableObject CurrentInteractionObject
        {
            get
            {
                return currentInteractionObject;
            }
        }
        #endregion

        #region ProtectedVariables
        protected List<GameObject> currentValidObjects = new List<GameObject>();
        protected VRTK_InteractableObject currentInteractionObject = null;
        protected bool interactionStarted = false;
        #endregion

        #region EventHandling
        // Emitted when a valid interactable object enters the interaction area trigger collider.
        public event InteractionAreaEventHandler ObjectEnteredInteractionArea;

        // Emitted when a valid interactable object exists the interaction area trigger collider.
        public event InteractionAreaEventHandler ObjectExitedInteractionArea;

        // Emitted when an interactable object starts the interaction with the area.
        public event InteractionAreaEventHandler ObjectUsedInteractionArea;

        // Emitted when an interactable object finishes the interaction with the area.
        public event InteractionAreaEventHandler ObjectFinishedInteractionArea;

        // Emitted when an interactable object interrupts the user with the area
        public event InteractionAreaEventHandler ObjectInterruptInteractionArea;

        /// <summary>
        /// Called when a valid object enters the collider for the InteractionArea.
        /// </summary> 
        /// <param name="e">The event arguments that holds a reference to the InteractableObject.</param>
        public virtual void OnObjectEnteredInteractionArea(InteractionAreaEventArgs e)
        {
            if (ObjectEnteredInteractionArea != null)
            {
                ObjectEnteredInteractionArea(this, e);
            }
        }

        /// <summary>
        /// Called when the valid object leaves the InteractionArea.
        /// </summary>
        /// <param name="e">The event arguments that holds a reference to the InteractableObject.</param>
        public virtual void OnObjectExitedInteractionArea(InteractionAreaEventArgs e)
        {
            if (ObjectExitedInteractionArea != null)
            {
                ObjectExitedInteractionArea(this, e);
            }
        }

        /// <summary>
        /// Called when the interaction event has started in the InteractionArea.
        /// </summary>
        /// <param name="e">The event arguments that holds a reference to the InteractableObject.</param>
        public virtual void OnObjectUsedInteractionArea(InteractionAreaEventArgs e)
        {
            interactionStarted = true;

            if (ObjectUsedInteractionArea != null)
            {
                ObjectUsedInteractionArea(this, e);
            }
        }

        /// <summary>
        /// Called at the end of the interaction event in the InteractionArea.
        /// </summary>
        /// <param name="e">The event arguments that holds a reference to the InteractableObject.</param>
        public virtual void OnObjectFinishedInteractionArea(InteractionAreaEventArgs e)
        {
            interactionStarted = false;

            if (ObjectFinishedInteractionArea != null)
            {
                ObjectFinishedInteractionArea(this, e);
            }
        }

        public virtual void OnObjectInterruptInteractionArea(InteractionAreaEventArgs e)
        {
            interactionStarted = false;

            if (ObjectInterruptInteractionArea != null)
            {
                ObjectInterruptInteractionArea(this, e);
            }
        }

        /// <summary> 
        /// Sets the given GameObject and returns the Arguments for an InteractableEvent. This method should be
        /// called whenever using one of the 'On' Event Handleing methods.
        /// </summary>
        /// <param name="interactableObject">The GameObject that is triggering the event in the InteractionArea.</param>
        /// <returns></returns>
        public static InteractionAreaEventArgs SetInteractionAreaEvent(GameObject interactableObject)
        {
            VRTK_InteractableObject ioCheck = interactableObject.GetComponent<VRTK_InteractableObject>();
            bool interactableHasExtraArgs = false;

            if (ioCheck != null)
            {
                interactableHasExtraArgs = ioCheck.HasExtraEventArgs;
            }

            InteractionAreaEventArgs e = new InteractionAreaEventArgs
            {
                interactionObject = interactableObject,
                hasMoreReactionInfo = interactableHasExtraArgs
            };

            return e;
        }

        protected void SetUpInteractableObjectListener(VRTK_InteractableObject obj, bool state)
        {
            if (state)
            {
                ObjectEnteredInteractionArea += obj.InteractionAreaEntered;
                ObjectExitedInteractionArea += obj.InteractionAreaExited;
                ObjectUsedInteractionArea += obj.InteractionAreaUsed;
                ObjectFinishedInteractionArea += obj.InteractionAreaFinished;
                ObjectInterruptInteractionArea += obj.InteractionAreaInterrupted;
            }
            else
            {
                ObjectEnteredInteractionArea -= obj.InteractionAreaEntered;
                ObjectExitedInteractionArea -= obj.InteractionAreaExited;
                ObjectUsedInteractionArea -= obj.InteractionAreaUsed;
                ObjectFinishedInteractionArea -= obj.InteractionAreaFinished;
                ObjectInterruptInteractionArea -= obj.InteractionAreaInterrupted;
            }
        }
        #endregion

        #region UnityFunctions
        // TODO Remove or refactor this code - allows for setting the goal area for the InteractionArea
        /*protected virtual void OnEnable()
        {
            if (validObjectListPolicy && validObjectListPolicy.gameObjectList != null)
            {
                for (int n = 0; n < validObjectListPolicy.gameObjectList.Count; n++)
                {
                    // TODO Fix when there are multiple IA for one object [VIRTUOSO-79]
                    if (validObjectListPolicy.gameObjectList[n] != null)
                    {
                        VRTK_InteractableObject interactable = validObjectListPolicy.gameObjectList[n].GetComponentInChildren<VRTK_InteractableObject>();

                        if (interactable != null)
                        {
                            interactable.GoalInteractionArea = gameObject;
                        }
                    }
                }
            }
        }*/

        protected virtual void Awake()
        {
            // Check to make sure that the collider is set to be a trigger in order to recieve the right calls
            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().isTrigger = true;
            }
            else
            {
                Debug.LogWarning("No collider found on " + name + ". Triggers will not be detected and events cannot be propogated.");
            }
        }

        protected virtual void Update()
        {
            CheckCurrentValidObjectStillValid();
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            VRTK_InteractableObject ioCheck = ValidInteractableObject(collider.gameObject);

            if (ioCheck && ioCheck != currentInteractionObject)
            {
                currentInteractionObject = ioCheck;

                SetUpInteractableObjectListener(ioCheck, true);

                OnObjectEnteredInteractionArea(SetInteractionAreaEvent(currentInteractionObject.gameObject));
            }
        }

        protected virtual void OnTriggerStay(Collider collider)
        {
            //Do sanity check to see if there should be an object
            if (ValidInteractableObject(collider.gameObject))
            {
                AddCurrentValidObject(collider.gameObject);
            }
        }

        protected virtual void OnTriggerExit(Collider collider)
        {
            VRTK_InteractableObject ioCheck = ValidInteractableObject(collider.gameObject);

            if (ioCheck)
            {
                OnObjectExitedInteractionArea(SetInteractionAreaEvent(ioCheck.gameObject));

                currentInteractionObject = null;

                SetUpInteractableObjectListener(ioCheck, false);
            }
        }
        #endregion

        #region ObjectChecking
        protected virtual VRTK_InteractableObject ValidInteractableObject(GameObject checkObject)
        {
            var currentInteractableObject = checkObject.GetComponentInParent<VRTK_InteractableObject>();
            return (currentInteractableObject != null && !VRTK_PolicyList.Check(currentInteractableObject.gameObject, validObjectListPolicy) ? currentInteractableObject : null);
        }

        protected virtual void CheckCurrentValidObjectStillValid()
        {
            //If there is a current valid snap object
            for (int i = 0; i < currentValidObjects.Count; i++)
            {
                var currentIOCheck = currentValidObjects[i].GetComponentInParent<VRTK_InteractableObject>();

                if (currentIOCheck != null /*&& currentIOCheck.StoredInteractionArea != null && currentIOCheck.StoredInteractionArea != gameObject*/)
                {
                    RemoveCurrentValidObject(currentValidObjects[i]);
                }
            }
        }

        protected virtual void AddCurrentValidObject(GameObject givenObject)
        {
            if (!currentValidObjects.Contains(givenObject))
            {
                currentValidObjects.Add(givenObject);
            }
        }

        protected virtual void RemoveCurrentValidObject(GameObject givenObject)
        {
            if (currentValidObjects.Contains(givenObject))
            {
                currentValidObjects.Remove(givenObject);

                if (currentValidObjects.Count == 0)
                {
                    interactionStarted = false;
                }
            }
        }
        #endregion
    }
}