using CharlesRiverAnalytics.Virtuoso.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// The EventBusSender class is a component that allows all the public events on all other components
    /// attached to the same GameObject to be sent to the EventBus system for the reaction system. To use,
    /// simply attach to the GameObject with events you want to fire.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    [DisallowMultipleComponent]
    public class EventBusSender : MonoBehaviour
    {
        #region ProtectedVariables

        protected List<DynamicDelegate> dynamicEventList;
        protected List<Component> componentsWithEvents;

        #endregion

        #region UnityFunctions

        protected void Awake()
        {
            dynamicEventList = new List<DynamicDelegate>();
            componentsWithEvents = new List<Component>();

            var allComponents = GetComponentsInChildren<Component>(true);

            foreach (var currentComponent in allComponents)
            {
                string[] eventNames = EventUtility.GetEventNameArray(currentComponent);

                if (eventNames.Length > 0)
                {
                    componentsWithEvents.Add(currentComponent);
                }
            }
        }

        protected void OnEnable()
        {
            if (componentsWithEvents.Count > 0)
            {
                foreach (Component currentEventSender in componentsWithEvents)
                {
                    // Subscribe to all of the event sender events so you can foward them to the EventBus
                    string[] eventNames = EventUtility.GetEventNameArray(currentEventSender);

                    for (int i = 0; i < eventNames.Length; i++)
                    {
                        string currentEventName = eventNames[i];

                        dynamicEventList.Add(EventUtility.SubscribeToEvent(
                            this,
                            currentEventSender,
                            eventNames[i],
                            (sender, args) =>
                            {
                                EventBusHierarchy eventBusHierarchy = currentEventSender.GetComponent<EventBusHierarchy>();

                                EventBus.Instance.ForwardEvent(currentEventName, eventBusHierarchy, sender, args);
                            },
                            currentEventSender.GetType()));
                    }
                }
            }
        }

        protected void OnDisable()
        {
            // Unregister from all dynamic events
            EventUtility.UnsubscribeFromAllEvents(dynamicEventList);

            dynamicEventList.Clear();
        }

        #endregion
    }
}