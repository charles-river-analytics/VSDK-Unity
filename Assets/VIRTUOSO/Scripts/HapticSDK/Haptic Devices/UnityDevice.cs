using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
    using UnityEngine.VR;
    using XRNode = UnityEngine.VR.VRNode;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Haptic device for Unity XR input devices. To use, first make sure a scene has a Haptic Manager.
    /// Then, set up two objects with UnityDevice scripts (one for each controller). While not required,
    /// attaching these to the SDK Unity Controller Tracker objects is preferred.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) Updated: September 2019
    /// </summary>
    [HapticSystem("Unity", "Left Controller", "BodyCoordinates/LeftHand", typeof(SDK_UnityController), false)]
    [HapticSystem("Unity", "Right Controller", "BodyCoordinates/RightHand", typeof(SDK_UnityController), true)]
    public class UnityDevice : HapticDevice
    {
        #region Protected Variables
        // Indicates which hand this object corresponds to
        protected XRNode nodeType;
        // Holds a reference to the Unity XR device for haptic feedback
        protected InputDevice hapticDevice;

        // By default, this is set to zero
        protected uint hapticChannel = 0;
        #endregion

        #region Haptic Device Behavior
        protected override void CancelHaptics()
        {
            if(hapticDevice.isValid)
            {
                hapticDevice.StopHaptics();
            }
            else
            {
                Debug.LogWarningFormat("Unity XR Haptic Device {0} failed to cancel: device is invalid", nodeType.ToString());
            }
        }

        protected override void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
            if(hapticDevice.isValid)
            {
                bool successful = hapticDevice.SendHapticImpulse(hapticChannel, intensity, hapticDuration);
                if(! successful)
                {
                    Debug.LogWarningFormat("Unity XR Haptic Device {0} failed to play haptics");
                }
            }
            else
            {
                Debug.LogWarningFormat("Unity XR Haptic Device for the {0} failed to play haptics: device is invalid", nodeType.ToString());
            }
        }

        /// <summary>
        /// This override is used to determine the correct XRNode type for this device.
        /// </summary>
        public override void ApplyDefaultData(HapticSystemAttribute hapticSystemInfo)
        {
            base.ApplyDefaultData(hapticSystemInfo);

            nodeType = (bool)hapticSystemInfo.AdditionalData[0] ? XRNode.RightHand : XRNode.LeftHand;
        }

        /// <summary>
        /// This override is called when the VRTK SDK is loaded. This is used here to fetch the Input Device handle for the appropriate controller.
        /// </summary>
        protected override void Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (e.currentSetup.controllerSDKInfo.type == hapticSystemInfo.ConnectedSDKType)
            {
                hapticDevice = InputDevices.GetDeviceAtXRNode(nodeType);
            }
        }
        #endregion
    }
}
