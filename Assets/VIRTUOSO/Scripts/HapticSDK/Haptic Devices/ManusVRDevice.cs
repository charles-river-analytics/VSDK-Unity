using UnityEngine;
using VRTK;
#if VRTK_DEFINE_SDK_MANUS_VR
using Assets.ManusVR.Scripts;
#endif

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Haptic device for the Manus VR Gloves. The Manus VR gloves have one actuator on each hand, so this
    /// script must be used with each hand as well.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
#if VRTK_DEFINE_SDK_MANUS_VR
    [HapticSystem("ManusVR", "Left Glove", "BodyCoordinates/LeftHand", typeof(SDK_ManusVRHand), false)]
    [HapticSystem("ManusVR", "Right Glove", "BodyCoordinates/RightHand", typeof(SDK_ManusVRHand), true)]
#endif
    public class ManusVRDevice : HapticDevice
    {
        #region PublicVariables
        [Tooltip("The side the glove is on.")]
        public bool isRightGlove;
        #endregion

        #region PrivateVariables
#if VRTK_DEFINE_SDK_MANUS_VR
        private HandData handData;
#endif
        #endregion

        #region HapticDeviceImplementation
        protected override void CancelHaptics()
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            Manus.ManusSetVibration(handData.Session,
                                    isRightGlove ? device_type_t.GLOVE_RIGHT : device_type_t.GLOVE_LEFT,
                                    0,
                                    (ushort)hapticDuration);
#endif
        }

        protected override void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
#if VRTK_DEFINE_SDK_MANUS_VR
            Manus.ManusSetVibration(handData.Session,
                                    isRightGlove ? device_type_t.GLOVE_RIGHT : device_type_t.GLOVE_LEFT, 
                                    intensity,
                                    (ushort)hapticDuration);
#endif
        }

        public override void ApplyDefaultData(HapticSystemAttribute hapticSystemInfo)
        {
            base.ApplyDefaultData(hapticSystemInfo);

            isRightGlove = (bool)hapticSystemInfo.AdditionalData[0];
        }

        protected override void Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (e.currentSetup.handSDKInfo.type == hapticSystemInfo.ConnectedSDKType)
            {
#if VRTK_DEFINE_SDK_MANUS_VR
                handData = e.currentSetup.handSDK.GetHandController().GetComponent<HandData>();
#endif
            }
        }
        #endregion
    }
}