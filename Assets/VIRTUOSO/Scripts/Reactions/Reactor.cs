using System;
using System.Collections.Generic;
using UnityEngine;
using CharlesRiverAnalytics.Virtuoso.Utilities;
using System.Reflection;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Allows for hooking up any event system to the reactions provided in VIRTUOSO. Pass this class a component with
    /// events and select the reactions for each event in the provided class. The events signature in the class must follow 
    /// the standard .Net event pattern (https://docs.microsoft.com/en-us/dotnet/csharp/event-pattern) in order to work.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), December 2018
    /// </summary>
    [Serializable]
    public class Reactor : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region PublicVariables
        [Tooltip("The component that will send out events that will trigger the reactions to fire.")]
        public Component eventSender;
        [HideInInspector]
        public Component lastUsedComponent;
        [HideInInspector]
        public Dictionary<string, ReactorEditorInfo> reactionsPerEventDictionary = new Dictionary<string, ReactorEditorInfo>();
        #endregion

        #region ProtectedVariables
        protected List<DynamicDelegate> dynamicEventList;
        // These guys are used to temporarily hold the dictionary keys/values to help serialize the dictionary
        [SerializeField]
        protected List<string> _keyList = new List<string>();
        [SerializeField]
        protected List<ReactorEditorInfo> _valueList = new List<ReactorEditorInfo>();
        #endregion

        #region UnityFunctions
        public void Awake()
        {
            dynamicEventList = new List<DynamicDelegate>();

            // Go through every event and hook into the given reactions
            if(eventSender != null)
            {
                foreach(string key in reactionsPerEventDictionary.Keys)
                {
                    for(int n = 0; n < reactionsPerEventDictionary[key].reactionList.Count; n++)
                    {
                        // Skip any null reactions
                        if(reactionsPerEventDictionary[key].reactionList[n] != null)
                        {
                            dynamicEventList.Add(EventUtility.SubscribeToEvent(reactionsPerEventDictionary[key].reactionList[n],
                                                                                eventSender,
                                                                                key,
                                                                                reactionsPerEventDictionary[key].reactionTriggerMethodList[n],
                                                                                eventSender.GetType(),
                                                                                reactionsPerEventDictionary[key].reactionList[n].GetType(),
                                                                                BindingFlags.Instance | BindingFlags.Public));
                        }
                    }
                }
            }
        }

        protected void OnDisable()
        {
            // Unregister from all dynamic events
            EventUtility.UnsubscribeFromAllEvents(dynamicEventList);
        }
        #endregion

        #region EditorHelperMethods
        public void AddDictionaryKey(string key, bool foldout, List<GenericReaction> reactions, List<string> triggerList, List<int> triggerIndexSelection/*, List<int> reactionIndexSelection*/)
        {
            ReactorEditorInfo currentValue = new ReactorEditorInfo()
            {
                foldoutOpenStatus = foldout,
                reactionList = reactions,
                reactionTriggerMethodList = triggerList,
                reactionTriggerIndexSelectionList = triggerIndexSelection
            };

            if(reactionsPerEventDictionary.ContainsKey(key))
            {
                reactionsPerEventDictionary[key] = currentValue;
            }
            else
            {
                reactionsPerEventDictionary.Add(key, currentValue);
            }
        }

        public void Clear()
        {
            reactionsPerEventDictionary.Clear();
        }

        public List<ReactorEditorInfo> GetDictionaryItems()
        {
            List<ReactorEditorInfo> builtEditorList = new List<ReactorEditorInfo>();

            foreach (string key in reactionsPerEventDictionary.Keys)
            {
                ReactorEditorInfo currentEditorInfo = new ReactorEditorInfo
                {
                    foldoutOpenStatus = reactionsPerEventDictionary[key].foldoutOpenStatus,
                    reactionList = reactionsPerEventDictionary[key].reactionList,
                    reactionTriggerMethodList = reactionsPerEventDictionary[key].reactionTriggerMethodList,
                    reactionTriggerIndexSelectionList = reactionsPerEventDictionary[key].reactionTriggerIndexSelectionList
                };

                builtEditorList.Add(currentEditorInfo);
            }

            return builtEditorList;
        }
        #endregion

        #region ISerializationCallbackReceiverImplementation
        // Implementation provided by Unity: https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.html
        public void OnBeforeSerialize()
        {
            _keyList.Clear();
            _valueList.Clear();

            foreach (var kvp in reactionsPerEventDictionary)
            {
                _keyList.Add(kvp.Key);
                _valueList.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                if(reactionsPerEventDictionary.ContainsKey(_keyList[i]))
                {
                    reactionsPerEventDictionary[_keyList[i]] = _valueList[i];
                }
                else
                {
                    reactionsPerEventDictionary.Add(_keyList[i], _valueList[i]);
                }
            }
        }
        #endregion
    }
}