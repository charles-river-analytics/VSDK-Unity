using CharlesRiverAnalytics.Virtuoso.Utilities;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Reaction to rotates the current camera as defined by VRTK around the world's y-axis. The developer is able
    /// to specify how much rotation should be applied in the editor.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class CameraRotationReaction : GenericReaction
    {
        #region PublicVariables
        [Tooltip("The amount of rotation (in degrees) to apply to the world's y-axis around the camera."), Range(-180, 180)]
        public float rotationAmount;
        #endregion

        #region PrivateVariables
        private const float DELAY_BETWEEN_ROTATIONS = 0.2f;
        private float timeOfLastRotation = 0;
        private Transform cameraTransform;
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            if(cameraTransform == null)
            {
                cameraTransform = VRTK_DeviceFinder.HeadsetCamera();
            }

            if((Time.time - timeOfLastRotation) > DELAY_BETWEEN_ROTATIONS)
            {
                // You can't rotate the headset directly since it is controlled by the device, so rotate the parent instead
                cameraTransform?.parent.RotateAround(cameraTransform.position, Vector3.up, rotationAmount);

                timeOfLastRotation = Time.time;
            }
        }
        #endregion

        #region UnityFunctions
        private void OnEnable()
        {
            // Since the HMD won't be available until after the SDK is set up, make sure this is enabled after this happens
            cameraTransform = VRTK_DeviceFinder.HeadsetCamera();
        }
        #endregion
    }
}