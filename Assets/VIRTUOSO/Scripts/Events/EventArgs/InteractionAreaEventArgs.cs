using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// Event Payload 
    /// The game object that causes the event to go off in the Interaction Area
    /// </summary>
    public class InteractionAreaEventArgs : EventArgs
    {
        public GameObject interactionObject;
        // When marked true, a reaction will query the interactionObject for it's own special EventArgs
        // since many reactions can be hooked into a single event
        public bool hasMoreReactionInfo = false;
    }
}