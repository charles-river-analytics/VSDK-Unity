using CharlesRiverAnalytics.Virtuoso.Utilities;
using UnityEditor;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Editor window for adjusting the settings used by the Haptic SDK. This works by setting the 
    /// values on the ScriptableHapticSetting class to save them. Also provides a utility to reset
    /// the values to their default.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class HapticSettingsWindow : EditorWindow
    {
        #region PrivateVariables
        private ScriptableHapticSettings persistentSettingsData;
        private ScriptableHapticSettings temporarySettingsData;
        #endregion

        #region SettingMethods
        [MenuItem("VIRTUOSO/Haptics/HapticSDK Settings")]
        private static void SetupHaptics()
        {
            EditorWindow.GetWindow(typeof(HapticSettingsWindow));
        }

        private void GetMostRecentSettings()
        {
            persistentSettingsData = ScriptableHapticSettings.GetSettings();

            temporarySettingsData = persistentSettingsData;
        }
        #endregion

        #region Unity Functions
        private void Awake()
        {
            GetMostRecentSettings();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Prompt for default save location for patterns
            temporarySettingsData.defaultPatternSaveLocation = EditorGUILayout.DelayedTextField("Pattern Save Location", temporarySettingsData.defaultPatternSaveLocation);

            // Prompt for default save location for curves
            temporarySettingsData.defaultCurveSaveLocation = EditorGUILayout.DelayedTextField("Curve Save Location", temporarySettingsData.defaultCurveSaveLocation);

            // Prompt for time granularity
            temporarySettingsData.defaultTimeGranularity = EditorGUILayout.FloatField("Time Granularity", temporarySettingsData.defaultTimeGranularity);

            // Prompt for default intensity
            temporarySettingsData.defaultIntensity = EditorGUILayout.FloatField("Default Intensity", temporarySettingsData.defaultIntensity);

            // Prompt for curve granularity
            temporarySettingsData.defaultCurveRenderingGranularity = EditorGUILayout.IntField("Curve Granularity", temporarySettingsData.defaultCurveRenderingGranularity);

            EditorGUILayout.BeginHorizontal();

            // Reset to default
            if (GUILayout.Button("Reset to Default"))
            {
                // Delete the scriptable object, it will be recreated next time it is accessed with the default settings
                ScriptableObjectUtility.DeleteScriptableObject(persistentSettingsData);

                GetMostRecentSettings();
            }

            // Save Settings
            if (GUILayout.Button("Save Settings"))
            {
                temporarySettingsData.OnValidate();

                persistentSettingsData = temporarySettingsData;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}