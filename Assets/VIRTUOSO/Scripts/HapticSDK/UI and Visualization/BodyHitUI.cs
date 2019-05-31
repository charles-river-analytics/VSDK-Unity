using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// In-Scene UI for a keyframe in the Haptic Pattern. When the GameObject that holds the keyframe data is double
    /// clicked in the scene, the UI generated in this class will appear. It allows a user to either remove the keyframe
    /// or set a new height, angle, intensity, or timing information for the keyframe.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary
    [ExecuteInEditMode]
    public class BodyHitUI : MonoBehaviour, IComparable<BodyHitUI>
    {
        #region PublicVariables
        public float hitAngle;
        public float hitHeight;
        public float intensity;
        public float timing;
        public int keyFrameIndex;
        // This is true when the Pattern UI has added the point but it has not been committed to the curve yet
        public bool isTemporary = false;
        #endregion

        #region PrivateVariables
        private ScriptableHapticCurve hapticCurve;
        private int hapticCurveIndex;
        private bool shouldDisplayUI = false;
        private Vector3 lastScreenPosition;
        private Rect uiRect;

        private const int UI_WIDTH = 200;
        private const int UI_HEIGHT = 400;
        #endregion

        #region PublicAPI
        public void SetValues(float angle, float height, float intense, float time, int keyIndex, int curveIndex, ScriptableHapticCurve curve)
        {
            hitAngle = angle;
            hitHeight = height;
            intensity = intense;
            timing = time;
            keyFrameIndex = keyIndex;
            hapticCurveIndex = curveIndex;
            hapticCurve = curve;
        }

        /// <summary>
        /// Shows the UI with the angle, height, timing, and intensity that the point has.
        /// </summary>
        /// <param name="screenPosition">The position on the screen where the UI was asked to be displayed</param>
        public void DisplayUI(Vector3 screenPosition)
        {
            lastScreenPosition = screenPosition;

            shouldDisplayUI = true;
        }

        /// <summary>
        /// Hides the UI when it is no longer needed. If the UI has focus, it cannot be hidden.
        /// </summary>
        /// <param name="screenPosition">The position on the screen where the mouse was clicked</param>
        /// <returns>True if the UI was hidden, otherwise false</returns>
        public bool HideUI(Vector3 screenPosition, bool forceClose = false)
        {
            if (forceClose || !uiRect.Contains(screenPosition))
            {
                shouldDisplayUI = false;

                lastScreenPosition = Vector3.zero;

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region BodyHitMethods
        private void UpdateCurve()
        {
            // Set the new keyframe in the curves
            int newKey = hapticCurve.UpdateKey(keyFrameIndex, hitHeight, hitAngle, intensity, timing);

#if UNITY_EDITOR
            // If the new key is different, then all the other keys above it may different as well, update them here
            if (newKey != keyFrameIndex)
            {
                FindObjectOfType<HapticPatternWindow>().UpdateKeyIndices(hapticCurveIndex, keyFrameIndex, newKey);
            }
#endif

            // Update GO position to match new location
            transform.position = GetComponentInParent<BodyCoordinate>().CalculatePositionFromBodyCoordinateHit(new BodyCoordinateHit(hitHeight, hitAngle), true);
        }

        private void RemovePointOnCurve()
        {
            hapticCurve.RemoveKey(keyFrameIndex);

#if UNITY_EDITOR
            // Make sure that keys are updated since this index is now gone
            FindObjectOfType<HapticPatternWindow>().UpdateKeyIndices(hapticCurveIndex, keyFrameIndex);
#endif

            DestroyImmediate(gameObject);
        }
        #endregion

        #region EventListener
#if UNITY_EDITOR
        public void RenderSceneGUI(SceneView sceneview)
        {
            if (shouldDisplayUI)
            {
                sceneview.Repaint();

                Handles.BeginGUI();
                uiRect = new Rect(lastScreenPosition.x, lastScreenPosition.y, UI_WIDTH, UI_HEIGHT);

                GUILayout.BeginArea(uiRect);

                // Make a gray, slightly transparent canvas
                EditorGUI.DrawRect(uiRect, new Color(1.0f, 0.5f, 0.5f, 1.0f));

                Rect rect = EditorGUILayout.BeginVertical();
                GUI.Box(rect, GUIContent.none);

                // Prompt and display angle
                hitAngle = EditorGUILayout.FloatField("Angle: ", hitAngle);

                // Sanitize
                hitAngle = Mathf.Clamp(hitAngle, 0.0f, 360.0f);

                // Prompt and display height
                hitHeight = EditorGUILayout.FloatField("Height: ", hitHeight);

                // Sanitize
                hitHeight = Mathf.Clamp01(hitHeight);

                // Prompt and display intensity
                intensity = EditorGUILayout.FloatField("Intensity: ", intensity);

                // Sanitize
                intensity = Mathf.Clamp01(intensity);

                // Prompt and display timing
                timing = EditorGUILayout.FloatField("Timing: ", timing);

                // Sanitize
                if(timing < 0.0f)
                {
                    timing = 0.0f;
                }

                // Display update button
                if (GUILayout.Button("Update"))
                {
                    UpdateCurve();
                }

                // Display remove point button
                if (GUILayout.Button("Remove"))
                {
                    RemovePointOnCurve();
                }

                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
                Handles.EndGUI();
            }
        }
#endif
        #endregion

        #region IComparableImplementation
        public int CompareTo(BodyHitUI other)
        {
            if (other == null)
            {
                return 1;
            }

            if (this.keyFrameIndex > other.keyFrameIndex)
            {
                return 1;
            }
            else if (other.keyFrameIndex > this.keyFrameIndex)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region UnityFunctions
#if UNITY_EDITOR
        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += RenderSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= RenderSceneGUI;
        }
#endif
        #endregion
    }
}