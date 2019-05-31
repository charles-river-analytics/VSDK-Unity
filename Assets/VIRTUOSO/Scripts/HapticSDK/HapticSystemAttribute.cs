using System;
using UnityEngine;
using VRTK;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Attribute to be used by a haptic device to define the system info for the editor. The attributes
    /// that are placed on the device are used in the editor to easily enable/disable the devices
    /// that the developer is trying to target. Additionaly, they can be used to set the default
    /// values for the device as well.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HapticSystemAttribute : Attribute
    {
        #region PrivateVariables
        [SerializeField]
        private string systemName;
        [SerializeField]
        private string deviceName;
        [SerializeField]
        private string affectedBodyFileLocation;
        [SerializeField]
        private Type connectedSDKType;
        [SerializeField]
        private string connectedSDKTypeName;
        [SerializeField]
        private object[] additionalData;
        #endregion

        public HapticSystemAttribute(string sysName, string devName, params object[] addData)
        {
            systemName = sysName;
            deviceName = devName;

            additionalData = addData;
        }

        public HapticSystemAttribute(string sysName, string devName, string bodyFileLocation, params object[] addData)
        {
            systemName = sysName;
            deviceName = devName;
            affectedBodyFileLocation = bodyFileLocation;

            additionalData = addData;
        }

        public HapticSystemAttribute(string sysName, string devName, string bodyFileLocation, Type sdkSetupType, params object[] addData)
        {
            systemName = sysName;
            deviceName = devName;
            affectedBodyFileLocation = bodyFileLocation;
            connectedSDKType = sdkSetupType;
            connectedSDKTypeName = connectedSDKType.AssemblyQualifiedName;
            additionalData = addData;
        }

        public string SystemName
        {
            get
            {
                return systemName;
            }
        }

        public string DeviceName
        {
            get
            {
                return deviceName;
            }
        }

        public string AffectedBodyFileLocation
        {
            get
            {
                return affectedBodyFileLocation;
            }
        }

        public Type ConnectedSDKType
        {
            get
            {
                return connectedSDKType;
            }
        }

        public string ConnectedSDKTypeName
        {
            get
            {
                return connectedSDKTypeName;
            }
        }

        public object[] AdditionalData
        {
            get
            {
                return additionalData;
            }
        }

        public virtual void ResetAfterSerialization()
        {
            if(!string.IsNullOrEmpty(ConnectedSDKTypeName))
            {
                connectedSDKType = Type.GetType(ConnectedSDKTypeName);
            }
        }
    }
}