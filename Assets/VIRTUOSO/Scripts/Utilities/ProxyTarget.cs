using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// This utility class works with the ProxyReaction. This is meant as a placehold for dynamic objects. If you don't know
    /// what object will call the reaction (for example, having multiple different gloves, but the exact glove doesn't matter), then
    /// place this script on the object. When a ProxyReaction is triggered, if the object that triggers it has this script attached,
    /// then these reactions will be fired as attached to the ProxyTarget.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ProxyTarget : MonoBehaviour
    {
        #region PublicVariables
        public List<GenericReaction> reactionsToFire;
        #endregion

        #region PublicAPI
        public void FireReactions(object o, EventArgs e)
        {
            foreach(GenericReaction reaction in reactionsToFire)
            {
                reaction.StartReaction(o, e);
            }
        }
        #endregion
    }
}