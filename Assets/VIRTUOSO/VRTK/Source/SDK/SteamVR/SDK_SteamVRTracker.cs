namespace VRTK
{
#if VRTK_DEFINE_SDK_STEAMVR
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Valve.VR;
#if VRTK_DEFINE_STEAMVR_PLUGIN_1_2_3
    using System;
    using System.Reflection;
#endif
#endif

    /// <summary>
    /// The SteamVR Tracker SDK script provides a bridge to SDK methods that deal with the tracker devices.
    /// Heavily based on SDK_SteamVRController.cs
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) 2018. 
    /// Updated: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    [SDK_Description(typeof(SDK_SteamVRSystem))]
    public class SDK_SteamVRTracker
#if VRTK_DEFINE_SDK_STEAMVR
        : SDK_BaseTracker
#else
        : SDK_FallbackTracker
#endif
    {
#if VRTK_DEFINE_SDK_STEAMVR
        protected Dictionary<GameObject, SteamVR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, SteamVR_TrackedObject>();
        protected Dictionary<uint, SteamVR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, SteamVR_TrackedObject>();

#if VRTK_DEFINE_STEAMVR_PLUGIN_1_2_3
        /// <summary>
        /// This method is called just after unloading the <see cref="VRTK_SDKSetup"/> that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public override void OnAfterSetupUnload(VRTK_SDKSetup setup)
        {
            base.OnAfterSetupUnload(setup);

            SteamVR_ControllerManager controllerManager = setup.actualLeftController.transform.parent.GetComponent<SteamVR_ControllerManager>();
            FieldInfo connectedField = typeof(SteamVR_ControllerManager).GetField(
                "connected",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (connectedField == null)
            {
                return;
            }

            bool[] connected = (bool[])connectedField.GetValue(controllerManager);
            Array.Clear(connected, 0, connected.Length);
            connectedField.SetValue(controllerManager, connected);
        }
#endif
        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
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
        /// The GetControllerIndex method returns the index of the given tracker.
        /// </summary>
        /// <param name="tracker">The GameObject containing the tracker.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetTrackerIndex(GameObject tracker)
        {
            SteamVR_TrackedObject trackedObject = GetTrackedObject(tracker);
            return (trackedObject != null ? (uint)trackedObject.index : uint.MaxValue);
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetTrackerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();

            if (index < uint.MaxValue)
            {
                if (cachedTrackedObjectsByIndex.ContainsKey(index) && cachedTrackedObjectsByIndex[index] != null)
                {
                    return cachedTrackedObjectsByIndex[index].gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetTrackerOrigin(VRTK_ControllerReference controllerReference)
        {
            SteamVR_TrackedObject trackedObject = GetTrackedObject(controllerReference.actual);

            if (trackedObject != null)
            {
                return (trackedObject.origin != null ? trackedObject.origin : trackedObject.transform.parent);
            }

            return null;
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            if (index <= (uint)SteamVR_TrackedObject.EIndex.Hmd || index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return Vector3.zero;
            }
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0
            SteamVR_TrackedObject device = GetTrackedObjectByIndex((int)index);
            VRTK_VelocityEstimator estimatedVelocity = device.GetComponent<VRTK_VelocityEstimator>();

            return estimatedVelocity.GetVelocityEstimate();
#else
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
            
            return device.velocity;
#endif
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            if (index <= (uint)SteamVR_TrackedObject.EIndex.Hmd || index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return Vector3.zero;
            }
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0
            SteamVR_TrackedObject device = GetTrackedObjectByIndex((int)index);
            VRTK_VelocityEstimator estimatedVelocity = device.GetComponent<VRTK_VelocityEstimator>();

            return estimatedVelocity.GetAngularVelocityEstimate();
#else
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);

            return device.angularVelocity;
#endif
        }

        protected virtual void Awake()
        {
#if VRTK_DEFINE_SDK_STEAMVR
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Listen(OnTrackedDeviceRoleChanged);
#endif
            SetTrackedControllerCaches(true);
        }

        protected virtual void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
        {
            SetTrackedControllerCaches(true);
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedTrackedObjectsByGameObject.Clear();
                cachedTrackedObjectsByIndex.Clear();
            }
            
            SteamVR_TrackedObject[] trackedObjects = VRTK_SharedMethods.FindEvenInactiveComponents<SteamVR_TrackedObject>();
            
            foreach (SteamVR_TrackedObject trackedObj in trackedObjects)
            {
                GameObject trackedGameObject = trackedObj.gameObject;
                int trackedObjectIndex = (int) trackedObj.index;

                uint ind = (uint)trackedObjectIndex;

                if(!cachedTrackedObjectsByGameObject.ContainsKey(trackedGameObject))
                {
                    cachedTrackedObjectsByGameObject.Add(trackedGameObject, trackedObj);
                }
                else
                {
                    cachedTrackedObjectsByGameObject[trackedGameObject] = trackedObj;
                }

                if(!cachedTrackedObjectsByIndex.ContainsKey(ind))
                {
                    cachedTrackedObjectsByIndex.Add(ind, trackedObj);
                }
                else
                {
                    cachedTrackedObjectsByIndex[ind] = trackedObj;
                }
            }
        }

        protected virtual SteamVR_TrackedObject GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();

            if (controller == null)
            {
                return null;
            }

            if (cachedTrackedObjectsByGameObject.ContainsKey(controller) && cachedTrackedObjectsByGameObject[controller] != null)
            {
                return cachedTrackedObjectsByGameObject[controller];
            }
            else
            {
                SteamVR_TrackedObject trackedObject = controller.GetComponent<SteamVR_TrackedObject>();
                if (trackedObject != null)
                {
                    cachedTrackedObjectsByGameObject.Add(controller, trackedObject);
                    cachedTrackedObjectsByIndex.Add((uint)trackedObject.index, trackedObject);
                }
                return trackedObject;
            }
        }

        public override int GetTrackerCount()
        {
            return cachedTrackedObjectsByGameObject.Count;
        }

        public override GameObject[] GetAllTrackers()
        {
            SetTrackedControllerCaches();

            List<GameObject> trackers = new List<GameObject>();
            
            foreach(GameObject key in cachedTrackedObjectsByGameObject.Keys)
            {
                trackers.Add(key);
            }
            return trackers.ToArray();
        }

        #region HelperMethods
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0
        protected SteamVR_TrackedObject GetTrackedObjectByIndex(int index)
        {
            SteamVR_TrackedObject[] allTrackedObjects = FindObjectsOfType<SteamVR_TrackedObject>();

            for(int n = 0; n < allTrackedObjects.Length; n++)
            {
                if((int)allTrackedObjects[n].index == index)
                {
                    return allTrackedObjects[n];
                }
            }

            return null;
        }
#endif
        #endregion
#endif
    }
}
