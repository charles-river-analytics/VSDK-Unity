using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CharlesRiverAnalytics.Virtuoso.Gestures
{
    /// <summary>
    /// Enables editing of AdvancedGestures which otherwise would be impossible since it uses subclasses to define behavior of the states.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) October 2018, Updated January 2019
    /// </summary>
    [CustomEditor(typeof(AdvancedGesture))]
    public class AdvancedGestureEditor : Editor
    {
        #region Control Variables
        protected AdvancedGestureState stateToDelete;
        protected AdvancedGestureCondition conditionToDelete;
        // reasonable width for enum fields in the editor so they don't stretch/contract too much
        protected static float ENUM_WIDTH = 128;
        #endregion

        #region Inspector Drawing Code
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Advanced Gesture States", EditorStyles.boldLabel);
            // do not draw the control variables
            AdvancedGesture advTarget = (AdvancedGesture)target;
            advTarget.gestureCooldown = EditorGUILayout.FloatField("Cooldown (seconds)", advTarget.gestureCooldown);
            advTarget.allowedWhileHoldingObjects = EditorGUILayout.Toggle("Allow While Holding", advTarget.allowedWhileHoldingObjects);

            foreach (AdvancedGestureState advState in advTarget.advancedGestureStateList)
            {
                DrawAdvancedGestureState(advState);
            }

            if (GUILayout.Button("Add Gesture State"))
            {
                advTarget.advancedGestureStateList.Add(new AdvancedGestureState());
            }

            if (stateToDelete != null)
            {
                advTarget.advancedGestureStateList.Remove(stateToDelete);
                stateToDelete = null;
            }
            // set dirty to ensure the object is saved
            EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// Draws the inspector for a single AdvancedGestureState
        /// </summary>
        protected void DrawAdvancedGestureState(AdvancedGestureState advState)
        {
            GUILayout.BeginHorizontal();
            advState.gestureName = GUILayout.TextField(advState.gestureName);
            if (GUILayout.Button("Remove Gesture State"))
            {
                stateToDelete = advState;
            }
            GUILayout.EndHorizontal();
            advState.coreGesture = (Gesture)EditorGUILayout.ObjectField("Core Gesture", advState.coreGesture, typeof(Gesture), false);
            advState.holdTime = EditorGUILayout.Slider("Gesture Hold Time", advState.holdTime, 0.0f, 5.0f);

            foreach (AdvancedGestureCondition condition in advState.AdvancedGestureConditionList)
            {
                DrawAdvancedGestureCondition(condition);
            }

            GUILayout.BeginHorizontal();
            advState.editorNewConditionType = (AdvancedGestureState.AdvancedConditionType)EditorGUILayout.EnumPopup(advState.editorNewConditionType, GUILayout.Width(ENUM_WIDTH));
            if (GUILayout.Button("Add"))
            {
                switch (advState.editorNewConditionType)
                {
                    case AdvancedGestureState.AdvancedConditionType.Hold:
                        {
                            advState.holdConditionList.Add(new HoldGestureCondition());
                            break;
                        }
                    case AdvancedGestureState.AdvancedConditionType.Movement:
                        {
                            advState.movementConditionList.Add(new MovementGestureCondition());
                            break;
                        }
                    case AdvancedGestureState.AdvancedConditionType.PalmAngle:
                        {
                            advState.palmConditions.Add(new PalmNormalCondition());
                            break;
                        }
                    case AdvancedGestureState.AdvancedConditionType.SimultaneousAction:
                        {
                            advState.simultaneousGestureConditionList.Add(new SimultaneousGesture());
                            break;
                        }
                }
            }
            GUILayout.EndHorizontal();

            if (conditionToDelete != null)
            {
                if (conditionToDelete is HoldGestureCondition)
                {
                    advState.holdConditionList.Remove((HoldGestureCondition)conditionToDelete);
                }
                else if (conditionToDelete is MovementGestureCondition)
                {
                    advState.movementConditionList.Remove((MovementGestureCondition)conditionToDelete);
                }
                else if (conditionToDelete is PalmNormalCondition)
                {
                    advState.palmConditions.Remove((PalmNormalCondition)conditionToDelete);
                }
                else if (conditionToDelete is SimultaneousGesture)
                {
                    advState.simultaneousGestureConditionList.Remove((SimultaneousGesture)conditionToDelete);
                }
                conditionToDelete = null;
            }
        }
        /// <summary>
        /// Draws the inspector for an advanced gesture condition
        /// </summary>
        protected void DrawAdvancedGestureCondition(AdvancedGestureCondition condition)
        {
            GUILayout.BeginHorizontal();
            string header = condition.GetType().Name;
            GUILayout.Label(header, EditorStyles.boldLabel);
            if (GUILayout.Button("Remove"))
            {
                conditionToDelete = condition;
            }
            GUILayout.EndHorizontal();

            if (condition is MovementGestureCondition)
            {
                MovementGestureCondition mvtCondition = (MovementGestureCondition)condition;
                GUILayout.BeginHorizontal();
                mvtCondition.distanceFromPreviousGesture = EditorGUILayout.Slider("Distance from last gesture", mvtCondition.distanceFromPreviousGesture, 0.0f, 1.5f);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                mvtCondition.distanceOperator = (MovementGestureCondition.MovementOperator)EditorGUILayout.EnumPopup("Comparison Operator", mvtCondition.distanceOperator);
                if (mvtCondition.distanceOperator == MovementGestureCondition.MovementOperator.EqualTo)
                {
                    mvtCondition.distanceTolerance = EditorGUILayout.Slider("Tolerance", mvtCondition.distanceTolerance, 0.0f, 0.2f);
                }
                GUILayout.EndHorizontal();
            }
            else if (condition is HoldGestureCondition)
            {
                HoldGestureCondition holdCondition = (HoldGestureCondition)condition;
                holdCondition.gestureHoldTime = EditorGUILayout.FloatField("Hold Time", holdCondition.gestureHoldTime);
            }
            else if (condition is PalmNormalCondition)
            {
                PalmNormalCondition palmCondition = (PalmNormalCondition)condition;
                GUILayout.BeginHorizontal();
                palmCondition.otherVectorToUse = (PalmNormalCondition.OtherVector)EditorGUILayout.EnumPopup("Other Vector", palmCondition.otherVectorToUse);
                palmCondition.otherVectorDirection = (PalmNormalCondition.VectorType)EditorGUILayout.EnumPopup("Other Vector Direction", palmCondition.otherVectorDirection);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                palmCondition.eulerRotationFromOtherVector = EditorGUILayout.Vector3Field("Rotation of Other", palmCondition.eulerRotationFromOtherVector);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                palmCondition.tolerance = EditorGUILayout.Slider("Tolerance", palmCondition.tolerance, 0.0f, 1.0f);
                GUILayout.EndHorizontal();
            }
            else if (condition is SimultaneousGesture)
            {
                SimultaneousGesture simulCondition = (SimultaneousGesture)condition;
                simulCondition.simultaneousGesture = (Gesture)EditorGUILayout.ObjectField("Other Gesture", simulCondition.simultaneousGesture, typeof(Gesture), false);
            }
        }
    }
    #endregion
}
