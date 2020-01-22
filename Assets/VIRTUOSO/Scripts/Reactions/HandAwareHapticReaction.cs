using UnityEngine;
using CharlesRiverAnalytics.Virtuoso.Haptic;
using System;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// This reaction uses contextual information to change which hand recieves a haptic burst on StartReaction.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) Updated: December 2019
    /// </summary>
    public class HandAwareHapticReaction : GenericReaction
    {

        #region PublicVariables
        [Tooltip("Haptic areas to actuate if the left hand triggered the reaction")]
        public HumanBodyBones[] leftBodyParts = new HumanBodyBones[] { HumanBodyBones.LeftHand, HumanBodyBones.LeftLowerArm };
        [Tooltip("Haptic areas to actuate if the right hand triggered the reaction")]
        public HumanBodyBones[] rightBodyParts = new HumanBodyBones[] { HumanBodyBones.RightHand, HumanBodyBones.RightLowerArm };
        [Tooltip("The pattern to play on StartReaction")]
        public ScriptableHapticPattern hapticPattern;
        #endregion

        #region PrivateVariables
        private HapticManager hapticManager;
        // reference needed to deactive on stop
        private HumanBodyBones[] partsToActivate;
        #endregion

        #region ReactionEventImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            InteractableObjectEventArgs interactionArgs = (InteractableObjectEventArgs)e;
            if (interactionArgs != null)
            {
                partsToActivate = null;
                
                if(VRTK_SDK_Bridge.IsControllerLeftHand(interactionArgs.interactingObject) 
                    || VRTK_SDK_Bridge.GetHandSDK().GetLeftHand() == interactionArgs.interactingObject)
                {
                    partsToActivate = leftBodyParts;
                }
                else if (VRTK_SDK_Bridge.IsControllerRightHand(interactionArgs.interactingObject)
                    || VRTK_SDK_Bridge.GetHandSDK().GetRightHand() == interactionArgs.interactingObject)
                {
                    partsToActivate = rightBodyParts;
                }

                if(partsToActivate != null)
                {
                    foreach(HumanBodyBones bodyPart in partsToActivate)
                    {
                        hapticManager.PlayPattern(bodyPart, hapticPattern);
                    }
                }
            }
        }

        public override void StopReaction(object o, EventArgs e)
        {
            if(partsToActivate != null)
            {
                foreach (HumanBodyBones bodyPart in partsToActivate)
                {
                    hapticManager.CancelPatternPlayingOnBody(bodyPart);
                }
                partsToActivate = null;
            }
        }
        #endregion

        #region UnityFunction
        private void Start()
        {
            hapticManager = FindObjectOfType<HapticManager>();

            if (hapticManager == null)
            {
                Debug.LogError("Cannot play any haptic reactions as there is no haptic manager in the scene.", this);

                enabled = false;
            }
        }
        #endregion
    }
}
