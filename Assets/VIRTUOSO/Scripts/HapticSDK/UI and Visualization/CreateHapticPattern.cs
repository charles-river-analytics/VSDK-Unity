#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Single method class to add a menu item to create a new haptic pattern.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public static class CreateHapticPattern
    {
        [MenuItem("VIRTUOSO/Haptics/Create Haptic Pattern")]
        public static void CreatePattern()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            string activeScene = EditorSceneManager.GetActiveScene().path;

            // Set up the scene to edit the pattern
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            HapticPatternWindow activePatternWindow = Camera.main.gameObject.AddComponent<HapticPatternWindow>();
            activePatternWindow.SetupCapsuleScene(activeScene);
        }
    }
}
#endif