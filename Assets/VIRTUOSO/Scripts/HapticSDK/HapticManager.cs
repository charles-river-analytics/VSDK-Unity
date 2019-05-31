using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Manages all the haptic devices that have an affect on the Body Coordinates in the scene.
    /// 
    /// If attached to a class with an Animator, it will also automatically set up the body parts on any
    /// transforms that have a BodyCoordinate attached to them.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class HapticManager : MonoBehaviour
    {
        #region PrivateVariables
        private Animator animator;
        private Dictionary<HumanBodyBones, List<HapticDevice>> bodyAffectedByDevice;
        // Body parts in this set will not receive haptics
        private HashSet<HumanBodyBones> ignoreBoneSet;
        // Devices in this set will get haptic patterns
        private HashSet<HapticDevice> ignoreHapticDevices;
        #endregion

        #region PublicAPI
        /// <summary>
        /// Checks to see if the given body part that was hit has a device that should be triggered. If it does,
        /// it tells that device to play their haptic for that hit location.
        /// </summary>
        /// <param name="bodyLocation">The body part that was hit</param>
        /// <param name="hitLocation">The height/angle of the hit on the body part</param>
        public void BodyPartHit(HumanBodyBones bodyLocation, BodyCoordinateHit hitLocation, ScriptableHapticPattern hapticPattern = null)
        {
            // Check to make sure this body part isn't ignored
            if (ignoreBoneSet.Contains(bodyLocation))
            {
                return;
            }

            if (bodyAffectedByDevice.ContainsKey(bodyLocation))
            {
                // Tell every device that has this body part to trigger the haptics at the given location
                for (int n = 0; n < bodyAffectedByDevice[bodyLocation].Count; n++)
                {
                    // Check to make sure this haptic device isn't ignored
                    if(ignoreHapticDevices.Contains(bodyAffectedByDevice[bodyLocation][n]))
                    {
                        return;
                    }

                    if (hapticPattern == null)
                    {
                        bodyAffectedByDevice[bodyLocation][n].TriggerDevice(bodyLocation, hitLocation, 0.5f);
                    }
                    else
                    {
                        bodyAffectedByDevice[bodyLocation][n].TriggerDevice(bodyLocation, hitLocation, hapticPattern);
                    }
                }
            }
        }

        /// <summary>
        /// Allows the manager to know that a device is able to target a specific body part
        /// </summary>
        /// <param name="device">The haptic device that is able to target a body coordinate</param>
        /// <param name="hitLocation">The body coordinate system that it can handle</param>
        public void AddDevicePerBodyLocation(HapticDevice device, ScriptableBodyCoordinate hitLocation)
        {
            if (bodyAffectedByDevice.ContainsKey(hitLocation.affectableBodyParts))
            {
                bodyAffectedByDevice[hitLocation.affectableBodyParts].Add(device);
            }
            else
            {
                bodyAffectedByDevice[hitLocation.affectableBodyParts] = new List<HapticDevice>
                {
                    device
                };
            }
        }

        /// <summary>
        /// Plays a specified haptic pattern on a specific body part. Use this method if one
        /// simply needs to give a notification to a body part through scripting.
        /// </summary>
        /// <param name="bodyLocation">The body part to notify</param>
        /// <param name="pattern">The haptic pattern to play</param>
        public void PlayPattern(HumanBodyBones bodyLocation, ScriptableHapticPattern pattern)
        {
            // Check to make sure this body part isn't ignored
            if (ignoreBoneSet.Contains(bodyLocation))
            {
                return;
            }

            if (bodyAffectedByDevice.ContainsKey(bodyLocation))
            {
                // Tell every device that has this body part to play the pattern
                for (int n = 0; n < bodyAffectedByDevice[bodyLocation].Count; n++)
                {
                    // Check to make sure this haptic device isn't ignored
                    if (ignoreHapticDevices.Contains(bodyAffectedByDevice[bodyLocation][n]))
                    {
                        return;
                    }

                    bodyAffectedByDevice[bodyLocation][n].PlayPattern(bodyLocation, pattern);
                }
            }
        }

        /// <summary>
        /// Stops any patterns that are playing on the given body part.
        /// </summary>
        public void CancelPatternPlayingOnBody(HumanBodyBones bodyLocation)
        {
            if (bodyAffectedByDevice.ContainsKey(bodyLocation))
            {
                // Tell every device that has this body part to stop
                for (int n = 0; n < bodyAffectedByDevice[bodyLocation].Count; n++)
                {
                    bodyAffectedByDevice[bodyLocation][n].StopPattern(bodyLocation);
                }
            }
        }

        /// <summary>
        /// Used to retrieve every unique haptic device that the manager
        /// </summary>
        /// <returns>A HashSet of every haptic device that the manager is aware of.</returns>
        public HashSet<HapticDevice> GetSetOfActiveDevices()
        {
            HashSet<HapticDevice> deviceSet = new HashSet<HapticDevice>();

            // Go through every entry in the body to device dictionary and add them to the hashset
            foreach (List<HapticDevice> deviceList in bodyAffectedByDevice.Values)
            {
                foreach (HapticDevice device in deviceList)
                {
                    deviceSet.Add(device);
                }
            }

            return deviceSet;
        }

        public void IgnoreHapticsOnBodyPart(HumanBodyBones bodyPartToIgnore)
        {
            ignoreBoneSet.Add(bodyPartToIgnore);

            CancelPatternPlayingOnBody(bodyPartToIgnore);
        }

        public void ReconsiderHapticsOnBodyPart(HumanBodyBones bodyPartToReconsider)
        {
            if (ignoreBoneSet.Contains(bodyPartToReconsider))
            {
                ignoreBoneSet.Remove(bodyPartToReconsider);
            }
        }

        public void IgnoreHapticsByDevice(HapticDevice deviceToIgnore)
        {
            ignoreHapticDevices.Add(deviceToIgnore);
        }

        public void ReconsiderHapticsByDevice(HapticDevice deviceToReconsider)
        {
            if (ignoreHapticDevices.Contains(deviceToReconsider))
            {
                ignoreHapticDevices.Remove(deviceToReconsider);
            }
        }
        #endregion

        #region Unity Functions
        void Awake()
        {
            bodyAffectedByDevice = new Dictionary<HumanBodyBones, List<HapticDevice>>();
            ignoreBoneSet = new HashSet<HumanBodyBones>();
            ignoreHapticDevices = new HashSet<HapticDevice>();

            animator = GetComponent<Animator>();

            if (animator != null)
            {
                foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
                {
                    Transform boneTransform = animator.GetBoneTransform(bone);

                    if (boneTransform != null)
                    {
                        BodyCoordinate attachedCoordinate = boneTransform.gameObject.GetComponent<BodyCoordinate>();

                        if (attachedCoordinate != null)
                        {
                            attachedCoordinate.attachedBody = bone;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("No Animator found on HapticManager. Bones cannot be automatically set, please make sure it was manually done.", this);
            }
        }
        #endregion
    }
}