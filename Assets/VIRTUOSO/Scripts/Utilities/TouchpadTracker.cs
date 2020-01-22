using System;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Allows for defining extremes values on the X/Y axis of a touchpad. When these values are exceeded, 
    /// an event is sent out that allows it to be tied to a reaction.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [RequireComponent(typeof(VRTK_ControllerEvents))]
    public class TouchpadTracker : MonoBehaviour
    {
        #region PublicVariables
        [Range(-1, 1)]
        public float horizontalMinAxis;
        [Range(-1, 1)]
        public float horizontalMaxAxis;
        [Range(-1, 1)]
        public float verticalMinAxis;
        [Range(-1, 1)]
        public float verticalMaxAxis;
        #endregion

        #region Events
        public event EventHandler<EventArgs> HorizontalMaxAxisPassed;
        public event EventHandler<EventArgs> HorizontalMinAxisPassed;
        public event EventHandler<EventArgs> VerticalMaxAxisPassed;
        public event EventHandler<EventArgs> VerticalMinAxisPassed;

        public virtual void OnHorizontalMaxAxisPassed(EventArgs e)
        {
            if (HorizontalMaxAxisPassed != null)
            {
                HorizontalMaxAxisPassed(this, e);
            }
        }

        public virtual void OnHorizontalMinAxisPassed(EventArgs e)
        {
            if (HorizontalMinAxisPassed != null)
            {
                HorizontalMinAxisPassed(this, e);
            }
        }

        public virtual void OnVerticalMaxAxisPassed(EventArgs e)
        {
            if (VerticalMaxAxisPassed != null)
            {
                VerticalMaxAxisPassed(this, e);
            }
        }

        public virtual void OnVerticalMinAxisPassed(EventArgs e)
        {
            if (VerticalMinAxisPassed != null)
            {
                VerticalMinAxisPassed(this, e);
            }
        }

        private void AttachedControllerEvents_TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if(e.touchpadAxis.x < horizontalMinAxis)
            {
                OnHorizontalMinAxisPassed(new EventArgs());
            }
            else if(e.touchpadAxis.x > horizontalMaxAxis)
            {
                OnHorizontalMaxAxisPassed(new EventArgs());
            }

            if(e.touchpadAxis.y < verticalMinAxis)
            {
                OnVerticalMinAxisPassed(new EventArgs());
            }
            else if(e.touchpadAxis.y > verticalMaxAxis)
            {
                OnVerticalMaxAxisPassed(new EventArgs());
            }
        }
        #endregion

        #region UnityFunctions
        private void Awake()
        {
            VRTK_ControllerEvents attachedControllerEvent = GetComponent<VRTK_ControllerEvents>();

            attachedControllerEvent.TouchpadAxisChanged += AttachedControllerEvents_TouchpadAxisChanged;
        }

        private void OnApplicationQuit()
        {
            VRTK_ControllerEvents attachedControllerEvent = GetComponent<VRTK_ControllerEvents>();

            attachedControllerEvent.TouchpadAxisChanged -= AttachedControllerEvents_TouchpadAxisChanged;
        }
        #endregion
    }
}