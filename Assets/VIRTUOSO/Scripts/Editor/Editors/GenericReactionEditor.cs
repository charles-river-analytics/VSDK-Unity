using UnityEditor;
using System.Reflection;
using System;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Custom editor for the GenericReaction classes. The main point of the custom editor is to hide
    /// the start/stop reaction variable if the given ReactionEvent has one of it's methods marked
    /// as HideMethodFromInspector.
    /// 
    /// Since GenericReaction is a Generic type, each child implementation needs their own custom editor, 
    /// but can inherit from this one and pass in the genericStringName to reuse these functions.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// </summary>
    public class GenericReactionEditor : Editor
    {
        protected SerializedProperty reactionEvent;
        protected SerializedProperty genericEventSystem;
        protected SerializedProperty startReaction;
        protected SerializedProperty stopReaction;
        protected string genericStringName;

        #region UnityFunctions
        protected virtual void OnEnable()
        {
            reactionEvent = serializedObject.FindProperty("reactionEvent");
            genericEventSystem = serializedObject.FindProperty(genericStringName);
            startReaction = serializedObject.FindProperty("startReaction");
            stopReaction = serializedObject.FindProperty("stopReaction");
        }

        public override void OnInspectorGUI()
        {
            // Reads the latest values on the current (serialized) object (i.e. the one that is selected)
            serializedObject.Update();

            // Display the default Unity prompt for objects in the scene
            EditorGUILayout.PropertyField(reactionEvent);
            EditorGUILayout.PropertyField(genericEventSystem);

            // For displaying the start/stop reaction dropdown menu, look at the given reaction and see if
            // it has the HideMethodFromInspectorAttribute on either of its reaction method
            if (reactionEvent != null)
            {
                object reactionObject = GetObjectFromProperty(reactionEvent);

                if (reactionObject != null)
                {
                    MethodBase startMethod = reactionObject.GetType().GetMethod("StartReaction");
                    var startAttribute = startMethod.GetCustomAttributes(typeof(HideMethodFromInspectorAttribute), true);

                    // If the attribute length is 0, then it was not added to the method and the variable should be displayed
                    if (startAttribute.Length == 0)
                    {
                        EditorGUILayout.PropertyField(startReaction);
                    }

                    // Same for StopReaction
                    MethodBase stopMethod = reactionObject.GetType().GetMethod("StopReaction");
                    var stopAttribute = stopMethod.GetCustomAttributes(typeof(HideMethodFromInspectorAttribute), true);
                    
                    if (stopAttribute.Length == 0)
                    {
                        EditorGUILayout.PropertyField(stopReaction);
                    }
                }
            }

            // Write properties back to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        protected HideMethodFromInspectorAttribute GetHideMethodAttribute(Type givenType)
        {
            // Get instance of the attribute.
            HideMethodFromInspectorAttribute hideMethodAttribute =
                (HideMethodFromInspectorAttribute)Attribute.GetCustomAttribute(givenType, typeof(HideMethodFromInspectorAttribute));

            if (hideMethodAttribute != null)
            {
                return hideMethodAttribute;
            }

            return null;
        }

        protected object GetObjectFromProperty(SerializedProperty property)
        {
            Type objectBaseType = property.serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = objectBaseType.GetField(property.propertyPath);
            return fieldInfo.GetValue(property.serializedObject.targetObject);
        }
    }
}