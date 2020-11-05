using CharlesRiverAnalytics.Virtuoso.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// TODO
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    [CustomPropertyDrawer(typeof(EventBusHierarchy))]
    public class EventBusHierarchyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty hierarchyValue = property.FindPropertyRelative("hierarchyValue");

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            hierarchyValue.stringValue = EditorGUI.DelayedTextField(position, hierarchyValue.stringValue);

            EditorGUI.EndProperty();

            // Update the EventBus class on this known hierarchies
            EventBus.Instance.AddKnownObject(hierarchyValue.stringValue);
        }
    }
}