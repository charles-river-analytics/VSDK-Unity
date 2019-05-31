using UnityEngine.Events;
using VRTK.UnityEventHelper;
using System;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// Allows the user to specify event functions within the editor or use the AddListener function
    /// to create reactions to InteractionArea events. This class must be attached to a GameObject 
    /// that also has the InteractionArea (or child)
    /// attached to it.
    /// 
    /// Based on VRTK_UnityEvents
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public sealed class InteractionArea_UnityEvents : VRTK_UnityEvents<InteractionArea>
    {
        #region PublicVariables
        [Serializable]
        public sealed class InteractionAreaEvent : UnityEvent<object, InteractionAreaEventArgs> { }

        public InteractionAreaEvent OnObjectEnteredInteractionArea = new InteractionAreaEvent();
        public InteractionAreaEvent OnObjectExitedInteractionArea = new InteractionAreaEvent();
        public InteractionAreaEvent OnObjectStartInteraction = new InteractionAreaEvent();
        public InteractionAreaEvent OnObjectInterruptInteraction = new InteractionAreaEvent();
        public InteractionAreaEvent OnObjectFinishedInteraction = new InteractionAreaEvent();
        #endregion

        #region ListenerMangers
        protected override void AddListeners(InteractionArea component)
        {
            component.ObjectEnteredInteractionArea += ObjectEnteredInteractionArea;
            component.ObjectExitedInteractionArea += ObjectExitedInteractionArea;
            component.ObjectInterruptInteractionArea += ObjectUnusedInteractionArea;
            component.ObjectUsedInteractionArea += ObjectUsedInteractionArea;
            component.ObjectFinishedInteractionArea += ObjectFinishedInteractionArea;
        }

        protected override void RemoveListeners(InteractionArea component)
        {
            component.ObjectEnteredInteractionArea -= ObjectEnteredInteractionArea;
            component.ObjectExitedInteractionArea -= ObjectExitedInteractionArea;
            component.ObjectInterruptInteractionArea -= ObjectUnusedInteractionArea;
            component.ObjectUsedInteractionArea -= ObjectUsedInteractionArea;
            component.ObjectFinishedInteractionArea -= ObjectFinishedInteractionArea;
        }
        #endregion

        #region OnEventHandlers
        private void ObjectEnteredInteractionArea(object o, InteractionAreaEventArgs e)
        {
            OnObjectEnteredInteractionArea.Invoke(o, e);
        }

        private void ObjectExitedInteractionArea(object o, InteractionAreaEventArgs e)
        {
            OnObjectExitedInteractionArea.Invoke(o, e);
        }

        private void ObjectUsedInteractionArea(object o, InteractionAreaEventArgs e)
        {
            OnObjectStartInteraction.Invoke(o, e);
        }

        private void ObjectUnusedInteractionArea(object o, InteractionAreaEventArgs e)
        {
            OnObjectInterruptInteraction.Invoke(o, e);
        }

        private void ObjectFinishedInteractionArea(object o, InteractionAreaEventArgs e)
        {
            OnObjectFinishedInteraction.Invoke(o, e);
        }
        #endregion
    }
}