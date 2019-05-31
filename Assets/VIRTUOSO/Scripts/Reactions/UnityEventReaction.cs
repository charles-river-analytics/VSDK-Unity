using System;
using UnityEngine.Events;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// A simple, but powerful class. If there is a specific function that needs to be called as a 
    /// reaction, simply use this class and fill in the GameObject and function in the UnityEvent.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// </summary>
    public class UnityEventReaction : GenericReaction
    {
        #region PublicVariables
        public UnityEvent eventSystem;
        #endregion

        #region ReactionEventImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            eventSystem.Invoke();
        }

        [HideMethodFromInspector]
        public override void StopReaction(object o, EventArgs e)
        {
            // No-op
        }
        #endregion
    }
}