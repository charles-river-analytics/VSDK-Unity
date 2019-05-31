using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Haptic device for the Sense Gloves. It handles the actuators for a single hand so each
    /// hand will need one of these scripts and a body coordinate system for each finger.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
#if VRTK_DEFINE_SDK_SENSE_GLOVE
    [HapticSystem("SenseGlove", "Left Glove", "BodyCoordinates/Finger_LeftThumb", typeof(SDK_SenseGloveHand),
                                              "BodyCoordinates/Finger_LeftIndex",
                                              "BodyCoordinates/Finger_LeftMiddle",
                                              "BodyCoordinates/Finger_LeftRing",
                                              "BodyCoordinates/Finger_LeftPinky",
                                              false)]
    [HapticSystem("SenseGlove", "Right Glove", "BodyCoordinates/Finger_RightThumb", typeof(SDK_SenseGloveHand),
                                              "BodyCoordinates/Finger_RightIndex",
                                              "BodyCoordinates/Finger_RightMiddle",
                                              "BodyCoordinates/Finger_RightRing",
                                              "BodyCoordinates/Finger_RightPinky",
                                              true)]
#endif
    public class SenseGloveDevice : HapticDevice
    {
        #region PublicFingerCoordinates
        public ScriptableBodyCoordinate indexFingerCoordinates;
        public ScriptableBodyCoordinate middleFingerCoordinates;
        public ScriptableBodyCoordinate ringFingerCoordinates;
        public ScriptableBodyCoordinate pinkyFingerCoordinates;
        #endregion

        #region PrivateVariables
        [SerializeField]
        private bool isRightHand;
        // Describes each finger from thumb to pinky
        private bool[] fingers = new bool[5];
        private int[] intensities = new int[5];
        private int[] duration = new int[5];
#if VRTK_DEFINE_SDK_SENSE_GLOVE
        private SenseGlove_Object glove;
#endif
        #endregion

        #region HapticDeviceImplementation
        protected override void CancelHaptics()
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            glove.StopBuzzMotors();

            fingers = new bool[5];
            intensities = new int[5];
            duration = new int[5];
#endif
        }

        protected override void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
            int fingerIndex = FindHitFingerIndex(bodyPart);

            if (fingerIndex != -1)
            {
                fingers[fingerIndex] = true;
                intensities[fingerIndex] = (int)(intensity * 100);
                duration[fingerIndex] = hapticDuration;
            }

            glove.SendBuzzCmd(fingers, intensities, duration);
#endif
        }

        public override void ApplyDefaultData(HapticSystemAttribute hapticSystemInfo)
        {
            base.ApplyDefaultData(hapticSystemInfo);

            indexFingerCoordinates = Resources.Load<ScriptableBodyCoordinate>(hapticSystemInfo.AdditionalData[0] as string);
            middleFingerCoordinates = Resources.Load<ScriptableBodyCoordinate>(hapticSystemInfo.AdditionalData[1] as string);
            ringFingerCoordinates = Resources.Load<ScriptableBodyCoordinate>(hapticSystemInfo.AdditionalData[2] as string);
            pinkyFingerCoordinates = Resources.Load<ScriptableBodyCoordinate>(hapticSystemInfo.AdditionalData[3] as string);

            isRightHand = (bool)hapticSystemInfo.AdditionalData[4];
        }

        protected override void Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (e.currentSetup.handSDKInfo.type == hapticSystemInfo.ConnectedSDKType)
            {
#if VRTK_DEFINE_SDK_SENSE_GLOVE
                if (isRightHand)
                {
                    glove = e.currentSetup.handSDK.GetRightHand().GetComponent<SenseGlove_Object>();
                }
                else
                {
                    glove = e.currentSetup.handSDK.GetLeftHand().GetComponent<SenseGlove_Object>();
                }
#endif
            }
        }
        #endregion

        #region SenseGloveMethods
        /// <summary>
        /// Provides the index of the finger that was hit based on the assigned coordinate system
        /// </summary>
        /// <param name="bodyPart">The body part that was hit</param>
        /// <returns>The index that represents the finger that was hit (0 - thumb to 4 - Pinky)</returns>
        private int FindHitFingerIndex(HumanBodyBones bodyPart)
        {
            if (affectedBodyPart?.affectableBodyParts == bodyPart)
            {
                return 0;
            }
            else if (indexFingerCoordinates?.affectableBodyParts == bodyPart)
            {
                return 1;
            }
            else if (middleFingerCoordinates?.affectableBodyParts == bodyPart)
            {
                return 2;
            }
            else if (ringFingerCoordinates?.affectableBodyParts == bodyPart)
            {
                return 3;
            }
            else if (pinkyFingerCoordinates?.affectableBodyParts == bodyPart)
            {
                return 4;
            }

            return -1;
        }
        #endregion

        #region Unity Functions
        protected override void Start()
        {
            base.Start();

            if (manager != null)
            {
                manager.AddDevicePerBodyLocation(this, indexFingerCoordinates);
                manager.AddDevicePerBodyLocation(this, middleFingerCoordinates);
                manager.AddDevicePerBodyLocation(this, ringFingerCoordinates);
                manager.AddDevicePerBodyLocation(this, pinkyFingerCoordinates);
            }
        }
        #endregion
    }
}