#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using CharlesRiverAnalytics.Virtuoso.Utilities;
using System;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Handles the In-Scene UI for creating a new pattern. Allows the user to load saved patterns or work on new ones.
    /// From this class, the user is able to manage the curves that are attached to the pattern by either removing them,
    /// adding a saved one, or starting on a new one. The class also passes the values that the user sets for the collision
    /// resolution and timing information.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public sealed class HapticSceneViewGUI : IDisposable
    {
        #region PublicVariables
        public HapticPatternWindow patternWindow;
        #endregion

        #region PrivateVariables
        private string patternName;
        private ScriptableHapticCurve curveToAdd;
        private ScriptableHapticPattern patternToLoad;
        private PatternCollisionResolution selectedCollisionResolution;
        private int collisionResolutionIndex;
        private PlaybackTiming selectedPlaybackTiming;
        private float customPlaybackTiming;
        private OffsetUse selectedOffsetUse;
        private PatternOvershootResolution selectedHeightOvershoot;
        private PatternOvershootResolution selectedAngleOvershoot;
        private GUIStyle textLabelStyle;

        private const float UI_SCREEN_WIDTH = 4.0f;
        #endregion

        #region HapticSceneMethods
        public HapticSceneViewGUI()
        {
            SetUpRenderer();

            CreateGUIStyles();
        }

        public void Dispose()
        {
            CleanUp();
        }

        public void SetUpRenderer()
        {
            // There should only be 1 delegate from this class at a time, so remove the event in case it already exists and this was called
            SceneView.onSceneGUIDelegate -= RenderSceneGUI;

            SceneView.onSceneGUIDelegate += RenderSceneGUI;
        }

        /// <summary>
        /// Handles how to render the actual elements of the UI
        /// </summary>
        public void RenderSceneGUI(SceneView sceneview)
        {
            Handles.BeginGUI();

            // Make a canvas that spans the entire height of the scene and a portion of the width
            Rect canvasRect = new Rect(0, 0, Screen.width / UI_SCREEN_WIDTH, Screen.height);
            GUILayout.BeginArea(canvasRect);

            // Make the area behind the GUI a gray, slightly transparent canvas
            EditorGUI.DrawRect(canvasRect, new Color(0.5f, 0.5f, 0.5f, 0.5f));

            Rect verticleLayoutBox = EditorGUILayout.BeginVertical();
            GUI.Box(verticleLayoutBox, GUIContent.none);

            // Add header for loading/saving patterns
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Pattern Management", MessageType.None);

            // Load a saved pattern
            EditorGUILayout.LabelField("Load Saved Pattern:", textLabelStyle);
            patternToLoad = EditorGUILayout.ObjectField(patternToLoad, typeof(ScriptableHapticPattern), false) as ScriptableHapticPattern;

            if (patternToLoad != null)
            {
                if (GUILayout.Button("Load Pattern"))
                {
                    LoadPattern();

                    patternToLoad = null;
                }
            }

            // Save changes and return to previous scene
            if (GUILayout.Button("Save and return"))
            {
                patternWindow.SavePattern(patternName, selectedCollisionResolution, collisionResolutionIndex, selectedPlaybackTiming, customPlaybackTiming, selectedOffsetUse, selectedHeightOvershoot, selectedAngleOvershoot);
            }

            // Save the background color to temporarily change the discard background to red to indicate the negative nature of the button
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            // Discard all the changes and return to previous scene
            if (GUILayout.Button("Discard changes"))
            {
                bool discardResult = EditorUtility.DisplayDialog("Discard changes?",
                                                                 "Are you sure you want to discard all changes made to this pattern?",
                                                                 "Yes",
                                                                 "Cancel");

                if (discardResult)
                {
                    patternWindow.DiscardChanges();
                }
            }

            GUI.backgroundColor = oldColor;

            // Add header for pattern settings
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Pattern Settings", MessageType.None);

            // Prompt for pattern name which will become the file name
            EditorGUILayout.LabelField("Pattern Name:", textLabelStyle);
            patternName = EditorGUILayout.TextField(patternName);

            // Prompt for how to handle intensity collisions
            EditorGUILayout.LabelField("Collision Resolution:", textLabelStyle);
            selectedCollisionResolution = (PatternCollisionResolution)EditorGUILayout.EnumPopup(selectedCollisionResolution);

            // Prompt for the index of the curve to use if the resolution is set to CurvePriority
            if (selectedCollisionResolution == PatternCollisionResolution.CurvePriority)
            {
                EditorGUILayout.LabelField("Curve Index:", textLabelStyle);
                collisionResolutionIndex = EditorGUILayout.IntField(collisionResolutionIndex);
            }

            // Prompt for the playback timing on the pattern
            EditorGUILayout.LabelField("Playback Timing:", textLabelStyle);
            selectedPlaybackTiming = (PlaybackTiming)EditorGUILayout.EnumPopup(selectedPlaybackTiming);

            // Prompt for custom playback timing if they have selected CustomPlayback as the option
            if (selectedPlaybackTiming == PlaybackTiming.Custom)
            {
                EditorGUILayout.LabelField("Custom Playback Time:", textLabelStyle);
                customPlaybackTiming = EditorGUILayout.FloatField(customPlaybackTiming);
            }

            // Prompt for offset use
            EditorGUILayout.LabelField("Offset Use:", textLabelStyle);
            selectedOffsetUse = (OffsetUse)EditorGUILayout.EnumPopup(selectedOffsetUse);

            // Prompt for height overshoot
            EditorGUILayout.LabelField("Height Overshoot Resolution:", textLabelStyle);
            selectedHeightOvershoot = (PatternOvershootResolution)EditorGUILayout.EnumPopup(selectedHeightOvershoot);

            // Prompt for angle overshoot
            EditorGUILayout.LabelField("Angle Overshoot Resolution:", textLabelStyle);
            selectedAngleOvershoot = (PatternOvershootResolution)EditorGUILayout.EnumPopup(selectedAngleOvershoot);

            // Header for management curves
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Curve Management", MessageType.None);

            // Build string list of attached curves
            string[] curveNames = new string[patternWindow.HapticPattern.curveList.Count];

            for (int n = 0; n < patternWindow.HapticPattern.curveList.Count; n++)
            {
                curveNames[n] = patternWindow.HapticPattern.curveList[n].ToString();
            }

            // Show list of curves on the pattern for the user to select
            EditorGUILayout.LabelField("Current curve:", textLabelStyle);
            patternWindow.CurveIndex = EditorGUILayout.Popup(patternWindow.CurveIndex, curveNames);

            // Allow the user to add a new curve
            if (GUILayout.Button("Add New Curve"))
            {
                patternWindow.AddNewCurve("Default");
            }

            if (GUILayout.Button("Remove Current Curve"))
            {
                patternWindow.RemoveCurrentCurve();
            }

            // Add selected curve to pattern button
            EditorGUILayout.LabelField("Add Saved Curve:", textLabelStyle);
            curveToAdd = EditorGUILayout.ObjectField(curveToAdd, typeof(ScriptableHapticCurve), false) as ScriptableHapticCurve;

            if (curveToAdd != null)
            {
                if (GUILayout.Button("Add Curve"))
                {
                    patternWindow.AddSavedCurve(curveToAdd);

                    curveToAdd = null;
                }
            }

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void LoadPattern()
        {
            patternName = ScriptableObjectUtility.GetScriptableObjectFileName(patternToLoad);
            selectedCollisionResolution = patternToLoad.collisionResolution;
            collisionResolutionIndex = patternToLoad.curvePriorityIndex;
            selectedPlaybackTiming = patternToLoad.playbackTiming;
            customPlaybackTiming = patternToLoad.customPlaybackTiming;
            selectedOffsetUse = patternToLoad.offsetUse;
            selectedHeightOvershoot = patternToLoad.heightOvershootResolution;
            selectedAngleOvershoot = patternToLoad.angleOvershootResolution;

            patternWindow.LoadPattern(patternToLoad);
        }

        private void CreateGUIStyles()
        {
            textLabelStyle = new GUIStyle();
            textLabelStyle.normal.textColor = Color.black;
        }

        private void CleanUp()
        {
            SceneView.onSceneGUIDelegate -= RenderSceneGUI;
        }
        #endregion
    }
}
#endif