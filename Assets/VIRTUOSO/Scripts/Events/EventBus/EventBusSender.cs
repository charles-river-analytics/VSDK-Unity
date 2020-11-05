using CharlesRiverAnalytics.Virtuoso.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// TODO
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    [DisallowMultipleComponent]
    public class EventBusSender : MonoBehaviour
    {
        #region PublicVariables

        //[Tooltip("The component that will send out events that will trigger the reactions to fire.")]
        //public Component eventSender;

        #endregion

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

            foreach(var currentComponent in allComponents)
            {
                string[] eventNames = EventUtility.GetEventNameArray(currentComponent);

                if(eventNames.Length > 0)
                {
                    Debug.Log(currentComponent + " on " + name + " has public events.");
                    componentsWithEvents.Add(currentComponent);
                }
            }
        }

        protected void OnEnable()
        {
            if (componentsWithEvents.Count > 0)
            {
                foreach(Component currentEventSender in componentsWithEvents)
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
                            // Get instance of the Hierarchy
                            string hierarchString = "";

                                foreach (FieldInfo property in currentEventSender.GetType().GetFields())
                                {
                                    if (property.FieldType == typeof(EventBusHierarchy))
                                    {
                                        hierarchString = ((EventBusHierarchy)property.GetValue(currentEventSender)).hierarchyValue;

                                        break;
                                    }
                                }

                            // The hierarchy wasn't established on the event sender, check the Gameobject for the mixin class
                            if (string.IsNullOrEmpty(hierarchString))
                                {
                                    var t = currentEventSender.GetComponent<EventBusMixin>();

                                    if (t != null)
                                    {
                                        hierarchString = t.objectHierarchy.hierarchyValue;
                                    }
                                }

                                EventBus.Instance.ForwardEvent(currentEventName, hierarchString, sender, args);
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