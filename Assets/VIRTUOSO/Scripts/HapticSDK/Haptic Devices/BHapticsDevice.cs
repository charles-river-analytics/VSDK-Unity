#if VRTK_DEFINE_SDK_BHAPTICS
using Bhaptics.Tact.Unity;
using System;
#endif
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// BHaptics Haptic Device. The BHaptics has several actuators on their devices that can affect
    /// multiple body parts. This script can target any body part with the position variable. Additionally,
    /// a mapping needs to be provided that lets a specific physical actuator know which body coordinate space
    /// they must react to.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
#if VRTK_DEFINE_SDK_BHAPTICS
    [HapticSystem("Bhaptics", "TACTAL", "BodyCoordinates/Head", Pos.Head, "DeviceMappings/BHapticsFaceMapping")]
    [HapticSystem("Bhaptics", "TACTOT Front", "BodyCoordinates/ChestFront", Pos.VestFront, "DeviceMappings/BHapticsFrontVestMapping")]
    [HapticSystem("Bhaptics", "TACTOT Back", "BodyCoordinates/ChestBack", Pos.VestBack, "DeviceMappings/BHapticsBacktVestMapping")]
    [HapticSystem("Bhaptics", "Left TACTOSY", "BodyCoordinates/LeftForearm", Pos.LeftArm, "DeviceMappings/BHapticsLeftWristMapping")]
    [HapticSystem("Bhaptics", "Right TACTOSY", "BodyCoordinates/RightForearm", Pos.RightArm, "DeviceMappings/BHapticsRightWristMapping")]
    [HapticSystem("Bhaptics", "Left TACTOSY Hand", "BodyCoordinates/LeftHand", Pos.LeftHand)]
    [HapticSystem("Bhaptics", "Right TACTOSY Hand", "BodyCoordinates/RightHand", Pos.RightHand)]
    [HapticSystem("Bhaptics", "Left TACTOSY Foot", "BodyCoordinates/LeftFoot", Pos.LeftFoot)]
    [HapticSystem("Bhaptics", "Right TACTOSY Foot", "BodyCoordinates/RightFoot", Pos.RightFoot)]
#endif
    public class BHapticsDevice : HapticDevice
    {
        #region PublicVariables
#if VRTK_DEFINE_SDK_BHAPTICS
        [Tooltip("The position tag for where the device is attached to.")]
        public Pos devicePosition;
#endif
        public ScriptableDeviceMapping bhapticsMapping;
        #endregion

        #region PrivateVariables
#if VRTK_DEFINE_SDK_BHAPTICS
        private TactSource tactSource;
#endif
        private const int HAPTIC_ACTUATOR_COUNT = 20;
        // The DotPoint array that will be filled to play the haptic patterns
        private byte[] hapticBytes = new byte[HAPTIC_ACTUATOR_COUNT];
        #endregion

        #region HapticDeviceImplementation
        protected override void CancelHaptics()
        {
#if VRTK_DEFINE_SDK_BHAPTICS
            tactSource.Stop();
#endif
            // Clear the array
            hapticBytes = new byte[HAPTIC_ACTUATOR_COUNT];
        }

        protected override void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
            int hitIndex = affectedBodyPart.BodyCoordinateHitIndex(bodyPart, hitLocation);

            // If -1 is returned, then the hitLocation is not in the CoordinateSpace that the device is attached to
            if (hitIndex == -1)
            {
                return;
            }

            // Assign the intensity value for each of the mapped effector
            for (int n = 0; n < bhapticsMapping.mapping[hitIndex].indexMapping.Length; n++)
            {
                // The intensity needs to be between 0 and 100, so convert the intensity to that range
                hapticBytes[bhapticsMapping.mapping[hitIndex].indexMapping[n]] = (byte)(intensity * 100);
            }

            // Assign the intensity values and play
#if VRTK_DEFINE_SDK_BHAPTICS
            tactSource.DotPoints = hapticBytes;
            tactSource.Play();
#endif

            // Clear the array since it will need different values on the next frame
            hapticBytes = new byte[HAPTIC_ACTUATOR_COUNT];
        }

        public override void ApplyDefaultData(HapticSystemAttribute hapticSystemInfo)
        {
            base.ApplyDefaultData(hapticSystemInfo);

#if VRTK_DEFINE_SDK_BHAPTICS
            devicePosition = (Pos) Enum.Parse(typeof(Pos), hapticSystemInfo.AdditionalData[0].ToString());
#endif

            if (hapticSystemInfo.AdditionalData.Length > 1)
            {
                ScriptableDeviceMapping mapping = Resources.Load<ScriptableDeviceMapping>(hapticSystemInfo.AdditionalData[1] as string);

                if (mapping != null)
                {
                    bhapticsMapping = mapping;
                }
            }
        }
        #endregion

        #region Unity Functions
        protected override void Start()
        {
            base.Start();

#if VRTK_DEFINE_SDK_BHAPTICS
            tactSource = gameObject.AddComponent<TactSource>();

            tactSource.FeedbackType = FeedbackType.DotMode;
            tactSource.Position = devicePosition;
            tactSource.TimeMillis = hapticDuration;
#endif
        }
        #endregion
    }
}