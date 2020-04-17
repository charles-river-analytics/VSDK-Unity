namespace VRTK
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This class is a data structure for VRTK Tracked Object events (just index changes at this time).
    /// </summary>
    public class VRTKTrackedObjectEventArgs : EventArgs
    {
        public uint currentIndex;
        public uint previousIndex;
    }

    public delegate void VRTKTrackedObjectEventHandler(object sender, VRTKTrackedObjectEventArgs e);

    /// <summary>
    /// This class handles (generic) tracked object behavior such as event handling, index changes, etc. This should not be confused with the VRTK_SDKTracker class.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) August 2018
    /// </summary>
    public class VRTK_TrackedObject : MonoBehaviour
    {
        /// <summary>
        /// The Tracker Index indicates where this tracker falls in the VRTK_SDKSetup list. It acts a unique identifier, but due to the
        /// way VR tracking system works the unique identifier can still change.
        /// </summary>
        public uint index = uint.MaxValue;

        public event VRTKTrackedObjectEventHandler TrackerEnabled;
        public event VRTKTrackedObjectEventHandler TrackerDisabled;
        public event VRTKTrackedObjectEventHandler TrackerIndexChanged;

        protected GameObject aliasTracker;

        public virtual void OnTrackerEnabled(VRTKTrackedObjectEventArgs e)
        {
            if (TrackerEnabled != null)
            {
                TrackerEnabled(this, e);
            }
        }

        public virtual void OnTrackerDisabled(VRTKTrackedObjectEventArgs e)
        {
            if (TrackerDisabled != null)
            {
                TrackerDisabled(this, e);
            }
        }

        public virtual void OnTrackerIndexChanged(VRTKTrackedObjectEventArgs e)
        {
            if (TrackerIndexChanged != null)
            {
                TrackerIndexChanged(this, e);
            }
        }

        protected virtual VRTKTrackedObjectEventArgs SetEventPayload(uint previousIndex = uint.MaxValue)
        {
            return new VRTKTrackedObjectEventArgs
            {
                currentIndex = index,
                previousIndex = previousIndex
            };
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            aliasTracker = VRTK_DeviceFinder.GetScriptAliasTracker(gameObject);
            if (aliasTracker == null)
            {
                aliasTracker = gameObject;
            }

            index = VRTK_DeviceFinder.GetTrackerIndex(gameObject);
            OnTrackerEnabled(SetEventPayload());
        }

        protected virtual void OnDisable()
        {
            OnTrackerDisabled(SetEventPayload());
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void FixedUpdate()
        {
            VRTK_SDK_Bridge.TrackerProcessFixedUpdate(VRTK_ControllerReference.GetControllerReference(index));
        }

        protected virtual void Update()
        {
            uint checkIndex = VRTK_DeviceFinder.GetTrackerIndex(gameObject);
            if (checkIndex != index)
            {
                uint previousIndex = index;
                index = checkIndex;
                OnTrackerIndexChanged(SetEventPayload(previousIndex));
            }

            VRTK_SDK_Bridge.TrackerProcessUpdate(VRTK_ControllerReference.GetControllerReference(index));

            if (aliasTracker != null && gameObject.activeInHierarchy && !aliasTracker.activeSelf)
            {
                aliasTracker.SetActive(true);
            }
        }
    }
}