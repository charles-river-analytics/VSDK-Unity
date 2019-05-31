using CharlesRiverAnalytics.Virtuoso.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Custom editor for the Haptic Manager class. Displays a list of all the available HapticDevices
    /// if they are using the HapticSystem Attribute class. It allows the devices to be enabled/disabled
    /// by the developer to indicate what haptic device they are targeting.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [CustomEditor(typeof(HapticManager))]
    public class HapticManagerEditor : Editor
    {
        #region PrivateVariables
        private HapticManager targetManager;
        private bool useVisualizer;
        private Dictionary<string, List<HapticDeviceInfo>> hapticSystemAndDevices;
        private Dictionary<string, HapticSystemEditorInfo> hapticSystemEditorInfo;
        #endregion

        #region Unity Functions
        protected void Awake()
        {
            targetManager = target as HapticManager;

            hapticSystemAndDevices = new Dictionary<string, List<HapticDeviceInfo>>();
            hapticSystemEditorInfo = new Dictionary<string, HapticSystemEditorInfo>();

            // Get all the haptic device classes in the project
            foreach (Type type in Assembly.GetAssembly(typeof(HapticDevice)).GetTypes())
            {
                object[] attributeList = type.GetCustomAttributes(typeof(HapticSystemAttribute), true);

                if (attributeList.Length > 0)
                {
                    foreach (object currentAttribute in attributeList)
                    {
                        HapticSystemAttribute hapticAttribute = currentAttribute as HapticSystemAttribute;

                        if (!hapticSystemAndDevices.ContainsKey(hapticAttribute.SystemName))
                        {
                            hapticSystemAndDevices.Add(hapticAttribute.SystemName, new List<HapticDeviceInfo>()
                                                                                            {
                                                                                                new HapticDeviceInfo(hapticAttribute, type)
                                                                                            });

                            hapticSystemEditorInfo.Add(hapticAttribute.SystemName, new HapticSystemEditorInfo());
                        }
                        else
                        {
                            hapticSystemAndDevices[hapticAttribute.SystemName].Add(new HapticDeviceInfo(hapticAttribute, type));
                        }
                    }
                }
            }

            // Use EditorPrefs to find the devices that are being used by the developer
            foreach (KeyValuePair<string, List<HapticDeviceInfo>> currentDevice in hapticSystemAndDevices)
            {
                for (int n = 0; n < currentDevice.Value.Count; n++)
                {
                    currentDevice.Value[n].isSelected = EditorPrefs.GetBool(Constants.EditorPrefLocation + currentDevice.Key + "." + currentDevice.Value[n].systemAttribute.DeviceName);
                }
            }

            useVisualizer = EditorPrefs.GetBool(Constants.EditorPrefLocation + "UseVisualizer");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Check to see if the dictionary is set up, if not, then work cannot be done
            if(hapticSystemAndDevices == null)
            {
                Awake();
            }

            // Reads the latest values on the current (serialized) object (i.e., the one that is selected)
            serializedObject.Update();

            // Ask if they want the visualizer attached
            useVisualizer = EditorGUILayout.Toggle("Visualize Haptics", useVisualizer);

            if (useVisualizer)
            {
                HapticVisualizer attachedVisualizer = targetManager.gameObject.GetComponent<HapticVisualizer>();

                if (attachedVisualizer == null)
                {
                    targetManager.gameObject.AddComponent<HapticVisualizer>();
                }
            }
            else
            {
                HapticVisualizer attachedVisualizer = targetManager.gameObject.GetComponent<HapticVisualizer>();

                if (attachedVisualizer != null)
                {
                    DestroyImmediate(attachedVisualizer);
                }
            }

            foreach (var hapticDevice in hapticSystemAndDevices)
            {
                hapticSystemEditorInfo[hapticDevice.Key].isFoldedOut = EditorGUILayout.Foldout(hapticSystemEditorInfo[hapticDevice.Key].isFoldedOut, hapticDevice.Key);

                if (hapticSystemEditorInfo[hapticDevice.Key].isFoldedOut)
                {
                    // Display each haptic device
                    foreach (HapticDeviceInfo currentDevice in hapticSystemAndDevices[hapticDevice.Key])
                    {
                        currentDevice.isSelected = EditorGUILayout.Toggle(currentDevice.systemAttribute.DeviceName, currentDevice.isSelected);
                    }
                }

                // Save values to the editor prefs
                for (int n = 0; n < hapticDevice.Value.Count; n++)
                {
                    // Add/remove any scripts that have changed
                    if (EditorPrefs.GetBool(Constants.EditorPrefLocation + hapticDevice.Key + "." + hapticDevice.Value[n].systemAttribute.DeviceName) != hapticDevice.Value[n].isSelected)
                    {
                        // Add component
                        if (hapticDevice.Value[n].isSelected)
                        {
                            GameObject deviceSystemObject = GameObject.Find(hapticDevice.Key);

                            if (deviceSystemObject == null)
                            {
                                deviceSystemObject = new GameObject(hapticDevice.Key);
                                deviceSystemObject.transform.parent = (target as HapticManager).transform;
                            }

                            GameObject deviceObject = new GameObject(hapticDevice.Value[n].systemAttribute.DeviceName);
                            deviceObject.transform.parent = deviceSystemObject.transform;

                            HapticDevice deviceInfo = deviceObject.AddComponent(hapticDevice.Value[n].deviceType) as HapticDevice;
                            deviceInfo.ApplyDefaultData(hapticDevice.Value[n].systemAttribute);
                        }
                        // Remove component
                        else
                        {
                            GameObject objToRemove = GameObject.Find(hapticDevice.Key + "/" + hapticDevice.Value[n].systemAttribute.DeviceName);

                            if (objToRemove != null)
                            {
                                GameObject parentObj = objToRemove.transform.parent.gameObject;

                                DestroyImmediate(objToRemove);

                                if (parentObj.transform.childCount == 0)
                                {
                                    DestroyImmediate(parentObj);
                                }
                            }
                        }
                    }

                    // Write to EditorPrefs so the used devices are saved
                    EditorPrefs.SetBool(Constants.EditorPrefLocation + hapticDevice.Key + "." + hapticDevice.Value[n].systemAttribute.DeviceName, hapticDevice.Value[n].isSelected);
                }
            }

            EditorPrefs.SetBool(Constants.EditorPrefLocation + "UseVisualizer", useVisualizer);

            // Write properties back to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }

    /// <summary>
    /// Holdds data for each haptic device in a  hpatic system for the haptic manager editor.
    /// </summary>
    public class HapticDeviceInfo
    {
        public HapticSystemAttribute systemAttribute;
        public Type deviceType;
        public bool isSelected;

        public HapticDeviceInfo(HapticSystemAttribute sysAttribute, Type devType)
        {
            systemAttribute = sysAttribute;
            deviceType = devType;

            isSelected = false;
        }
    }

    /// <summary>
    /// Holds data related to each haptic system for the haptic manager editor.
    /// </summary>
    public class HapticSystemEditorInfo
    {
        public bool isFoldedOut;

        public HapticSystemEditorInfo()
        {
            isFoldedOut = false;
        }
    }
}