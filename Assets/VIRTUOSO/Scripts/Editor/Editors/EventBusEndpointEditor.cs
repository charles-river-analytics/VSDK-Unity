using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// Styles the EventBusEndpoint component to use a reorderable list for the reaction list.
    /// 
    /// TODO Make the hierarchy and event inputs become dropdown menus instead of strings
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    [CustomEditor(typeof(EventBusEndpoint))]
    public class EventBusEndpointEditor : Editor
    {
        private SerializedProperty hierarchyString;
        private SerializedProperty eventString;
        private SerializedProperty reactionList;
        private ReorderableList reorderableReactionList;

        void OnEnable()
        {
            hierarchyString = serializedObject.FindProperty("hierarchyString");
            eventString = serializedObject.FindProperty("eventString");
            reactionList = serializedObject.FindProperty("reactionList");

            reorderableReactionList = new ReorderableList(serializedObject, reactionList, true, true, true, true);

            // Customize the reorderable list
            reorderableReactionList.elementHeight = EditorGUIUtility.singleLineHeight * 2.5f;

            reorderableReactionList.drawHeaderCallback = (Rect rect) => 
            {
                EditorGUI.LabelField(rect, "Reaction List");
            };

            reorderableReactionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                var element = reorderableReactionList.serializedProperty.GetArrayElementAtIndex(index);
                
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("reaction"), GUIContent.none);

                EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fireMethod"), GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            hierarchyString.stringValue = EditorGUILayout.TextField("Hierarchy", hierarchyString.stringValue);
            eventString.stringValue = EditorGUILayout.TextField("Event", eventString.stringValue);

            reorderableReactionList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}