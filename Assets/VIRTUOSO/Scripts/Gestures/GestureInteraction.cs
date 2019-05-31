using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharlesRiverAnalytics.Virtuoso.Gestures;
using VRTK;


namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Event Payload - provides the object the gesture is attached to
    /// whenever a gesture event happens
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        public GameObject gestureObject;

        public string gestureName;

        public SDK_BaseGestureLibrary.Hand gestureHand;

        public GestureEventArgs(GameObject gameObject, string name, SDK_BaseGestureLibrary.Hand hand)
        {
            gestureObject = gameObject;
            gestureName = name;
            gestureHand = hand;
        }
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="GestureEventArgs"/></param>
    public delegate void GestureEventHandler(object sender, GestureEventArgs e);

    public enum GestureEvents
    {
        GestureStarted,
        GestureEnded
    }

    /// <summary>
    /// Allows the user to make gestures with events going off when the user first makes the gesture
    /// and when they end it.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class GestureInteraction : VirtuosoEvent
    {
        #region PublicVariables
        public string gestureName;
        public Gesture gesture;
        #endregion

        #region PrivateVariables
        private bool gestureHasStartedLeft = false;
        private bool gestureHasStartedRight = false;
        private bool hasGestureLibraryFinishedSetup = false;
        private const float LIBRARY_TIME_CHECK_SECONDS = 1.0f;
        #endregion

        #region EventVariables
        // Emitted when the gesture is first done by either hand
        public event GestureEventHandler GestureStarted;

        // Emitted when the gesture finishes by either hand
        public event GestureEventHandler GestureEnded;

        public virtual void OnGestureStarted(GestureEventArgs e)
        {
            if (GestureStarted != null)
            {
                GestureStarted(this, e);
            }
        }

        public virtual void OnGestureEnded(GestureEventArgs e)
        {
            if (GestureEnded != null)
            {
                GestureEnded(this, e);
            }
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Continually checks for Gesture Library attached to the active Hand SDK. This must happen after
        /// the first frame or else the InteractGesture script will crash on the first frame as the Gesture
        /// Library may not be active in the scene yet.
        /// </summary>
        protected IEnumerator CheckForGestureLibrary()
        {
            // Don't let this occur before the first frame as the SDK will not be fully set up
            yield return new WaitForEndOfFrame();

            GameObject currentHandSDK = VRTK_SDKManager.instance.loadedSetup.actualHand;

            if (currentHandSDK == null)
            {
                Debug.LogError("No Hand SDK is currently seen by active SDK. Please make sure that Hand is filled in the SDK Setup.", this);

                enabled = false;

                yield break;
            }

            while (!hasGestureLibraryFinishedSetup)
            {
                if (currentHandSDK.activeInHierarchy)
                {
                    hasGestureLibraryFinishedSetup = true;

                    yield break;
                }

                yield return new WaitForSecondsRealtime(LIBRARY_TIME_CHECK_SECONDS);
            }
        }

        private void WaitForSDKSetup(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            StartCoroutine(CheckForGestureLibrary());
        }
        #endregion

        #region UnityFunctions
        protected void Awake()
        {
            VRTK_SDKManager.SubscribeLoadedSetupChanged(WaitForSDKSetup);
        }

        protected void OnApplicationQuit()
        {
            VRTK_SDKManager.UnsubscribeLoadedSetupChanged(WaitForSDKSetup);
        }

        protected void LateUpdate()
        {
            if (hasGestureLibraryFinishedSetup)
            {
                bool isLeftGesturing = gesture.IsGestureOccuring(SDK_BaseGestureLibrary.Hand.Left);
                bool isRightGesturing = gesture.IsGestureOccuring(SDK_BaseGestureLibrary.Hand.Right);

                // Check if the left hand started a gesture
                if (isLeftGesturing && !gestureHasStartedLeft)
                {
                    OnGestureStarted(new GestureEventArgs(gameObject, gestureName, SDK_BaseGestureLibrary.Hand.Left));

                    gestureHasStartedLeft = true;
                }
                // Check if the left hand stopped doing a gesture
                else if (!isLeftGesturing && gestureHasStartedLeft)
                {
                    OnGestureEnded(new GestureEventArgs(gameObject, gestureName, SDK_BaseGestureLibrary.Hand.Left));

                    gestureHasStartedLeft = false;
                }

                // Check if the right hand started a gesture
                if (isRightGesturing && !gestureHasStartedRight)
                {
                    OnGestureStarted(new GestureEventArgs(gameObject, gestureName, SDK_BaseGestureLibrary.Hand.Right));

                    gestureHasStartedRight = true;
                }
                // Check if the right hand stopped a gesture
                else if (!isRightGesturing && gestureHasStartedRight)
                {
                    OnGestureEnded(new GestureEventArgs(gameObject, gestureName, SDK_BaseGestureLibrary.Hand.Right));

                    gestureHasStartedRight = false;
                }
            }
        }
        #endregion
    }
}
