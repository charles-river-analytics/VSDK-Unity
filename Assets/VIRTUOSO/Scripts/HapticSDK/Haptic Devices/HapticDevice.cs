using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// EventArgs for when a haptic pulse is sent out. Allows listeners to know where the haptic pulse is felt
    /// as well as the intensity of the feedback.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), April 2019
    /// </summary>
    public class HapticFeedbackEventArgs : EventArgs
    {
        public HumanBodyBones bodyPart;
        public BodyCoordinateHit hitLocation;
        public float feedbackIntensity;

        public HapticFeedbackEventArgs(HumanBodyBones body, BodyCoordinateHit hit, float intense)
        {
            bodyPart = body;
            hitLocation = hit;
            feedbackIntensity = intense;
        }
    }

    /// <summary>
    /// Generalized class for haptic devices. A specific device must specify how it triggers a haptic pulse
    /// in a single frame and how to cancle that pulse. The developer must then specify in the editor what 
    /// body parts the device instance will affect in terms of the body coordinate system.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public abstract class HapticDevice : MonoBehaviour
    {
        #region PublicVariables
        public ScriptableBodyCoordinate affectedBodyPart;
        [Tooltip("The amount of time that a single haptic pulse should take (in ms)")]
        [Range(20, 10000)]
        public int hapticDuration = 100;
        #endregion

        #region ProtectedVariables
        protected HapticManager manager;
        [SerializeField]
        protected HapticSystemAttribute hapticSystemInfo;
        #endregion

        #region PrivateVariables
        private ScriptableHapticPattern patternPlaying;
        private HumanBodyBones bodyPartHit;
        private float patternTime = 0.0f;
        #endregion

        #region Events
        public event EventHandler<HapticFeedbackEventArgs> HapticFeedbackPlayed;

        public virtual void OnHapticFeedbackPlayed(HapticFeedbackEventArgs e)
        {
            if (HapticFeedbackPlayed != null)
            {
                HapticFeedbackPlayed(this, e);
            }
        }
        #endregion

        #region PublicAPI
        /// <summary>
        /// A single haptic pulse that is triggered by an object hitting a body part that has been
        /// set up to recieve haptic events.
        /// </summary>
        /// <param name="bodyPart">The body part of the hit</param>
        /// <param name="hitLocation">The body coordinate system hit</param>
        /// <param name="intensity">The intensity to play the haptic vibration</param>
        public void TriggerDevice(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity)
        {
            if (affectedBodyPart.HitInsideAffectedArea(bodyPart, hitLocation))
            {
                StartHaptics(bodyPart, hitLocation, intensity);
            }
        }

        /// <summary>
        /// Plays a pattern that is triggered by an object hitting a body part that has been
        /// set up to recieve haptic events.
        /// </summary>
        /// <param name="bodyPart">The body part of the hit</param>
        /// <param name="hitLocation">The body coordinate system hit</param>
        /// <param name="hapticPattern">The haptic pattern</param>
        public void TriggerDevice(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, ScriptableHapticPattern hapticPattern)
        {
            if(bodyPart == affectedBodyPart.affectableBodyParts)
            {
                hapticPattern.hitOffset = hitLocation;

                PlayPattern(bodyPart, hapticPattern);
            }
        }

        /// <summary>
        /// Starts playing a pattern on a given bodypart
        /// </summary>
        /// <param name="bodyLocation">The body part to play the pattern on</param>
        /// <param name="pattern">The haptic pattern to play</param>
        public void PlayPattern(HumanBodyBones bodyLocation, ScriptableHapticPattern pattern)
        {
            patternPlaying = pattern;
            bodyPartHit = bodyLocation;
            patternTime = 0.0f;

            if (patternPlaying.playbackTiming == PlaybackTiming.Custom)
            {
                StartCoroutine(Play());
            }
        }

        /// <summary>
        /// Stops playing a pattern on a specific body part.
        /// </summary>
        /// <param name="bodyLocation">The body part that is current playing a pattern</param>
        public void StopPattern(HumanBodyBones bodyLocation)
        {
            if(bodyLocation == affectedBodyPart.affectableBodyParts)
            {
                CancelHaptics();

                ResetHapticPattern();
            }
        }
        #endregion

        #region PatternPlayingMethods
        protected IEnumerator Play()
        {
            while (patternPlaying != null)
            {
                bool keepPlaying = PlayPatternWithTiming(patternPlaying.customPlaybackTiming);

                // Wait that set amount of time
                if (keepPlaying)
                {
                    yield return new WaitForSeconds(patternPlaying.customPlaybackTiming);
                }
            }
        }

        /// <summary>
        /// Plays the current haptic pattern with the given frame time.
        /// </summary>
        /// <param name="frameTime">The time adjustment based on either DeltaTime, FixedDeltaTime, or custom</param>
        /// <returns>False when the timing of the pattern is over</returns>
        protected bool PlayPatternWithTiming(float frameTime)
        {
            // Stop playing the pattern if it is over
            if (patternTime > patternPlaying.EndTime)
            {
                CancelHaptics();

                ResetHapticPattern();

                return false;
            }

            // Get the current pattern value hit locations and intensity
            BodyCoordinateHit[] hits = patternPlaying.GetHitLocationsAtCurrentTime(patternTime);
            float[] intensities = patternPlaying.GetIntenstiyValuesAtCurrentTime(patternTime);

            // Tell the device to play
            PlayHapticList(bodyPartHit, hits, intensities);

            // Update the timing information
            patternTime += frameTime;

            return true;
        }

        protected void ResetHapticPattern()
        {
            if(patternPlaying != null)
            {
                patternPlaying.hitOffset = new BodyCoordinateHit();
            }
            
            patternTime = 0;
            patternPlaying = null;
        }

        protected void PlayHapticList(HumanBodyBones bodyPartHit, BodyCoordinateHit[] hitLocations, float[] intensities)
        {
            for (int n = 0; n < hitLocations.Length; n++)
            {
                // Ignore any pulses that have negative infinity since those are marked to be skipped
                if(float.IsNegativeInfinity(hitLocations[n].hitAngle) || float.IsNegativeInfinity(hitLocations[n].hitHeight))
                {
                    continue;
                }

                StartHaptics(bodyPartHit, hitLocations[n], intensities[n]);

                // Send an event with the haptic info you are playing
                OnHapticFeedbackPlayed(new HapticFeedbackEventArgs(bodyPartHit, hitLocations[n], intensities[n]));
            }
        }

        protected void ResolvePatternConflicts(ref BodyCoordinateHit[] hits, ref float[] intensities)
        {
            // Must have a valid pattern assigned
            if (patternPlaying != null)
            {
                int[] indiceArray = new int[hits.Length];
                Dictionary<int, List<int>> indiceCounter = new Dictionary<int, List<int>>();

                // Collisions are detected based on hitting the same coordinate space, so grab all the 
                // needed indices from all the hits
                for (int n = 0; n < hits.Length; n++)
                {
                    indiceArray[n] = affectedBodyPart.BodyCoordinateHitIndex(bodyPartHit, hits[n]);
                }

                // Iterate through the indices, saving the duplicates
                for (int n = 0; n < indiceArray.Length; n++)
                {
                    if (indiceCounter.ContainsKey(indiceArray[n]))
                    {
                        indiceCounter[indiceArray[n]].Add(n);
                    }
                    else
                    {
                        indiceCounter[indiceArray[n]] = new List<int>
                        {
                            n
                        };
                    }
                }

                // Iterate throught keyes, lists with more than 1 key in the list are the duplicates
                foreach (KeyValuePair<int, List<int>> item in indiceCounter)
                {
                    if (item.Value.Count > 1)
                    {
                        float adjustedIntensityValue = 0;

                        // Calculate the intensity from multiple collisions
                        switch (patternPlaying.collisionResolution)
                        {
                            // New intensity will be the smallest intensity value
                            case PatternCollisionResolution.Min:
                                float minValue = float.MaxValue;

                                for (int n = 0; n < item.Value.Count; n++)
                                {
                                    if (intensities[item.Value[n]] < minValue)
                                    {
                                        minValue = intensities[item.Value[n]];
                                    }
                                }

                                adjustedIntensityValue = minValue;

                                break;
                            // New intensity will be the greatest intensity value
                            case PatternCollisionResolution.Max:
                                float maxValue = float.MinValue;

                                for (int n = 0; n < item.Value.Count; n++)
                                {
                                    if (intensities[item.Value[n]] > maxValue)
                                    {
                                        maxValue = intensities[item.Value[n]];
                                    }
                                }

                                adjustedIntensityValue = maxValue;

                                break;
                            // New intensity will be the average of all the intensity value
                            case PatternCollisionResolution.Average:
                                float average = 0;

                                for (int n = 0; n < item.Value.Count; n++)
                                {
                                    average += intensities[item.Value[n]];
                                }

                                adjustedIntensityValue = average / item.Value.Count;

                                break;
                            // New intensity will be calculated by adding all the intensity values
                            case PatternCollisionResolution.Add:
                                float sum = 0;

                                for (int n = 0; n < item.Value.Count; n++)
                                {
                                    sum += intensities[item.Value[n]];
                                }

                                adjustedIntensityValue = sum;
                                break;
                            // New intensity will be calculated by multiplying all the intensity values
                            case PatternCollisionResolution.Multiply:
                                float product = 0;

                                for (int n = 0; n < item.Value.Count; n++)
                                {
                                    product *= intensities[item.Value[n]];
                                }

                                adjustedIntensityValue = product;
                                break;
                            // New intensity will be determined by a specific curve, otherwise it will default to the first curve in the list
                            case PatternCollisionResolution.CurvePriority:
                                if (item.Value.Contains(patternPlaying.curvePriorityIndex))
                                {
                                    adjustedIntensityValue = intensities[patternPlaying.curvePriorityIndex];
                                }
                                else
                                {
                                    adjustedIntensityValue = intensities[0];
                                }

                                break;
                        }

                        adjustedIntensityValue = Mathf.Clamp01(adjustedIntensityValue);

                        // Apply the calculated intensity back to the array
                        for (int n = 0; n < item.Value.Count; n++)
                        {
                            intensities[item.Value[n]] = adjustedIntensityValue;
                        }
                    }
                }
            }
        }

        public virtual void ApplyDefaultData(HapticSystemAttribute hapticSystemAtt)
        {
            // Check to see if the first data point is a BodyCoordinateSystem
            ScriptableBodyCoordinate bodyCoordinate = Resources.Load<ScriptableBodyCoordinate>(hapticSystemAtt.AffectedBodyFileLocation);

            if (bodyCoordinate != null)
            {
                affectedBodyPart = bodyCoordinate;
            }

            hapticSystemInfo = hapticSystemAtt;
        }
        #endregion

        #region AbstractMethods
        /// <summary>
        /// How the device plays a single haptic pulse
        /// </summary>
        /// <param name="hitLocation">The hit location in the BodyCoordinate space</param>
        /// <param name="intensity">The normalized (0-1) intensity value</param>
        protected abstract void StartHaptics(HumanBodyBones bodyPart, BodyCoordinateHit hitLocation, float intensity);

        /// <summary>
        /// How the device stops haptics
        /// </summary>
        protected abstract void CancelHaptics();
        #endregion

        #region UnityFunctions
        protected virtual void Start()
        {
            manager = FindObjectOfType<HapticManager>();

            if (manager == null)
            {
                Debug.LogWarning("No HapticManager found in scene. Please add a Haptic Manager to the scene.");

                enabled = false;
            }
            else
            {
                manager.AddDevicePerBodyLocation(this, affectedBodyPart);
            }

            // Since some types aren't serializable, use this method to rebuild them from serialized data
            hapticSystemInfo.ResetAfterSerialization();

            if (hapticSystemInfo.ConnectedSDKType != null)
            {
                VRTK_SDKManager.instance.LoadedSetupChanged += Instance_LoadedSetupChanged;
            }
        }

        protected void OnEnable()
        {
            ResetHapticPattern();
        }

        protected void Update()
        {
            if (patternPlaying != null && patternPlaying.playbackTiming == PlaybackTiming.Update)
            {
                PlayPatternWithTiming(Time.deltaTime);
            }
        }

        protected void FixedUpdate()
        {
            if (patternPlaying != null && patternPlaying.playbackTiming == PlaybackTiming.FixedUpdate)
            {
                PlayPatternWithTiming(Time.fixedDeltaTime);
            }
        }

        protected void OnApplicationQuit()
        {
            if (hapticSystemInfo.ConnectedSDKType != null)
            {
                VRTK_SDKManager.instance.LoadedSetupChanged -= Instance_LoadedSetupChanged;
            }
        }
        #endregion

        protected virtual void Instance_LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            // No-op
        }
    }
}