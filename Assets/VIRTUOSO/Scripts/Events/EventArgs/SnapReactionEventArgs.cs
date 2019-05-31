using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// Helper class for the Snap Reaction. If extra information is needed for snapping, this class 
    /// can be expanded to hold the information and get passed to the Interaction Area when the 
    /// InteractableObject interacts with the IA.
    /// </summary>
    public class SnapReactionEventArgs : InteractionAreaEventArgs
    {
        public Transform snapLocation;
        public bool keepObjectGrabbable;
    }
}