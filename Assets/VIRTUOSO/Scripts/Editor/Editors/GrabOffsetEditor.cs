using CharlesRiverAnalytics.Virtuoso.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.EditorScripts
{
    /// <summary>
    /// Custom Editor for the GrabOffset class. This editor helps display the available SDKs that VRTK/VIRTUOSO has available to it.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [CustomEditor(typeof(GrabOffset))]
    public class GrabOffsetEditor : Editor
    {
        #region PrivateVariables
        private SerializedProperty selectedSDK;
        private SerializedProperty positionOffset;
        private SerializedProperty rotationOffset;
        #endregion

        #region UnityFunctions
        private void Awake()
        {
            selectedSDK = serializedObject.FindProperty("handOrControllerSDKName");
            positionOffset = serializedObject.FindProperty("positionOffset");
            rotationOffset = serializedObject.FindProperty("rotationOffset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            Func<VRTK_SDKInfo, ReadOnlyCollection<VRTK_SDKInfo>, string> sdkNameSelector = (info, installedInfos)
                    => info.description.prettyName;
            List<string> availableControllerSDKNames = VRTK_SDKManager.AvailableControllerSDKInfos.Select(info => sdkNameSelector(info, VRTK_SDKManager.AvailableControllerSDKInfos)).ToList<string>();
            List<string> availableHandSDKNames = VRTK_SDKManager.AvailableHandSDKInfos.Select(info => sdkNameSelector(info, VRTK_SDKManager.AvailableHandSDKInfos)).ToList<string>();
            List<string> allAvailableDevices = availableControllerSDKNames.Concat(availableHandSDKNames).Distinct().ToList<string>();

            int index = Mathf.Max(0, allAvailableDevices.IndexOf(selectedSDK.stringValue));

            index = EditorGUILayout.Popup("SDK", index, allAvailableDevices.ToArray());
            selectedSDK.stringValue = allAvailableDevices[index];

            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}