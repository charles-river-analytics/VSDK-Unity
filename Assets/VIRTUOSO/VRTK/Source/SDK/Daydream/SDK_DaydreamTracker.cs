using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// This is the Daydream SDK for trackers. It is functionally equivalent to the Fallback SDK, but with a different class name so that
    /// VRTK will allow Daydream VR to be used. This will go untested for the time being since Daydream is not an intended platform for VIRTUOSO. 
    /// The Fallback SDK System is intended to prevent the VRTK system from
    /// crashing in the event that no other VR system is able to launch. It simply returns null/default values for all methods.
    /// <remark>This script still says controller a lot. This allows it to play nice with controllers used as trackers.</remark>
    /// </summary>
    [SDK_Description(typeof(SDK_DaydreamSystem))]
    public class SDK_DaydreamTracker : SDK_BaseTracker
    {
        #region VRTK Functions Duplicated from SDK_FallbackController

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetTrackerByIndex(uint index, bool actual = false)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given tracker.
        /// </summary>
        /// <param name="controller">The GameObject containing the tracker.</param>
        /// <returns>The index of the given tracker.</returns>
        public override uint GetTrackerIndex(GameObject controller)
        {
            return uint.MaxValue;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetTrackerOrigin(VRTK_ControllerReference controllerReference)
        {
            return null;
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {

        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {

        }

        /// <summary>
        /// Retrieves the total number of trackers.
        /// </summary>
        /// <returns></returns>
        public override int GetTrackerCount()
        {
            return 0;
        }

        public override GameObject[] GetAllTrackers()
        {
            return new GameObject[0];
        }

        #endregion

    }

}
