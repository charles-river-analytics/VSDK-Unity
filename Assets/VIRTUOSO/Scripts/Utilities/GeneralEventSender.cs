using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Helper class that gives any subclass the ability to send an event without creating a specific one for itself.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class GeneralEventSender : MonoBehaviour
    {
        #region Events
        public event EventHandler<EventArgs> ActionTaken;

        protected void OnActionComplete(EventArgs e)
        {
            ActionTaken?.Invoke(this, e);
        }

        protected void OnActionComplete(string action)
        {
            ActionTaken?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}