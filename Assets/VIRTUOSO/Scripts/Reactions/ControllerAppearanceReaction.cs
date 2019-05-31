using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Similiar to VRTK_InteractObjectAppearance, allows for the controller's appearance to be
    /// changed based on any event.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Feb 2019
    /// </summary>
    public class ControllerAppearanceReaction : GenericReaction
    {
        #region PublicVariables
        #endregion

        #region PrivateVariables
        private GameObject controllerModel;
        #endregion

        #region ApperanceMethods
        private GameObject GetControllerModelFromEvent(EventArgs e)
        {
            InteractionAreaEventArgs interactionAreaEventArgs = e as InteractionAreaEventArgs;

            if (interactionAreaEventArgs != null)
            {
                if (interactionAreaEventArgs.hasMoreReactionInfo)
                {
                    ControllerEventArgSender eventArgsSender = interactionAreaEventArgs.interactionObject.GetComponent<ControllerEventArgSender>();

                    if (eventArgsSender != null)
                    {
                        ControllerReactionEventArgs eventArgs = eventArgsSender.GetEventArgs() as ControllerReactionEventArgs;

                        return VRTK_DeviceFinder.GetModelAliasController(eventArgs.interactingController);
                    }
                }
            }

            return null;
        }
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            // See if this is directly an InteractableObject event
            InteractableObjectEventArgs interactableArgs = e as InteractableObjectEventArgs;

            if(interactableArgs != null)
            {
                controllerModel = VRTK_DeviceFinder.GetModelAliasController(interactableArgs.interactingObject);
            }
            else
            {
                controllerModel = GetControllerModelFromEvent(e);   
            }

            if (controllerModel != null)
            {
                VRTK_ObjectAppearance.SetRendererHidden(controllerModel);
            }
        }

        public override void StopReaction(object o, EventArgs e)
        {
            if (controllerModel != null)
            {
                VRTK_ObjectAppearance.SetRendererVisible(controllerModel);
            }

            controllerModel = null;
        }
        #endregion

        #region Unity Functions
        void Start()
        {

        }

        void Update()
        {

        }
        #endregion
    }
}