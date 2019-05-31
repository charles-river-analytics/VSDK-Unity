namespace VRTK
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class provides a simulated VR tracker to VRTK. It is based off SDK_SimController.cs
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) August 2018
    /// </summary>
    [SDK_Description(typeof(SDK_SimSystem))]
    public class SDK_SimTracker : SDK_BaseTracker
    {
        // The SimTracker simulates the hardware using a Unity rigidbody.
        protected SDK_TrackerSim simTracker;

        #region VRTK Tracker SDK Functions
        /// <summary>
        /// Assigns a new trackerID to this object.
        /// </summary>
        /// <param name="newID"></param>
        public void SetTrackerID(uint newID)
        {
            trackerID = newID;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// In the simulated tracker, the reference is ignored and values are instead retrieved from the SimTracker object.
        /// </summary>
        /// <param name="controllerReference">Ignored by the simulated tracker</param>
        /// <returns></returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            return simTracker.GetAngularVelocity();
        }

        /// <summary>
        /// Retrieves the gameObject of the controller based on the tracker index.
        /// The index is ignored in the simulated tracker and instead it returns the SimTracker gameObject.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override GameObject GetTrackerByIndex(uint index, bool actual = false)
        {
            return simTracker.gameObject;
        }

        /// <summary>
        /// Retrieves the tracker index of this object.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public override uint GetTrackerIndex(GameObject controller)
        {
            return trackerID;
        }

        /// <summary>
        /// Returns the transform for the specified controllerReference; in the simulated tracker
        /// the reference is ignored and the tranform for the the TrackerSim is returned instead.
        /// </summary>
        /// <param name="controllerReference"></param>
        /// <returns></returns>
        public override Transform GetTrackerOrigin(VRTK_ControllerReference controllerReference)
        {
            return simTracker.transform;
        }

        /// <summary>
        /// Return the number of trackers. For a simulated tracker, this always returns 1.
        /// </summary>
        /// <returns></returns>
        public override int GetTrackerCount()
        {
            return 1;
        }

        /// <summary>
        /// Returns the velocity of the referenced tracker. In the simulated tracker the reference is ignored
        /// and this method returns the velocity for its paired trackersim object instead.
        /// </summary>
        /// <param name="controllerReference"></param>
        /// <returns></returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            return simTracker.GetVelocity();
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            //intentionally left blank; base method is abstract
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            //intentionally left blank; base method is abstract
        }

        public override GameObject[] GetAllTrackers()
        {
            GameObject[] simTrackers = new GameObject[1];
            if (simTracker)
            {
                simTrackers[0] = simTracker.gameObject;
                return simTrackers;
            }
            return null;
        }

        #endregion
    }
}
