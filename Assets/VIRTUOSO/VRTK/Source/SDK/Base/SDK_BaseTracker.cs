using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// The Base Tracker SDK script provides a bridge to SDK methods that deal with tracker pucks, as well as tracked controllers in a more general sense.
    /// It is based on the Base Controller class, but is different as it applies no 3D models, leftness/rightness, or buttons.
    /// This class uses a few VRTK_ControllerReferences because that class provides some useful convenience methods and it did not seem worthwhile to
    /// produce a carbon copy with the word 'controller' replaced with tracker.
    /// Author: Dan Duggan (dduggan@cra.com) 2018
    /// </summary>
    public abstract class SDK_BaseTracker : SDK_Base
    {

        /// <summary>
        /// TrackerID refers to the ID by which the SDK identifies this tracker.
        /// </summary>
        public uint trackerID;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public abstract void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public abstract void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);

        /// <summary>
        /// The GetTrackerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the tracker.</param>
        /// <returns>The index of the given tracker.</returns>
        public abstract uint GetTrackerIndex(GameObject tracker);

        /// <summary>
        /// The GetTrackerByIndex method returns the GameObject of a tracker with a specific index.
        /// </summary>
        /// <param name="index">The index of the tracker to find.</param>
        /// <param name="actual">If true it will return the actual tracker, if false it will return the script alias tracker GameObject.</param>
        /// <returns>The GameObject of the tracker</returns>
        public abstract GameObject GetTrackerByIndex(uint index, bool actual = false);

        /// <summary>
        /// Return an array of all trackers within the system.
        /// </summary>
        /// <returns></returns>
        public abstract GameObject[] GetAllTrackers();

        /// <summary>
        /// The GetTrackerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracjer to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public abstract Transform GetTrackerOrigin(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public abstract Vector3 GetVelocity(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public abstract Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// Retrieves the number of trackers active in the SDK.
        /// </summary>
        /// <returns></returns>
        public abstract int GetTrackerCount();

    }
}
