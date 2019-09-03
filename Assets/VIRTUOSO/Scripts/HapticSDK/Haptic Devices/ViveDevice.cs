using UnityEngine;
using VRTK;
using CharlesRiverAnalytics.Virtuoso.Utilities;
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
using Valve.VR;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Haptic device for the Vive controllers. A controller has a single actuator so it only has to
    /// activate the vibration motor on that controller. Use this script on each controller.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
#if VRTK_DEFINE_STEAMVR_PLUGIN_1_2_1_OR_NEWER || VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
    [HapticSystem("Vive", "Left Controller", "BodyCoordinates/LeftHand", typeof(SDK_SteamVRController), false)]
    [HapticSystem("Vive", "Right Controller", "BodyCoordinates/RightHand", typeof(SDK_SteamVRController), true)]
#endif
    public class ViveDevice : HapticDevice
    {
        #region PublicVariables
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
        public SteamVR_Action_Vibration vibration;
#endif
        [Range(0, 320)]
        ///How often the haptic motor should bounce (0 - 320 in hz)
        public float frequency = 100.0f;
        #endregion

        #region PrivateVariables
        [SerializeField]
        private bool isRightController;
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
        private SteamVR_Behaviour_Pose trackedObject;
#elif VRTK_DEFINE_STEAMVR_PLUGIN_1_2_1_OR_NEWER && ! VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
        // The max timing of a haptic pulse in microseconds for the controller, this also determines the max intensity
        private const float VIVE_VIBRATION_VALUE = 3999;
        private SteamVR_TrackedObject trackedObject;
#endif
        #endregion

        #region HapticDeviceOverride
        protected override void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER && VRTK_DEFINE_STEAMVR_INPUT_COMPILED
            vibration.Execute(0.0f, hapticDuration / Constants.MS_TO_SECONDS, frequency, intensity, trackedObject.inputSource);
#elif VRTK_DEFINE_STEAMVR_PLUGIN_1_2_1_OR_NEWER
            SteamVR_Controller.Input((int)trackedObject.index).TriggerHapticPulse((ushort)(intensity * VIVE_VIBRATION_VALUE));
#endif
        }

        protected override void CancelHaptics()
        {
            // No-op
        }

        public override void ApplyDefaultData(HapticSystemAttribute hapticSystemInfo)
        {
            base.ApplyDefaultData(hapticSystemInfo);

            isRightController = (bool)hapticSystemInfo.AdditionalData[0];
        }

        protected override void Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (e.currentSetup.controllerSDKInfo.type == hapticSystemInfo.ConnectedSDKType)
            {
                GameObject controllerGameObject;

                if (isRightController)
                {
                    controllerGameObject = e.currentSetup.controllerSDK.GetControllerRightHand(true);
                }
                else
                {
                    controllerGameObject = e.currentSetup.controllerSDK.GetControllerLeftHand(true);
                }


                if (controllerGameObject != null)
                {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER && VRTK_DEFINE_STEAMVR_INPUT_COMPILED
                    trackedObject = controllerGameObject.GetComponent<SteamVR_Behaviour_Pose>();
                    vibration = SteamVR_Actions.naturalistic_Haptic;
#elif VRTK_DEFINE_STEAMVR_PLUGIN_1_2_1_OR_NEWER
                    trackedObject = controllerGameObject.GetComponent<SteamVR_TrackedObject>();
#endif
                }

            }
        }
        #endregion
    }
}