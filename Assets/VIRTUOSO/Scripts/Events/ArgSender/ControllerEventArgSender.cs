using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using System;
using VRTK;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Ensures that the controller that causes the initial interaction on an InteractableObject is passed to
    /// a reaction if needed.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class ControllerEventArgSender : EventArgSender
    {
        #region PrivateVariables
        private VRTK_InteractableObject interactableObject;
        private GameObject controllerGameObject;
        #endregion

        #region EventArgSenderImplementation
        public override EventArgs GetEventArgs()
        {
            ControllerReactionEventArgs snapEventArgs = new ControllerReactionEventArgs()
            {
                interactingController = controllerGameObject
            };

            return snapEventArgs;
        }
        #endregion

        #region EventHandlers
        private void InteractingObject_InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            controllerGameObject = null;
        }

        private void InteractingObject_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            controllerGameObject = e.interactingObject.transform.parent.gameObject;
        }
        #endregion

        #region UnityFunctions
        private void Awake()
        {
            interactableObject = GetComponent<VRTK_InteractableObject>();

            interactableObject.InteractableObjectGrabbed += InteractingObject_InteractableObjectGrabbed;
            interactableObject.InteractableObjectUngrabbed += InteractingObject_InteractableObjectUngrabbed;
        }

        private void OnApplicationQuit()
        {
            interactableObject.InteractableObjectGrabbed -= InteractingObject_InteractableObjectGrabbed;
            interactableObject.InteractableObjectUngrabbed -= InteractingObject_InteractableObjectUngrabbed;
        }
        #endregion
    }
}