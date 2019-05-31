using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Helper class that contains a set of methods for managing scriptable object assets.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public static class ScriptableObjectUtility
    {
#if UNITY_EDITOR
        /// <summary>
        /// Saves a scriptable object in a specified location with a specified name.
        /// </summary>
        /// <param name="objectToSave">The scriptable object to save</param>
        /// <param name="filePath">The folder location to hold the scriptable object</param>
        /// <param name="fileName">The file name for the scriptable object</param>
        public static void SaveScriptableObject(ScriptableObject objectToSave, string filePath, string fileName)
        {
            // Check to make sure the path exists
            if (!AssetDatabase.IsValidFolder(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            // Check to make sure that a file name was given, if not default it
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "UnNamedScriptableObject";
            }

            string fullPath = filePath + fileName + ".asset";

            if (!AssetDatabase.Contains(objectToSave))
            {
                AssetDatabase.CreateAsset(objectToSave, fullPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Renames a scriptable object's file name with the provided new one
        /// </summary>
        /// <param name="objectToRename">The scriptable object to change</param>
        /// <param name="fileName">The new file name </param>
        public static void RenameScriptableObjectFile(ScriptableObject objectToRename, string fileName)
        {
            string assetPath = AssetDatabase.GetAssetPath(objectToRename.GetInstanceID());

            if (!string.IsNullOrEmpty(assetPath))
            {
                // Check to make sure that a file name was given, if not default it
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "UnNamedScriptableObject";
                }

                AssetDatabase.RenameAsset(assetPath, fileName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Deletes the .asset file for a scriptable object that is passed in
        /// </summary>
        /// <param name="objectToDelete">The scriptableobject to delete</param>
        public static void DeleteScriptableObject(ScriptableObject objectToDelete)
        {
            string assetPath = AssetDatabase.GetAssetPath(objectToDelete.GetInstanceID());

            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Provides the file name without extension for a scriptable object
        /// </summary>
        /// <param name="objectToName">The scriptable object that file name is needed</param>
        /// <returns>A string with the file name and no extension</returns>
        public static string GetScriptableObjectFileName(ScriptableObject objectToName)
        {
            string fullPath = AssetDatabase.GetAssetPath(objectToName.GetInstanceID());

            return Path.GetFileNameWithoutExtension(fullPath);
        }
#endif
    }
}