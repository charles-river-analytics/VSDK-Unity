using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Links to all the active Haptic Devices that are in the scene. When that haptic device plays, it will
    /// display an indication of the location on the effected body part. This script assumes that the body
    /// coordinates are located below it in the hierarchy.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public sealed class HapticVisualizer : MonoBehaviour
    {
        #region PublicVariables
        public float timeToFadeOut = 1.0f;
        public Mesh hapticPatternVisualMesh;
        public Material hapticPatternVisualMaterial;
        #endregion

        #region PrivateVariables
        private HapticManager manager;
        private Dictionary<HumanBodyBones, BodyCoordinate> bodyPartToCoordinate;

        private const float MESH_SCALE = .05f;
        #endregion

        #region EventHandler
        private void HapticDevice_HapticFeedbackPlayed(object sender, HapticFeedbackEventArgs e)
        {
            if (bodyPartToCoordinate.ContainsKey(e.bodyPart))
            {
                // Calculate the position of the hit
                Vector3 hitPosition = bodyPartToCoordinate[e.bodyPart].CalculatePositionFromBodyCoordinateHit(e.hitLocation);

                Matrix4x4 matrixTRS = new Matrix4x4();
                matrixTRS.SetTRS(hitPosition, Quaternion.identity, new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE));

                Graphics.DrawMesh(hapticPatternVisualMesh, matrixTRS, hapticPatternVisualMaterial, 0);
            }
        }
        #endregion

        #region Unity Functions
        void Start()
        {
            manager = FindObjectOfType<HapticManager>();

            // Hook into every device's event in the scene to know when they play a haptic pulse
            foreach (HapticDevice hapticDevice in manager.GetSetOfActiveDevices())
            {
                hapticDevice.HapticFeedbackPlayed += HapticDevice_HapticFeedbackPlayed;
            }

            // Grab all the body coordinates below this script in the hierarchy and save a reference to it
            bodyPartToCoordinate = new Dictionary<HumanBodyBones, BodyCoordinate>();

            foreach (BodyCoordinate bodyCoordinate in GetComponentsInChildren<BodyCoordinate>())
            {
                if(!bodyPartToCoordinate.ContainsKey(bodyCoordinate.attachedBody))
                {
                    bodyPartToCoordinate.Add(bodyCoordinate.attachedBody, bodyCoordinate);
                }
            }
        }

        private void OnApplicationQuit()
        {
            foreach (HapticDevice hapticDevice in manager.GetSetOfActiveDevices())
            {
                hapticDevice.HapticFeedbackPlayed -= HapticDevice_HapticFeedbackPlayed;
            }
        }
        #endregion
    }
}