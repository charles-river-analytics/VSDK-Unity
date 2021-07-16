using System.Collections.Generic;
using UnityEditor;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using UnityEngine;
using System.Reflection;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Custom editor for the Reactor class. Allows the user to provide a component with event and provides feedback
    /// to the user if there are any issues with the provided class. It then displays a list of all the events the
    /// component has and builds a custom array to add any reaction that is currently in the scene.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), December 2018
    /// </summary>
    [CustomEditor(typeof(Reactor))]
    public class ReactorEditor : Editor
    {
        #region ProtectedVariables
        protected SerializedProperty eventSender;
        protected SerializedProperty lastUsedComponent;
        protected Reactor targetReactor;

        protected Dictionary<string, ReactorEditorInfo> reactionsPerEventDictionary;

        protected const string UNDO_MESSAGE = "Undo Reactor Info";
        protected const string NO_EVENTS_WARNING_MESSAGE = "No events were found on the given eventSender. Please provide a different object.";
        protected const string NO_REACTION_WARNING_MESSAGE = "No Reactions are currently in the scene. Please add at least one Reaction to the scene.";
        protected const string PROVIDE_COMPONENT_MESSAGE = "Please provide a component with events.";
        #endregion

        #region EditorHelperMethods
        /// <summary>
        /// Takes the list of EditorInfo (Reaction, Method Selection, etc.) from the target and rebuilds
        /// the dictionary in the editor script.
        /// </summary>
        /// <param name="eventNameArray">String array of events from the given component.</param>
        private void CopyFromTarget(string[] eventNameArray)
        {
            // Grab the dictionary items
            List<ReactorEditorInfo> editorInfoList = targetReactor.GetDictionaryItems();

            // Rebuild the dictionary
            for (int n = 0; n < editorInfoList.Count; n++)
            {
                ReactorEditorInfo currentInfo = new ReactorEditorInfo
                {
                    foldoutOpenStatus = editorInfoList[n].foldoutOpenStatus,
                    reactionList = editorInfoList[n].reactionList,
                    reactionTriggerMethodList = editorInfoList[n].reactionTriggerMethodList,
                    reactionTriggerIndexSelectionList = editorInfoList[n].reactionTriggerIndexSelectionList
                };

                if (reactionsPerEventDictionary.ContainsKey(eventNameArray[n]))
                {
                    reactionsPerEventDictionary[eventNameArray[n]] = currentInfo;
                }
                else
                {
                    reactionsPerEventDictionary.Add(eventNameArray[n], currentInfo);
                }
            }
        }
        #endregion

        #region UnityFunctions        
        protected virtual void OnEnable()
        {
            eventSender = serializedObject.FindProperty("eventSender");
            lastUsedComponent = serializedObject.FindProperty("lastUsedComponent");

            targetReactor = target as Reactor;

            reactionsPerEventDictionary = new Dictionary<string, ReactorEditorInfo>();
        }

        public override void OnInspectorGUI()
        {
            // Reads the latest values on the current (serialized) object (i.e. the one that is selected)
            serializedObject.Update();

            // Display the default Unity prompt for objects in the scene
            EditorGUILayout.PropertyField(eventSender);

            // Array holding the string names of the events on the given object
            string[] eventNameArray;

            // Get all events for the given object
            if (eventSender.objectReferenceValue != null)
            {
                eventNameArray = Utilities.EventUtility.GetEventNameArray(eventSender.objectReferenceValue);

                Component eventSenderComponent = eventSender.objectReferenceValue as Component;

                // See if the dictionary needs to be updated with new events
                if (eventSenderComponent != null && eventSenderComponent != lastUsedComponent.objectReferenceValue)
                {
                    lastUsedComponent.objectReferenceValue = eventSenderComponent;

                    reactionsPerEventDictionary.Clear();
                    targetReactor.Clear();

                    // Create a list for each event from provided event sender
                    for (int n = 0; n < eventNameArray.Length; n++)
                    {
                        ReactorEditorInfo currentInfo = new ReactorEditorInfo
                        {
                            foldoutOpenStatus = false,
                            reactionList = new List<GenericReaction>(),
                            reactionTriggerMethodList = new List<string>(),
                            reactionTriggerIndexSelectionList = new List<int>()
                        };

                        reactionsPerEventDictionary.Add(eventNameArray[n], currentInfo);
                    }

                    // Since the dictionary has been changed, make sure it is updated
                    serializedObject.ApplyModifiedProperties();
                }
                // Otherwise, copy the one back from the target 
                else
                {
                    CopyFromTarget(eventNameArray);
                }

                // Display a warning if there are no events on the component
                if (eventNameArray.Length == 0)
                {
                    EditorGUILayout.HelpBox(NO_EVENTS_WARNING_MESSAGE, MessageType.Error);
                }
                // Otherwise, display the event list and allow the user to hook up reactions to the events
                else
                {
                    // Display every event that the event sender has
                    for (int n = 0; n < eventNameArray.Length; n++)
                    {
                        // Delta to minimize/maximize the specific event
                        reactionsPerEventDictionary[eventNameArray[n]].foldoutOpenStatus = EditorGUILayout.Foldout(reactionsPerEventDictionary[eventNameArray[n]].foldoutOpenStatus, eventNameArray[n]);

                        // Show the psuedo array if the user has hit the delta
                        if (reactionsPerEventDictionary[eventNameArray[n]].foldoutOpenStatus)
                        {
                            // Display a prompt for a reaction based on the current event in the given component 
                            for (int h = 0; h < reactionsPerEventDictionary[eventNameArray[n]].reactionList.Count; h++)
                            {
                                EditorGUILayout.BeginHorizontal();

                                // Prompt for the Reaction
                                reactionsPerEventDictionary[eventNameArray[n]].reactionList[h] = (GenericReaction)EditorGUILayout.ObjectField(reactionsPerEventDictionary[eventNameArray[n]].reactionList[h], typeof(GenericReaction), true);

                                // Since some reactions use the HideMethod attribute, check here to see if they should be displayed
                                List<string> reactionTriggers = new List<string>();

                                if (reactionsPerEventDictionary[eventNameArray[n]].reactionList[h] != null)
                                {
                                    foreach (string methodName in Utilities.Constants.reactionTriggerMethods)
                                    {
                                        MethodBase currentMethod = reactionsPerEventDictionary[eventNameArray[n]].reactionList[h].GetType().GetMethod(methodName);

                                        var currentAttribute = currentMethod.GetCustomAttributes(typeof(HideMethodFromInspectorAttribute), true);

                                        // If the attribute length is 0, then it was not added to the method and the variable should be displayed
                                        if (currentAttribute.Length == 0)
                                        {
                                            reactionTriggers.Add(methodName);
                                        }
                                    }
                                }
                                else
                                {
                                    reactionTriggers.Add("");
                                }

                                // Reaction method prompt (index selection)
                                reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerIndexSelectionList[h] = EditorGUILayout.Popup(reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerIndexSelectionList[h], reactionTriggers.ToArray());

                                // Store method name with the editor info
                                reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerMethodList[h] = reactionTriggers[reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerIndexSelectionList[h]];

                                // Add a button to remove this from the list
                                if (GUILayout.Button("-"))
                                {
                                    reactionsPerEventDictionary[eventNameArray[n]].RemoveElementsAt(h);
                                }

                                EditorGUILayout.EndHorizontal();
                            }

                            // Add a button to allow adding more reactions to the list
                            if (GUILayout.Button("+"))
                            {
                                // Add null values to the lists so that index exists in the list and can be autopopulated by the editor (until changed by user)
                                reactionsPerEventDictionary[eventNameArray[n]].reactionList.Add(null);
                                reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerMethodList.Add(null);
                                reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerIndexSelectionList.Add(0);
                            }
                        }
                    }
                }

                // Copy the dictionary made in the Editor to the GameObject
                targetReactor.Clear();

                for (int n = 0; n < eventNameArray.Length; n++)
                {
                    targetReactor.AddDictionaryKey(eventNameArray[n],
                                                    reactionsPerEventDictionary[eventNameArray[n]].foldoutOpenStatus,
                                                    reactionsPerEventDictionary[eventNameArray[n]].reactionList,
                                                    reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerMethodList,
                                                    reactionsPerEventDictionary[eventNameArray[n]].reactionTriggerIndexSelectionList);
                }
            }
            // If no component is given, then all the info should be cleared
            else
            {
                EditorGUILayout.HelpBox(PROVIDE_COMPONENT_MESSAGE, MessageType.None);

                // Reset the dictionary and associated information
                reactionsPerEventDictionary.Clear();
                targetReactor.Clear();

                lastUsedComponent.objectReferenceValue = null;
            }

            // Write properties back to the serialized object
            serializedObject.ApplyModifiedProperties();

            Undo.RecordObject(targetReactor, UNDO_MESSAGE);
        }
        #endregion
    }
}