using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
#if VRTK_DEFINE_SDK_STEAMVR
using Valve.VR;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Helper struct to hold the list of Hardware IDs as strings. Promotes easy JSON Serialization 
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// Updated: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    public struct TrackerList
    {
        public List<string> trackerList;

        public TrackerList(List<string> givenList)
        {
            trackerList = givenList;
        }
    }

    /// <summary>
    /// Custom editor for displaying the hardware IDs for Vive trackers and allowing the user to select the
    /// needed ID. The user can refresh the list by hitting the refresh list button and it will pull all the
    /// Vive Trackers that are currently paired with SteamVR. The list is then saved as an EditorPref so that
    /// it can easily pull and dispaly the list.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// </summary>
    [CustomEditor(typeof(TrackedObjectID))]
    public class TrackedObjectIDEditor : Editor
    {
#if VRTK_DEFINE_SDK_STEAMVR
        #region PublicVariables
        public const string TrackerListPrefLocation = "VIRTUOSO.TrackerList";
        #endregion

        #region UnityEditorFunctions
        public override void OnInspectorGUI()
        {
            // Read
            serializedObject.Update();

            // Get properties of TrackeObjectID
            SerializedProperty indexProperty = serializedObject.FindProperty("trackerPopupIndex");
            SerializedProperty hardwareID = serializedObject.FindProperty("trackerHardwareID");

            // Give the user the option to manually empty the list
            if (GUILayout.Button("Clear Tracker List"))
            {
                ClearTrackerIDPref();
            }

            // Give the user the option to manually refresh the list
            if (GUILayout.Button("Refresh Tracker List"))
            {
                UpdateTrackerIDList();
            }

            // Give the user the ability to assign the index with a button push in the editor
            if(GUILayout.Button("Assign Index"))
            {
                AssignIndex();
            }

            // Make sure that the key has been set at least once and if not, refresh the tracker list
            string[] hardwareIDArray;

            // Check to see if their is a list in the Editor Preferences and use if it exists
            if(EditorPrefs.HasKey(TrackerListPrefLocation))
            {
                hardwareIDArray = GetHardwareIDArray();
            }
            else
            {
                hardwareIDArray = new string[0];
            }

            indexProperty.intValue = EditorGUILayout.Popup(indexProperty.intValue, hardwareIDArray);

            // Check to make sure there is something in the option list, if not, use an empty string
            if (hardwareIDArray.Length == 0)
            {
                hardwareID.stringValue = "";
            }
            else
            {
                hardwareID.stringValue = hardwareIDArray[indexProperty.intValue];
            }

            // Save
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region TrackerHelperFunctions
        private string[] GetHardwareIDArray()
        {
            string trackerJsonPref = EditorPrefs.GetString(TrackerListPrefLocation);

            TrackerList trackerListJson;

            try
            {
                trackerListJson = JsonUtility.FromJson<TrackerList>(trackerJsonPref);
            }
            catch(NullReferenceException)
            {
                Debug.LogWarning("No tracker list found in EditorPrefs.");
                return new string[0];
            }

            return trackerListJson.trackerList.ToArray();
        }

        /// <summary>
        /// Allows for the Editor to assign the index of a SteamVR Tracked Object at Editor time.
        /// </summary>
        public void AssignIndex()
        {
            EVRInitError initError = EVRInitError.None;
            OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Overlay);

            (serializedObject.targetObject as TrackedObjectID).AssignIndex();

            // Properly close reference to SteamVR so it can be access safely again later
            OpenVR.Shutdown();
        }

        [MenuItem("VIRTUOSO/Update Tracker ID List")]
        public static void UpdateTrackerIDList()
        {
            EVRInitError initError = EVRInitError.None;
            OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Overlay);
            List<string> hardwareIDList = new List<string>();

            for (uint n = 0; n < Constants.MAX_OPENVR_OBJECTS; n++)
            {
                ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass(n);

                if(deviceClass == ETrackedDeviceClass.GenericTracker)
                {
                    
                    hardwareIDList.Add(TrackedObjectID.GetHardwareIDFromIndex((int)n));
                }
            }

            // EditorPrefs cannot store lists but can store anything as a string, so convert the list to JSON and save that instead 
            string trackerListJson = JsonUtility.ToJson(new TrackerList(hardwareIDList));
            EditorPrefs.SetString(TrackerListPrefLocation, trackerListJson);

            // Properly close reference to SteamVR so it can be access safely again later
            OpenVR.Shutdown();
        }

        public static void ClearTrackerIDPref()
        {
            EditorPrefs.SetString(TrackerListPrefLocation, "");
        }
        #endregion
#endif
    }
}