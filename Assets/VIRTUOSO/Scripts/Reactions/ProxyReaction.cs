using CharlesRiverAnalytics.Virtuoso.Utilities;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// A reaction for dynamic objects. If you cannot hook an object in at design time in the editor, then use this script.
    /// Put a ProxyTarget on the object and fill out a list of reactions that object needs to take. If this reaction is called by the
    /// object with the ProxyTarget, then those reactions will be fired by the start of this reaction.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ProxyReaction : GenericReaction
    {
        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            Component componentSender = o as Component;

            if(componentSender != null)
            {
                ProxyTarget target = componentSender.gameObject.GetComponent<ProxyTarget>();

                if (target != null)
                {
                    target.FireReactions(o, e);
                }
            }
        }
        #endregion
    }
}