using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// Abstract superclass to define how hands are brought into VRTK.
    /// 
    /// The cachedHandControllerObject should be set to the gameobject that provides the hand manager class for that particular device.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public abstract class SDK_BaseHand : SDK_Base
    {
        protected Transform cachedRootTransform;
        protected GameObject cachedHandControllerObject;
        protected SDK_BaseGestureLibrary cachedGestureLibrary;

        protected Transform originalSDKRoot;

        #region UnityFunctions
        protected virtual void Awake()
        {
            SetHandCaches(true);
        }
        #endregion

        #region AbstractMethods
        /// <summary>
        /// The IsConnected property returns true if the device itself is reporting that it is connected and tracking.
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// The GetHandController method returns the main GameObject device that is used to track the hands.
        /// </summary>
        /// <returns>The GameObject that represents the device that is tracking the hands.</returns>
        public abstract GameObject GetHandController();

        /// <summary>
        /// Use to retrieve the GameObject that holds the data for the tracked left hand.
        /// </summary>
        /// <returns>The GameObject that represents the left hand</returns>
        public abstract GameObject GetLeftHand();

        /// <summary>
        /// Use to retrieve the GameObject that holds the data for the tracked right hand.
        /// </summary>
        /// <returns>The GameObject that represents the right hand</returns>
        public abstract GameObject GetRightHand();

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public abstract void ProcessUpdate(Dictionary<string, object> options);

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public abstract void ProcessFixedUpdate(Dictionary<string, object> options);

        /// <summary>
        /// Retrieves the number of hands active in the SDK.
        /// </summary>
        /// <returns></returns>
        public abstract int GetHandCount();

        /// <summary>
        /// Returns the base transforms for the object that is the first component in the hiercharchy for the hand 
        /// </summary>
        /// <returns></returns>
        public abstract Transform GetRootTransform();

        /// <summary>
        /// Sets the caches for the hand controller.
        /// </summary>
        /// <param name="forceRefresh"></param>
        public abstract void SetHandCaches(bool forceRefresh = false);
        #endregion

        #region SharedMethods
        /// <summary>
        /// Returns the GameObject that is associated with the currently loaded set up
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject GetSDKManagerHandController()
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;

            if (sdkManager != null)
            {
                return sdkManager.loadedSetup.actualHand;
            }

            return null;
        }

        /// <summary>
        /// Returns the GestureLibrary that is used to describe gestures.
        /// </summary>
        /// <returns></returns>
        public virtual SDK_BaseGestureLibrary GetGestureLibrary()
        {
            return cachedGestureLibrary;
        }

        /// <summary>
        /// Returns the GameObject that the hand controller needs to be under in order to function, 
        /// e.g. LeapMotion overrides this since it needs to appear under the HMD since the controller moves with the head
        /// </summary>
        /// <returns></returns>
        public virtual GameObject GetNeededParent()
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;

            if (sdkManager != null)
            {
                return sdkManager.loadedSetup.gameObject;
            }

            return null;
        }

        /// <summary>
        /// This method is called just before loading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public override void OnBeforeSetupLoad(VRTK_SDKSetup setup)
        {
            originalSDKRoot = GetRootTransform() != null ? GetRootTransform().parent : null;
        }

        /// <summary>
        /// This method is called just before unloading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public override void OnBeforeSetupUnload(VRTK_SDKSetup setup)
        {
            if (originalSDKRoot != null && GetRootTransform() != null)
            {
                GetRootTransform().parent = originalSDKRoot;
                GetRootTransform().gameObject.SetActive(false);
            }

        }
        #endregion
    }
}