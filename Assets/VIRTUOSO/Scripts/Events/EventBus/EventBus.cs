using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharlesRiverAnalytics.Virtuoso.Utilities;
using System.Text.RegularExpressions;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// The singleton class that manages the mapping between all the EventBus endpoints.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    public class EventBus : Singleton<EventBus>
    {
        #region ProtectedVariables

        protected Dictionary<ObjectEventMapping, HashSet<EventBusEndpoint>> endPointsWaitingOnEvents;
        protected Tree<string> hierarchyTree = new Tree<string>("/");

        #endregion

        #region PrivateVariables

        private Regex alphaNumericCharactersOnly = new Regex("^[a-zA-Z0-9 ]*$");

        #endregion

        protected struct ObjectEventMapping
        {
            public string hierarchyString;
            public string eventString;
        }

        #region PublicAPI

        public void ForwardEvent(string eventName, EventBusHierarchy eventObjectHierarchy, object sender, EventArgs args)
        {
            // Since the eventObjectHierarchy is a tree structure, we must check every combination of the built hierarchy
            foreach(string currentHierarchyString in eventObjectHierarchy.GetHierarchyCombination())
            {
                ObjectEventMapping checkKey;
                checkKey.hierarchyString = currentHierarchyString;
                checkKey.eventString = eventName;

                if (endPointsWaitingOnEvents.ContainsKey(checkKey))
                {
                    foreach (EventBusEndpoint endpoint in endPointsWaitingOnEvents[checkKey])
                    {
                        endpoint.FireReactions(sender, args);
                    }
                }
            }
        }

        public void SubscribeToEvent(EventBusEndpoint endpoint, string hierarchyString, string eventString)
        {
            ObjectEventMapping newKey;
            newKey.hierarchyString = hierarchyString;
            newKey.eventString = eventString;

            // The object/event pairing already exists, add the endpoint to the hashset for it
            if (endPointsWaitingOnEvents.ContainsKey(newKey))
            {
                endPointsWaitingOnEvents[newKey].Add(endpoint);
            }
            // The object/event pairing is new, add to the dictionary and create a hashset with this endpoint to start
            else
            {
                endPointsWaitingOnEvents[newKey] = new HashSet<EventBusEndpoint>() { endpoint };
            }
        }

        public void UnsubscribeToEvent(EventBusEndpoint endpoint, string hierarchyString, string eventString)
        {
            ObjectEventMapping newKey;
            newKey.hierarchyString = hierarchyString;
            newKey.eventString = eventString;

            // Can only unsubscribe from known object/event pairings with known endpoints
            if (endPointsWaitingOnEvents.ContainsKey(newKey) &&
                endPointsWaitingOnEvents[newKey].Contains(endpoint))
            {
                endPointsWaitingOnEvents[newKey].Remove(endpoint);
            }
        }

        public void AddKnownObject(EventBusHierarchy hierarchyValue)
        {
            var splitHierarchy = hierarchyValue.GetSplitHierarchy();

            Tree<string> currentNode;

            for (int i = 0; i < splitHierarchy.Length; i++)
            {
                currentNode = hierarchyTree.InChildNode(splitHierarchy[i]);

                // The node doesn't exist, so it needs to be added
                if (currentNode == null)
                {
                    // Only allow alphanumeric characters to define the hierarchy
                    if (alphaNumericCharactersOnly.IsMatch(splitHierarchy[i]))
                    {
                        currentNode = hierarchyTree.AddChild(splitHierarchy[i]);
                    }
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
            endPointsWaitingOnEvents = new Dictionary<ObjectEventMapping, HashSet<EventBusEndpoint>>();
        }

        #endregion
    }
}