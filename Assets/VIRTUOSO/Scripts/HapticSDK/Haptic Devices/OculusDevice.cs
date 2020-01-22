using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Haptic device for the Oculus controllers. Each has a single actuator.
    /// To use this script, attach it to each controller.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) May 2019
    /// </summary>
#if VRTK_DEFINE_SDK_OCULUS
    [HapticSystem("Oculus", "Left Controller", "BodyCoordinates/LeftHand", typeof(SDK_OculusController), false)]
    [HapticSystem("Oculus", "Right Controller", "BodyCoordinates/RightHand", typeof(SDK_OculusController), true)]
#endif
    public class OculusDevice : HapticDevice
    {
        #region Public Variables
        public bool isLeftController;
        #endregion

        #region Control Variables
#if VRTK_DEFINE_SDK_OCULUS
        // used to convert incoming pattern into something OVR can understand
        protected OVRHapticsClip hapticsClipLeft;
        // used to convert incoming pattern into something OVR can understand
        protected OVRHapticsClip hapticsClipRight;
#endif
        #endregion

        protected override void CancelHaptics()
        {
#if VRTK_DEFINE_SDK_OCULUS
            if (isLeftController)
            {
                OVRHaptics.LeftChannel.Clear();
            }
            else
            {
                OVRHaptics.RightChannel.Clear();
            }
#endif
        }

        protected override void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
#if VRTK_DEFINE_SDK_OCULUS

            if(hapticsClipLeft == null || hapticsClipRight == null)
            {
                OVRHaptics.Config.Load();
                hapticsClipLeft = new OVRHapticsClip();
                hapticsClipRight = new OVRHapticsClip();
            }

            int clipLengthBytes = 
                Mathf.Min(
                    Mathf.Max(OVRHaptics.Config.SampleRateHz * hapticDuration, OVRHaptics.Config.MinimumBufferSamplesCount), 
                    OVRHaptics.Config.MaximumBufferSamplesCount
                    );

            if (isLeftController)
            {
                hapticsClipLeft.Reset();
                for (int i = 0; i < clipLengthBytes; i++)
                {
                    hapticsClipLeft.WriteSample((byte)(intensity * byte.MaxValue));
                }
                OVRHaptics.LeftChannel.Preempt(hapticsClipLeft);
            }
            else
            {
                hapticsClipRight.Reset();
                for (int i = 0; i < clipLengthBytes; i++)
                {
                    hapticsClipRight.WriteSample((byte)(intensity * byte.MaxValue));
                }
                OVRHaptics.RightChannel.Preempt(hapticsClipRight);
            }
#endif
        }

        public override void ApplyDefaultData(HapticSystemAttribute hapticSystemInfo)
        {
            base.ApplyDefaultData(hapticSystemInfo);

            isLeftController = !(bool)hapticSystemInfo.AdditionalData[0];
        }

        protected override void Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
#if VRTK_DEFINE_SDK_OCULUS
            if (hapticSystemInfo != null && e.currentSetup != null && e.currentSetup.controllerSDKInfo.type == hapticSystemInfo.ConnectedSDKType)
            {
                OVRHaptics.Config.Load();
                hapticsClipLeft = new OVRHapticsClip();
                hapticsClipRight = new OVRHapticsClip();
            }
#endif
        }
    }
}