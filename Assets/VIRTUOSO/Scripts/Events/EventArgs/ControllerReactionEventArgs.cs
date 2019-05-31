using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// EventArgs for holding controller information as this can be lost between
    /// Interactable Objects and Interaction Areas.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ControllerReactionEventArgs : InteractionAreaEventArgs
    {
        public GameObject interactingController;
    }
}