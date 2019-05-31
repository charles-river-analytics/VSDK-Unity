using UnityEngine;
using UnityEditor;
using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using System;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Custom property drawer to help the user to select the proper VIRTUOSO event and based on the provided class, 
    /// the proper event methods available to that class for propogating the timeline events.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// </summary>
    [CustomPropertyDrawer(typeof(TimelineEventSelection))]
    public class TimelineEventSelectionDrawer : PropertyDrawer
    {
        #region PrivateVariables
        // Number of boxes that will be made to fit on one line in the editor
        private const float propertiesPerLine = 2.0f;
        #endregion

        #region UnityFunctions
        /// <summary>
        /// Draws the custom property drawer for displaying a timeline event selection, which includes the VirtuosoEvent and the event to listen to
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.indentLevel = 0;

            // Fit 2 properties on one line
            position.width /= propertiesPerLine;

            // Grab the providedEvent in the Selection object to find out what kind of enum to display
            SerializedProperty providedEvent = property.FindPropertyRelative("providedEvent");
            EditorGUI.ObjectField(position, providedEvent, typeof(VirtuosoEvent), GUIContent.none);

            // Move the next field over horizontally
            position.x += position.width;

            SerializedProperty selectedEventName = property.FindPropertyRelative("eventToListen");
            SerializedProperty selectedPopupValue = property.FindPropertyRelative("selectedPopupValue");

            // Grab the actual passed in class to display the proper enum
            VirtuosoEvent virtuosoEvent = (VirtuosoEvent)providedEvent.objectReferenceValue;

            if(virtuosoEvent is TimelineEvent)
            {
                DisplayEventList(position, virtuosoEvent as TimelineEvent, selectedPopupValue, selectedEventName);
            }
            else if(virtuosoEvent is InteractionArea)
            {
                DisplayEventList(position, virtuosoEvent as InteractionArea, selectedPopupValue, selectedEventName);
            }
            else if(virtuosoEvent is GestureInteraction)
            {
                DisplayEventList(position, virtuosoEvent as GestureInteraction, selectedPopupValue, selectedEventName);
            }
            else
            {
                EditorGUI.LabelField(position, "Provide VIRTUOSO Event");
            }

            EditorGUI.EndProperty();
        }
        #endregion

        #region HelperFunctions
        // Displays a popup box of event names for a given object
        protected void DisplayEventList<T>(Rect position, T givenEvent, SerializedProperty popupIntSelection, SerializedProperty eventString)
        {
            var eventNames = Utilities.EventUtility.GetEventNameArray(givenEvent);

            popupIntSelection.intValue = EditorGUI.Popup(position, popupIntSelection.intValue, eventNames);

            eventString.stringValue = eventNames[popupIntSelection.intValue];
        }
        #endregion
    }
}