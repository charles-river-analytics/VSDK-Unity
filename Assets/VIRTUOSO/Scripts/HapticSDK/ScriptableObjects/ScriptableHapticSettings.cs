using CharlesRiverAnalytics.Virtuoso.Utilities;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Holds reusable data that is needed through out the Haptic SDK.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class ScriptableHapticSettings : ScriptableObject
    {
        #region PublicVariables
        // Where the default setting file will be hosted
        public const string DEFAULT_SETTING_LOCATION = "Assets/VIRTUOSO/Resources/Editor/";
        // What the default setting file will be named
        public const string DEFAULT_SETTING_NAME = "HapticSettings";

        // Where any patterns created by the UI will be saved
        public string defaultPatternSaveLocation = "Assets/VIRTUOSO/Resources/Patterns/";
        // Where any curves created by the UI will be saved
        public string defaultCurveSaveLocation = "Assets/VIRTUOSO/Resources/Curves/";
        // The default intensity value for any new inflection point
        public float defaultIntensity = 0.5f;
        // The default time increase when a new inflection point is added
        public float defaultTimeGranularity = 0.5f;
        // The amount of samples that will be used when rendering the curve between two points in a haptic curve
        public int defaultCurveRenderingGranularity = 10;
        #endregion

        #region PublicAPI
        public static ScriptableHapticSettings GetSettings()
        {
            ScriptableHapticSettings settings = Resources.Load(DEFAULT_SETTING_NAME) as ScriptableHapticSettings;

            // If there is no settings, create one with the default values
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ScriptableHapticSettings>();
#if UNITY_EDITOR
                ScriptableObjectUtility.SaveScriptableObject(settings, DEFAULT_SETTING_LOCATION, DEFAULT_SETTING_NAME);
#endif
            }

            return settings;
        }
        #endregion

        #region UnityFunctions
        public void OnValidate()
        {
            // Keep intensity between 0 and 1
            defaultIntensity = Mathf.Clamp01(defaultIntensity);

            // Keep timing non-negative
            if (defaultTimeGranularity <= 0.0f)
            {
                defaultTimeGranularity = 0.01f;
            }

#if UNITY_EDITOR
            // Make sure the directory exists
            if (!Directory.Exists(defaultPatternSaveLocation))
            {
                Directory.CreateDirectory(defaultPatternSaveLocation);
                AssetDatabase.Refresh();
            }

            // Make sure the directory exists
            if (!Directory.Exists(defaultCurveSaveLocation))
            {
                Directory.CreateDirectory(defaultCurveSaveLocation);
                AssetDatabase.Refresh();
            }
#endif
        }
        #endregion
    }
}