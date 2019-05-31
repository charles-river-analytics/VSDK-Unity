using System;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// A component used in conjuction with VRTK_InteractableObject. When placed with an InteractableObject,
    /// it will provide a (specified) EventArgs to be used by reactions.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public abstract class EventArgSender : MonoBehaviour
    {
        public abstract EventArgs GetEventArgs();
    }
}