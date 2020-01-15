using System;
using System.Collections;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Simple timer class for firing off an event in the editor when a specified time has completed.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [DisallowMultipleComponent]
    public class Timer : GeneralEventSender
    {
        #region PublicVariables
        [Tooltip("Amount of time in seconds before the timer goes off.")]
        public float timerDuration = 1.0f;
        [Tooltip("When true, will start the timer when the Awake method is first called.")]
        public bool startOnEnable = false;
        #endregion

        #region PrivateVariables
        private Coroutine timerCoroutine;
        #endregion

        #region PublicAPI
        public void StartTimer()
        {
            if(timerCoroutine == null)
            {
                timerCoroutine = StartCoroutine(RunTimer());
            }
        }

        public void StopTimer()
        {
            if(timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);

                timerCoroutine = null;
            }
        }
        #endregion

        #region CoroutineFunctions
        private IEnumerator RunTimer()
        {
            yield return new WaitForSeconds(timerDuration);

            OnActionComplete(new EventArgs());
        }
        #endregion

        #region UnityFunctions
        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartTimer();
            }
        }
        #endregion
    }
}