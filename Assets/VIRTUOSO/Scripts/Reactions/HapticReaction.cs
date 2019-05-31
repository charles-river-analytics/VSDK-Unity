using System;
using CharlesRiverAnalytics.Virtuoso.Haptic;
using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Reaction event for playing haptics with the Haptic SDK.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class HapticReaction : GenericReaction
    {
        #region PublicVariables
        public HumanBodyBones bodyPart;
        public ScriptableHapticPattern hapticPattern;
        #endregion

        #region PrivateVariables
        private HapticManager hapticManager;
        #endregion

        #region ReactionEventImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            hapticManager.PlayPattern(bodyPart, hapticPattern);
        }

        public override void StopReaction(object o, EventArgs e)
        {
            hapticManager.CancelPatternPlayingOnBody(bodyPart);
        }
        #endregion

        #region UnityFunction
        private void Start()
        {
            hapticManager = FindObjectOfType<HapticManager>();

            if(hapticManager == null)
            {
                Debug.LogError("Cannot play any haptic reactions as there is no haptic manager in the scene.", this);

                enabled = false;
            }
        }
        #endregion
    }
}