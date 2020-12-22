using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// A utility class that helps with managing events at runtime.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// </summary>
    public static class EventUtility
    {
        /// <summary>
        /// Allows for the subscription of event when the event is not known ahead of time and must be
        /// identified as a string.
        /// </summary>
        /// <param name="listener">The instance of type T that will listen for the event.</param>
        /// <param name="eventSpeaker">The instance of type Y that will send out the event.</param>
        /// <param name="eventName">The string representing the name of the event.</param>
        /// <param name="methodHandleName">The string representing the name of the method to call.</param>
        /// <param name="overrideEventSpeakerType">Nullable, if provided will use the provided type to find the events of the event speaker.</param>
        /// <param name="classWithMethod">Nullable, if provided will the class type to find the method to call.</param>
        /// <param name="methodBindingFlags">Flags for finding a method. Defaults to finding Public, Instance methods.</param>
        /// <returns>The information needed to unsubscribe to the event.</returns>
        public static DynamicDelegate SubscribeToEvent<T, Y>(T listener, Y eventSpeaker, string eventName, string methodHandleName, Type overrideEventSpeakerType = null, Type classWithMethod = null, BindingFlags methodBindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            EventInfo eventInfo;
            MethodInfo eventHandleMethod;

            if (overrideEventSpeakerType == null)
            {
                eventInfo = typeof(Y).GetEvent(eventName);
            }
            else
            {
                eventInfo = overrideEventSpeakerType.GetEvent(eventName);
            }

            if(classWithMethod == null)
            {
                eventHandleMethod = typeof(T).GetMethod(methodHandleName, methodBindingFlags);
            }
            else
            {                
                eventHandleMethod = classWithMethod.GetMethod(methodHandleName, methodBindingFlags);
            }

            if (eventInfo != null && eventInfo.EventHandlerType != null)
            {
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, listener, eventHandleMethod);

                if (handler != null)
                {
                    eventInfo.AddEventHandler(eventSpeaker, handler);

                    return new DynamicDelegate(eventInfo, eventSpeaker, handler);
                }
                else
                {
                    Debug.LogError("Could not create delegate for event " + eventName + " with method " + methodHandleName);
                }
            }

            return null;
        }

        /// <summary>
        /// Allows for the subscription of event when the event is not known ahead of time and must be
        /// identified as a string with a lambda function used as the event delegate.
        /// </summary>
        /// <param name="listener">The instance of type T that will listen for the event.</param>
        /// <param name="eventSpeaker">The instance of type Y that will send out the event.</param>
        /// <param name="eventName">The string representing the name of the event.</param>
        /// <param name="callback">The lambda expression that will act as the delegate.</param>
        /// <param name="overrideEventSpeakerType">Null by default, if provided will use the type to find the events of the event speaker.</param>
        /// <returns></returns>
        public static DynamicDelegate SubscribeToEvent<T, Y>(T listener, Y eventSpeaker, string eventName, Action<object, EventArgs> callback, Type overrideEventSpeakerType = null)
        {
            EventInfo eventInfo;

            if (overrideEventSpeakerType == null)
            {
                eventInfo = typeof(Y).GetEvent(eventName);
            }
            else
            {
                eventInfo = overrideEventSpeakerType.GetEvent(eventName);
            }

            if (eventInfo != null && eventInfo.EventHandlerType != null)
            {
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, callback.Target, callback.Method);

                if (handler != null)
                {
                    eventInfo.AddEventHandler(eventSpeaker, handler);

                    return new DynamicDelegate(eventInfo, eventSpeaker, handler);
                }
                else
                {
                    Debug.LogError("Could not create delegate for event " + eventName + " with lambda.");
                }
            }
            
            return null;
        }

        /// <summary>
        /// Allows for the subscription of event when the event is not known ahead of time and must be
        /// identified as a string. Use this method when the eventSpeaker is inherited from a class and the
        /// pass variable is not of the derived class.
        /// 
        /// </summary>
        /// <param name="listener">The instance of type T that will listen for the event.</param>
        /// <param name="eventSpeaker">The instance of type Y that will send out the event.</param>
        /// <param name="eventName">The string representing the name of the event.</param>
        /// <param name="eventHandler">The string representing the name of the method.</param>
        /// <returns>The information needed to unsubscribe to the event.</returns>
        public static DynamicDelegate SubscribeToEvent<T, Y>(T listener, Y eventSpeaker, Type eventType, string eventName, string methodHandleName)
        {
            EventInfo eventInfo = eventType.GetEvent(eventName);
            MethodInfo eventHandleMethod = typeof(T).GetMethod(methodHandleName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (eventInfo != null && eventInfo.EventHandlerType != null)
            {
                //Delegate handler = Delegate.CreateDelegate(handlerType, listener, eventHandler, false, false);
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, listener, eventHandleMethod);

                if (handler != null)
                {
                    eventInfo.AddEventHandler(eventSpeaker, handler);

                    return new DynamicDelegate(eventInfo, eventSpeaker, handler);
                }
                else
                {
                    Debug.LogError("Could not create delegate for event " + eventName + " with method " + methodHandleName);
                }
            }

            return null;
        }

        /// <summary>
        /// Helper method to unsubscribe from all of a class' DynamicDelegates.
        /// </summary>
        /// <param name="delegateList">List of DynamicDelegates</param>
        public static void UnsubscribeFromAllEvents(List<DynamicDelegate> delegateList)
        {
            for (int n = 0; n < delegateList.Count; n++)
            {
                if (delegateList[n] != null)
                {
                    delegateList[n].UnsubscribeEvent();
                }
            }
        }

        /// <summary>
        /// Uses Reflection to provide the list of strings of the event names of the provided class.
        /// </summary>
        /// <param name="givenObject">The object you want to pull the events from.</param>
        /// <param name="addPlaceHolder">When set to true, will add 'None' to the front of the array.</param>
        /// <returns>The array of string event names</returns>
        public static string[] GetEventNameArray<T>(T givenObject, bool addPlaceHolder = false)
        {
            return GetEventList(givenObject.GetType(), addPlaceHolder);
        }

        public static string[] GetEventList(Type givenType, bool addPlaceHolder = false)
        {
            EventInfo[] eventInfoList = givenType.GetEvents();

            string[] tempEventList = eventInfoList.Select(x => x.Name).ToArray();

            if (addPlaceHolder)
            {
                // Add 'None' to the list of options so it defaults to it
                string[] eventList = new string[tempEventList.Length + 1];
                eventList[0] = "None";
                Array.Copy(tempEventList, 0, eventList, 1, tempEventList.Length);

                return eventList;
            }
            else
            {
                return tempEventList;
            }
        }
    }

    /// <summary>
    /// Custom structure to hold data about any event delegate that is created at run time. In order to unsubscribe from
    /// a dynamically created event, the reference to the the event, the target it was applied to, and the delegate 
    /// itself is needed. This class helps by holding all those references and stored after it is created so it can be 
    /// unsubscribed to at the end of the game's life.
    /// </summary>
    public class DynamicDelegate
    {
        public EventInfo eventInfo;
        public object target;
        public Delegate methodHandler;

        public DynamicDelegate(EventInfo givenEventInfo, object givenTarget, Delegate givenMethodHandler)
        {
            eventInfo = givenEventInfo;
            target = givenTarget;
            methodHandler = givenMethodHandler;
        }

        public void UnsubscribeEvent()
        {
            eventInfo.RemoveEventHandler(target, methodHandler);
        }
    }
}