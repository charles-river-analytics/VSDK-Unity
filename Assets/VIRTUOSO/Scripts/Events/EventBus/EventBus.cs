using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharlesRiverAnalytics.Virtuoso.Utilities;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// TODO
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    public class EventBus : Singleton<EventBus>
    {
        #region ProtectedVariables

        protected Dictionary<DictKey, HashSet<EventBusEndpoint>> endPointsWaitingOnEvents;
        protected Tree<string> hierarchyTree = new Tree<string>("/");

        #endregion

        protected struct DictKey
        {
            public string hierarchyString;
            public string eventString;
        }

        #region PublicAPI

        public void ForwardEvent(string eventName, string eventObjectHierarchy, object sender, EventArgs args)
        {
            // TODO
            Debug.Log("Event: " + eventName + " from " + eventObjectHierarchy + " of " + sender);

            DictKey checkKey;
            checkKey.hierarchyString = eventObjectHierarchy;
            checkKey.eventString = eventName;

            if (endPointsWaitingOnEvents.ContainsKey(checkKey))
            {
                foreach (EventBusEndpoint endpoint in endPointsWaitingOnEvents[checkKey])
                {
                    endpoint.FireReactions(sender, args);
                }
            }
        }

        public void SubscribeToEvent(EventBusEndpoint endpoint, string hierarchyString, string eventString)
        {
            DictKey newKey;
            newKey.hierarchyString = hierarchyString;
            newKey.eventString = eventString;

            if (endPointsWaitingOnEvents.ContainsKey(newKey))
            {
                endPointsWaitingOnEvents[newKey].Add(endpoint);
            }
            else
            {
                endPointsWaitingOnEvents[newKey] = new HashSet<EventBusEndpoint>() { endpoint };
            }
        }

        public void AddKnownObject(string hierarchyValue)
        {
            Debug.Log("New hierarchy: " + hierarchyValue);
            var splitHierarchy = hierarchyValue.Split('/');
            Debug.Log("Split: " + splitHierarchy.Length);

            Tree<string> currentNode;

            for (int i = 0; i < splitHierarchy.Length; i++)
            {
                currentNode = hierarchyTree.InChildNode(splitHierarchy[i]);

                // The node doesn't exist, so it needs to be added
                if (currentNode == null)
                {
                    currentNode = hierarchyTree.AddChild(splitHierarchy[i]);
                }
                // Else, it already exists, vist the children nodes in the next loop for the next hierarchy string
            }
        }

        [ContextMenu("Print Tree")]
        public void PrintTree()
        {
            hierarchyTree.Traverse((x) =>
            {
                Debug.Log(x);
            });
        }

        #endregion

        #region UnityFunctions

        protected void Awake()
        {
            endPointsWaitingOnEvents = new Dictionary<DictKey, HashSet<EventBusEndpoint>>();
        }

        #endregion
    }
}