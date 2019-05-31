#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using CharlesRiverAnalytics.Virtuoso.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using CharlesRiverAnalytics.Virtuoso.Scriptable;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// A portion of the In-Scene UI for creating haptic patterns. This class is the one responsbile for the actual
    /// haptic pattern and managing the state of information that is known about the pattern, such as keeping a reference
    /// to all the GameObjects that refer to a specific keyframe in the curve. If a pattern has to be loaded or saved, it
    /// is down through the script.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [ExecuteInEditMode]
    public class HapticPatternWindow : MonoBehaviour
    {
        #region PublicVariables
        public GameObject hitLocationPrefab;
        public Material activeHitMaterial;
        public Material inactiveHitMaterial;
        public Material capsuleMaterial;
        public ScriptableLineRenderer lineRendererData;
        #endregion

        #region PrivateVariables
        private string previousScene;
        private GameObject capsuleGO;
        private BodyHitListener hitListener;
        private HapticSceneViewGUI hapticSceneView;
        private BodyCoordinate capsuleBodyCoordinate;
        private ScriptableHapticPattern currentPattern;
        private bool setupComplete = false;
        // When true, the scene is cleaning up and will stop a few process
        private bool isClosing = false;
        private int currentCurveIndex = 0;
        private LineRenderer currentCurveLineRenderer;
        private ScriptableHapticSettings hapticSettings;
        // Stores the inflection points for each curve attached to the pattern
        private Dictionary<int, List<BodyHitUI>> bodyHitsPerCurve;
        #endregion

        #region PublicProperties
        public int CurveIndex
        {
            get
            {
                return currentCurveIndex;
            }
            set
            {
                if (!isClosing)
                {
                    // Since a new curve value is about to be set, update the visualization on the keyframe GO to inactive
                    UpdateCurveKeyFrameVisualization(currentCurveIndex, false);
                }

                currentCurveIndex = value;

                if (!isClosing)
                {
                    // A new value was set, now make these keyframes the active one
                    UpdateCurveKeyFrameVisualization(currentCurveIndex, true);

                    DrawCurrentCurvePath();
                }
            }
        }

        public ScriptableHapticPattern HapticPattern
        {
            get
            {
                return currentPattern;
            }
        }
        #endregion

        #region PublicAPI
        /// <summary>
        /// Sets up a new scene for pattern creation. This method will set up a capsule with a body coordinate system, a line
        /// renderer for the haptic points, and the GUI and listeners needed to manage the new pattern.
        /// </summary>
        /// <param name="previousSceneName">The string of the scene that the user needs to return to when the leave this Scene UI</param>
        public void SetupCapsuleScene(string previousSceneName)
        {
            previousScene = previousSceneName;

            // Don't allow playmode to be entered while in this scene
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

            // Hook into mouse events from the scene
            hitListener = new BodyHitListener();
            hitListener.BodyPartHit += HitListener_BodyPartHit;

            // Set up the In-Scene GUI
            hapticSceneView = new HapticSceneViewGUI
            {
                patternWindow = this
            };

            // Connect to the OnGUI event cycle
            SceneView.onSceneGUIDelegate += OnGUIDelegate;

            // Set up the capsule
            capsuleGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsuleGO.GetComponent<Renderer>().material = capsuleMaterial;
            capsuleGO.hideFlags = HideFlags.HideInInspector;
            capsuleBodyCoordinate = capsuleGO.AddComponent<BodyCoordinate>();
            capsuleBodyCoordinate.drawGizmoUnselected = true;

            // Set up the line render for the active curve line
            GameObject lineHolder = new GameObject("Line Holder", typeof(LineRenderer));
            lineHolder.transform.parent = capsuleGO.transform;
            lineHolder.hideFlags = HideFlags.HideInHierarchy;

            currentCurveLineRenderer = lineHolder.GetComponent<LineRenderer>();

            currentCurveLineRenderer.widthMultiplier = lineRendererData.widthMultiplier;
            currentCurveLineRenderer.numCornerVertices = lineRendererData.numCornerVertices;
            currentCurveLineRenderer.numCapVertices = lineRendererData.numCapVertices;
            currentCurveLineRenderer.material = lineRendererData.lineMaterial;
            currentCurveLineRenderer.receiveShadows = lineRendererData.receiveShadows;
            currentCurveLineRenderer.allowOcclusionWhenDynamic = lineRendererData.allowOcclusionWhenDynamic;
            currentCurveLineRenderer.startColor = lineRendererData.lineStartColor;
            currentCurveLineRenderer.endColor = lineRendererData.lineEndColor;

            // Start with 0 points in the line
            currentCurveLineRenderer.positionCount = 0;

            // Focus the camera on the capsule
            Selection.activeGameObject = capsuleGO;
            SceneView.lastActiveSceneView.FrameSelected();

            // Start work on the haptic pattern
            currentPattern = ScriptableObject.CreateInstance<ScriptableHapticPattern>();
            AddNewCurve("Default");

            // Don't need the event listener for a new scene anymore, wait for when this scene closes
            setupComplete = true;
        }

        /// <summary>
        /// Saves the pattern that was being worked on to the set default location with the given file name.
        /// </summary>
        /// <param name="patternName">The string that represents the file name without the extension</param>
        public void SavePattern(string patternName, PatternCollisionResolution selectedCollisionResolution, int collisionResolutionIndex,
                                PlaybackTiming selectedPlaybackTiming, float customPlaybackTiming, OffsetUse selectedOffsetUse,
                                PatternOvershootResolution selectedHeightOvershoot, PatternOvershootResolution selectedAngleOvershoot)
        {
            isClosing = true;

            // Save the various settings for the pattern
            HapticPattern.collisionResolution = selectedCollisionResolution;
            HapticPattern.curvePriorityIndex = collisionResolutionIndex;
            HapticPattern.playbackTiming = selectedPlaybackTiming;
            HapticPattern.customPlaybackTiming = customPlaybackTiming;
            HapticPattern.offsetUse = selectedOffsetUse;
            HapticPattern.heightOvershootResolution = selectedHeightOvershoot;
            HapticPattern.angleOvershootResolution = selectedAngleOvershoot;

            ScriptableObjectUtility.SaveScriptableObject(HapticPattern, hapticSettings.defaultPatternSaveLocation, patternName);

            for (int n = 0; n < HapticPattern.curveList.Count; n++)
            {
                // Only rename curves that this pattern created
                if (HapticPattern.curveList[n].temporaryCurve)
                {
                    ScriptableObjectUtility.RenameScriptableObjectFile(HapticPattern.curveList[n], patternName + "_Curve" + n);

                    // This curve is no longer temporary and should be saved
                    HapticPattern.curveList[n].temporaryCurve = false;
                }
            }

            ReturnToPreviousScene();
        }

        /// <summary>
        /// Updates the key indices in the bodyHitCurve dictionary. Since the developer may remove
        /// or update the ordering of the keys, the dictionary needs to be updated with the correct
        /// key to make sure the GameObject that represents that keyframe is correct.
        /// </summary>
        /// <param name="curveIndex">The index of the curve on the pattern that this applies to</param>
        /// <param name="changedIndex">The index that was changed or removed</param>
        /// <param name="changedTowardIndex">If a key was reordered, this is the new value for that key</param>
        public void UpdateKeyIndices(int curveIndex, int changedIndex, int changedTowardIndex = -1)
        {
            // Key was changed, update the rest
            if (changedTowardIndex != -1)
            {
                if (changedIndex < changedTowardIndex)
                {
                    for (int n = changedIndex; n <= changedTowardIndex; n++)
                    {
                        bodyHitsPerCurve[curveIndex][n].keyFrameIndex--;
                    }
                }
                else
                {
                    for (int n = changedTowardIndex; n <= changedIndex; n++)
                    {
                        bodyHitsPerCurve[curveIndex][n].keyFrameIndex++;
                    }
                }

                // Set the key index for the changed one
                bodyHitsPerCurve[curveIndex][changedIndex].keyFrameIndex = changedTowardIndex;
            }
            // Key was deleted, update the rest
            else
            {
                for (int n = changedIndex; n < bodyHitsPerCurve[curveIndex].Count; n++)
                {
                    // Only one point can be removed at a time so just decrement all the indices above the changed one
                    bodyHitsPerCurve[curveIndex][n].keyFrameIndex--;
                }

                // Remove the BodyHitUI from the list of known BodyHits
                bodyHitsPerCurve[curveIndex].RemoveAt(changedIndex);
            }

            // Since an index and key values were changed, resort the list by the keyframe index 
            bodyHitsPerCurve[curveIndex].Sort();
        }

        /// <summary>
        /// Removes any changes for the pattern and returns back to the prevoius scene. If a saved pattern
        /// or curve was used, any temporary patterns or curves will be removed and their associated .asset
        /// files will be deleted.
        /// </summary>
        public void DiscardChanges()
        {
            isClosing = true;

            for (int n = HapticPattern.curveList.Count - 1; n >= 0; n--)
            {
                // Delete all curves attached to the pattern that are labeled as temporary
                if (HapticPattern.curveList[n].temporaryCurve)
                {
                    ScriptableObjectUtility.DeleteScriptableObject(HapticPattern.curveList[n]);

                    bodyHitsPerCurve.Remove(n);
                }
                // Otherwise, go through the curve and look for any temporary keyframes in the curve
                else
                {
                    for (int h = bodyHitsPerCurve[n].Count - 1; h >= 0; h--)
                    {
                        // Remove this keyframe from the curve
                        if (bodyHitsPerCurve[n][h].isTemporary)
                        {
                            HapticPattern.RemoveKey(n, bodyHitsPerCurve[n][h].keyFrameIndex);
                        }
                    }
                }
            }

            ReturnToPreviousScene();
        }

        /// <summary>
        /// Removes the active curve that is being worked on from the pattern. It will also remove any GameObjects
        /// for the keyframes for that curve and update the dictionary of known keyframes per curve.
        /// </summary>
        public void RemoveCurrentCurve()
        {
            // Remove it from the pattern
            HapticPattern.RemoveCurve(CurveIndex);

            // Remove the GameObjects from the scene
            for (int n = 0; n < bodyHitsPerCurve[CurveIndex].Count; n++)
            {
                DestroyImmediate(bodyHitsPerCurve[CurveIndex][n].gameObject);
            }

            // Any dictionary keys above the currentCurveIndex needs to go down by 1 since a key will be removed
            for (int n = CurveIndex; n < bodyHitsPerCurve.Keys.Count - 1; n++)
            {
                bodyHitsPerCurve[n] = bodyHitsPerCurve[n + 1];
            }

            // Remove the last one since it is now a duplicate
            bodyHitsPerCurve.Remove(bodyHitsPerCurve.Keys.Count - 1);

            // If there are no curves left, re-add a default curve so the user is still able to add points
            if (HapticPattern.curveList.Count == 0)
            {
                AddNewCurve("Default");

                // Reset the line renderer to zero
                currentCurveLineRenderer.positionCount = 0;
            }

            // Since the curve has been removed, start at 0 since there will always be a curve at 0
            CurveIndex = 0;
        }

        /// <summary>
        /// Adds an empty curve to the pattern. This will mark it as temporary so if changes are discarded,
        /// the curve will be removed as well.
        /// </summary>
        public void AddNewCurve(string patternName)
        {
            CurveIndex = HapticPattern.AddNewCurve(hapticSettings.defaultCurveSaveLocation, patternName);

            // Mark the curve as temporary so it can be deleted if not saved
            HapticPattern.curveList[CurveIndex].temporaryCurve = true;

            bodyHitsPerCurve.Add(CurveIndex, new List<BodyHitUI>());
        }

        /// <summary>
        /// Adds a curve that already has a .asset file associated with the curve
        /// </summary>
        /// <param name="hapticCurve">The curve to add</param>
        public void AddSavedCurve(ScriptableHapticCurve hapticCurve)
        {
            // Add the curve info to the pattern
            CurveIndex = HapticPattern.AddSavedCurve(hapticCurve);

            // Start tracking the BodyHitUI for this new curve index
            bodyHitsPerCurve.Add(CurveIndex, new List<BodyHitUI>());

            // Get all the curve info into the dictionary
            AddCurveToDictionary(CurveIndex, hapticCurve);
        }

        /// <summary>
        /// Updates the given curve's keyframe GameObjects to use either the active or inactive material, so that the developer
        /// knows what keyframes they are editing.
        /// </summary>
        /// <param name="curveIndex">The curve index to update</param>
        /// <param name="isActive">If true, the material is set to the active material, otherwise the inactive material is used</param>
        public void UpdateCurveKeyFrameVisualization(int curveIndex, bool isActive)
        {
            if (bodyHitsPerCurve.ContainsKey(curveIndex))
            {
                for (int n = 0; n < bodyHitsPerCurve[curveIndex].Count; n++)
                {
                    Collider collider = bodyHitsPerCurve[curveIndex][n].GetComponent<Collider>();
                    collider.enabled = isActive;

                    Renderer renderer = bodyHitsPerCurve[curveIndex][n].GetComponent<Renderer>();
                    renderer.material = (isActive) ? activeHitMaterial : inactiveHitMaterial;
                }
            }
        }

        /// <summary>
        /// Load a saved haptic pattern and set it to be the active pattern to work on
        /// </summary>
        /// <param name="loadedPattern">The pattern to load</param>
        public void LoadPattern(ScriptableHapticPattern loadedPattern)
        {
            ResetPattern();

            currentPattern = loadedPattern;

            // Load each curve information from the pattern
            for (int n = 0; n < loadedPattern.curveList.Count; n++)
            {
                bodyHitsPerCurve.Add(n, new List<BodyHitUI>());

                AddCurveToDictionary(n, loadedPattern.curveList[n]);

                UpdateCurveKeyFrameVisualization(n, false);
            }

            // Make the first curve active
            UpdateCurveKeyFrameVisualization(0, true);
        }
        #endregion

        #region PatternManagementFunctions
        private void ResetPattern()
        {
            // Go through each GameObject and destroy it
            foreach (var bodyHitList in bodyHitsPerCurve)
            {
                for (int n = 0; n < bodyHitList.Value.Count; n++)
                {
                    DestroyImmediate(bodyHitList.Value[n].gameObject);
                }
            }

            // Empty the dictionary of all the values
            bodyHitsPerCurve.Clear();

            // Reset the index to 0
            CurveIndex = 0;

            currentPattern = null;
        }

        private void AddCurveToDictionary(int curveIndex, ScriptableHapticCurve curveToAdd)
        {
            if (curveToAdd != null)
            {
                // Go through each keyframe, save the value, and place the GO representation
                for (int h = 0; h < curveToAdd.heightCurve.length; h++)
                {
                    float hitHeight = curveToAdd.heightCurve[h].value;
                    float hitAngle = curveToAdd.angleCurve[h].value;

                    BodyHitUI currentHit = SpawnBodyHitGameObject(capsuleBodyCoordinate.CalculatePositionFromBodyCoordinateHit(new BodyCoordinateHit(hitHeight, hitAngle), true),
                                                                  hitAngle,
                                                                  hitHeight,
                                                                  curveToAdd.heightCurve[h].time,
                                                                  curveIndex,
                                                                  curveToAdd.intensityCurve[h].value);

                    bodyHitsPerCurve[curveIndex].Add(currentHit);
                }
            }
        }

        private BodyHitUI SpawnBodyHitGameObject(Vector3 spawnPosition, float hitAngle, float hitHeight, float hitTime, int curveIndex, float intensity, bool isTemporary = false)
        {
            GameObject inflectionPoint = Instantiate(hitLocationPrefab, spawnPosition, Quaternion.identity, capsuleGO.transform);
            inflectionPoint.hideFlags = HideFlags.HideInHierarchy;

            BodyHitUI currentHit = inflectionPoint.GetComponent<BodyHitUI>();
            currentHit.SetValues(hitAngle,
                                 hitHeight,
                                 intensity,
                                 hitTime,
                                 HapticPattern.GetKeyCountForCurve(curveIndex) - 1,
                                 curveIndex,
                                 HapticPattern.curveList[curveIndex]);
            currentHit.isTemporary = isTemporary;

            return currentHit;
        }

        private void DrawCurrentCurvePath()
        {
            if (bodyHitsPerCurve.ContainsKey(CurveIndex))
            {
                if(bodyHitsPerCurve[CurveIndex].Count <= 1)
                {
                    return;
                }

                // Reset the line for the current current
                currentCurveLineRenderer.positionCount = 0;
                currentCurveLineRenderer.positionCount = (bodyHitsPerCurve[CurveIndex].Count - 1) * hapticSettings.defaultCurveRenderingGranularity;

                float endTime = currentPattern.curveList[CurveIndex].EndTime;
                float timeSlice = endTime / currentCurveLineRenderer.positionCount;

                // Use the curve itself to find the points around the collider
                for (int n = 0; n < currentCurveLineRenderer.positionCount; n += hapticSettings.defaultCurveRenderingGranularity)
                {
                    for(int h = 0; h < hapticSettings.defaultCurveRenderingGranularity; h++)
                    {
                        currentCurveLineRenderer.SetPosition(n+h, capsuleBodyCoordinate.CalculatePositionFromBodyCoordinateHit(currentPattern.curveList[CurveIndex].GetHitLocationAtTime((n+h) * timeSlice), true));
                    }
                }

                // Draw the final point
                currentCurveLineRenderer.SetPosition(currentCurveLineRenderer.positionCount - 1, capsuleBodyCoordinate.CalculatePositionFromBodyCoordinateHit(currentPattern.curveList[CurveIndex].GetHitLocationAtTime(endTime), true));
            }
        }
        #endregion

        #region SceneManagement
        private void OnGUIDelegate(SceneView sceneView)
        {
            // Keep the capsule selected and hide the transform gizmo
            Selection.activeGameObject = capsuleGO;
            Tools.current = Tool.None;
        }

        private void ReturnToPreviousScene()
        {
            if (!string.IsNullOrEmpty(previousScene))
            {
                EditorSceneManager.OpenScene(previousScene);
            }
            else
            {
                EditorApplication.delayCall += () =>
                {
                    // If the previous scene was not a saved one, then return to a blank scene
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                };
            }
        }

        private void CleanUp()
        {
            if (setupComplete && isClosing)
            {
                SceneView.onSceneGUIDelegate -= OnGUIDelegate;
                EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;

                if (hapticSceneView != null)
                {
                    hapticSceneView.Dispose();
                    hapticSceneView = null;
                }

                if (hitListener != null)
                {
                    hitListener.BodyPartHit -= HitListener_BodyPartHit;
                    hitListener.Dispose();
                    hitListener = null;
                }
            }
        }
        #endregion

        #region EventListeners
        private void HitListener_BodyPartHit(object sender, BodyHitListener.HapticInformation e)
        {
            float hitTime;

            if (HapticPattern.curveList[CurveIndex].heightCurve.length == 0)
            {
                hitTime = 0.0f;
            }
            else
            {
                hitTime = HapticPattern.GetCurveEndTime(CurveIndex) + hapticSettings.defaultTimeGranularity;
            }

            // Add the point to the pattern
            HapticPattern.AddKey(CurveIndex, hitTime, e.bodyHitInfo, hapticSettings.defaultIntensity);

            // Add BodyHit GO to the hit location so the user can interact with the point in the UI
            BodyHitUI currentHit = SpawnBodyHitGameObject(e.worldHitLocation,
                                                          e.bodyHitInfo.hitAngle,
                                                          e.bodyHitInfo.hitHeight,
                                                          hitTime,
                                                          CurveIndex,
                                                          hapticSettings.defaultIntensity,
                                                          true);

            bodyHitsPerCurve[CurveIndex].Add(currentHit);

            // Update the line renderer
            DrawCurrentCurvePath();
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorApplication.isPlaying = false;
            }
        }
        #endregion

        #region UnityFunctions
        private void Awake()
        {
            bodyHitsPerCurve = new Dictionary<int, List<BodyHitUI>>();

            // Don't allow Play Mode when this script is in the scene
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            hapticSettings = ScriptableHapticSettings.GetSettings();
        }

        private void Update()
        {
            if (setupComplete)
            {
                if (hapticSceneView == null)
                {
                    Debug.LogError("Lost connection to the SceneView, please re-open the Haptic Pattern Creation window.", this);

                    isClosing = true;
                }
            }
        }

        private void OnDestroy()
        {
            CleanUp();
        }
        #endregion
    }
}
#endif