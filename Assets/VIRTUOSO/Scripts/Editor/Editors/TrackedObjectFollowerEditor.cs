using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Allows the user to select a tracker from the tracker list available to an SDK. First, they must select
    /// the SDK they want to pull the tracker from and then select the tracker from the tracker list. If the
    /// SDK doesn't have the tracker in its own list, then it will not appear here.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), October 2018
    /// </summary>
    [CustomEditor(typeof(TrackedObjectFollower))]
    public class TrackedObjectFollowerEditor : Editor
    {
        #region SerializedProperties
        SerializedProperty sdkIndex;
        SerializedProperty trackerIndex;
        #endregion

        #region PrivateVariables
        private VRTK_SDKSetup[] sdkArray;
        private List<string> sdkNameList;
        #endregion

        #region UnityEditorFunctions
        public void Awake()
        {
            sdkArray = VRTK_SharedMethods.FindEvenInactiveComponents<VRTK_SDKSetup>();
            sdkNameList = new List<string>();

            foreach (VRTK_SDKSetup currentSDK in sdkArray)
            {
                sdkNameList.Add(currentSDK.name);
            }

            sdkIndex = serializedObject.FindProperty("sdkSelectionIndex");
            trackerIndex = serializedObject.FindProperty("trackerSelectionIndex");
        }

        public override void OnInspectorGUI()
        {
            // Read any updates on the serialized object
            serializedObject.Update();

            // Prompt the user for the SDK selection
            sdkIndex.intValue = EditorGUILayout.Popup(sdkIndex.intValue, sdkNameList.ToArray());
            
            VRTK_SDKSetup selectedSDK = sdkArray[sdkIndex.intValue];
            List<string> trackerNames = new List<string>();

            // Turn the tracker list into an array of string names
            foreach(GameObject currentTracker in selectedSDK.actualTrackers)
            {
                trackerNames.Add(currentTracker.name);
            }

            // Prompt user for the tracker selection
            trackerIndex.intValue = EditorGUILayout.Popup(trackerIndex.intValue, trackerNames.ToArray());

            // Save changes made to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}