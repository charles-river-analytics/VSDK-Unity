using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using CharlesRiverAnalytics.Virtuoso.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso
{
    public delegate void TimelineEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Timeline events are used for propogating events happening for the entire scenario. This would include
    /// the background events of the scenario. Events can be held off from firing by filling in the
    /// dependencies for either other Timeline events, by Interaction Areas, gesture events, or anything
    /// that inherits the VirtuosoEvent.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class TimelineEvent : VirtuosoEvent
    {
        #region PublicVariables
        [Tooltip("The time (in ms) the timeline event will wait before firing the reactions.")]
        public int startDelay = 0;
        [Tooltip("The list of other timeline events, interaction areas, and gesture events that can constrain this event.")]
        public List<TimelineEventSelection> dependencyList;
        [Tooltip("The list of reactions that will fire when all dependencies have been met.")]
        public List<GenericReaction> reactionList;
        #endregion

        #region ProtectedVariables
        protected HashSet<VirtuosoEvent> awaitingEvents;
        protected List<DynamicDelegate> dynamicDelegateList;
        #endregion

        #region EventVariables
        // Emitted when the timeline event finishes
        public event TimelineEventHandler TimelineEventFinish;

        protected virtual void OnTimelineEventFinish(EventArgs timelineArgs)
        {
            if (TimelineEventFinish != null)
            {
                TimelineEventFinish(this, timelineArgs);
            }
        }
        #endregion

        #region UnityFunctions
        protected void Awake()
        {
            awaitingEvents = new HashSet<VirtuosoEvent>();
            dynamicDelegateList = new List<DynamicDelegate>();
        }

        protected void OnEnable()
        {            
            for (int n = 0; n < dependencyList.Count; n++)
            {
                if (dependencyList[n].providedEvent is TimelineEvent)
                {
                    TimelineEvent currentEvent = dependencyList[n].providedEvent as TimelineEvent;

                    awaitingEvents.Add(currentEvent);

                    dynamicDelegateList.Add(EventUtility.SubscribeToEvent(this, 
                                                                          currentEvent,
                                                                          dependencyList[n].eventToListen,
                                                                          "OnDependentTimeline", 
                                                                          null,
                                                                          GetType(),
                                                                          BindingFlags.Instance | BindingFlags.NonPublic));
                }
                else if(dependencyList[n].providedEvent is InteractionArea)
                {
                    InteractionArea currentEvent = dependencyList[n].providedEvent as InteractionArea;

                    awaitingEvents.Add(currentEvent);
                    
                    dynamicDelegateList.Add(EventUtility.SubscribeToEvent(this, 
                                                                          currentEvent, 
                                                                          dependencyList[n].eventToListen, 
                                                                          "OnDependentInteractionArea", 
                                                                          null,
                                                                          GetType(), 
                                                                          BindingFlags.Instance | BindingFlags.NonPublic));
                }
                else if(dependencyList[n].providedEvent is GestureInteraction)
                {
                    GestureInteraction currentEvent = dependencyList[n].providedEvent as GestureInteraction;

                    awaitingEvents.Add(currentEvent);

                    dynamicDelegateList.Add(EventUtility.SubscribeToEvent(this, 
                                                                          currentEvent, 
                                                                          dependencyList[n].eventToListen, 
                                                                          "OnDependentTimeline",
                                                                          null,
                                                                          GetType(),
                                                                          BindingFlags.Instance | BindingFlags.NonPublic));
                }
            }

            // If there are no dependencies, you start at the scene's start
            CheckDependencyCount();
        }

        protected void OnDisable()
        {
            for(int n = 0; n < dynamicDelegateList.Count; n++)
            {
                dynamicDelegateList[n].UnsubscribeEvent();
            }
        }
        #endregion

        #region EventHandling
        protected void OnDependentTimeline(object sender, EventArgs e)
        {
            awaitingEvents.Remove(sender as TimelineEvent);

            CheckDependencyCount();
        }

        protected void OnDependentInteractionArea(object sender, InteractionAreaEventArgs e)
        {
            awaitingEvents.Remove(sender as InteractionArea);

            CheckDependencyCount();
        }

        // TODO Add back with gestures [VIRTUOSO-169]
        /*protected void OnDependentGesture(object sender, GestureEventArgs e)
        {
            awaitingEvents.Remove(sender as InteractGesture);

            CheckDependencyCount();
        }*/
        #endregion

        #region HelperFunctions
        /// <summary>
        /// Helper function to check the size of the awaitingEvent set. Since objects are removed from the set
        /// after they are fired, when the set is empty, the timeline event has no more dependencies and 
        /// can now fire off the finish event.
        /// </summary>
        protected void CheckDependencyCount()
        {
            if(awaitingEvents.Count == 0)
            {
                StartCoroutine(FireEvent());
            }
        }
        #endregion

        #region Coroutines
        protected IEnumerator FireEvent()
        {
            yield return new WaitForSeconds(startDelay / Constants.MS_TO_SECONDS);
            
            for (int n = 0; n < reactionList.Count; n++)
            {
                if(reactionList[n] != null)
                {
                    // TODO Fix connection with reactions [VIRTUOSO-180]
                    //reactionList[n].StartReaction();
                }
            }

            OnTimelineEventFinish(new EventArgs());

            // Disable self as it is no longer needed
            enabled = false;
        }
        #endregion
    }
}