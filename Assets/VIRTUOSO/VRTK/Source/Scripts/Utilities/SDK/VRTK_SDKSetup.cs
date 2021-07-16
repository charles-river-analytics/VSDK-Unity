﻿// SDK Setup|Utilities|90030
namespace VRTK
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Callbacks;
#endif
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The SDK Setup describes a list of SDKs and game objects to use.
    /// 
    /// Updated: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    public sealed class VRTK_SDKSetup : MonoBehaviour
    {
        public delegate void LoadEventHandler(VRTK_SDKManager sender, VRTK_SDKSetup setup);

        [Tooltip("Determines whether the SDK object references are automatically set to the objects of the selected SDKs. If this is true populating is done whenever the selected SDKs change.")]
        public bool autoPopulateObjectReferences = true;

        [Tooltip("A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.")]
        public GameObject actualBoundaries;
        [Tooltip("A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.")]
        public GameObject actualHeadset;
        [Tooltip("A reference to the GameObject that contains the SDK Left Hand Controller.")]
        public GameObject actualLeftController;
        [Tooltip("A reference to the GameObject that contains the SDK Right Hand Controller.")]
        public GameObject actualRightController;
        [Tooltip("A list of all SDK tracker GameObjects")]
        public List<GameObject> actualTrackers;
        [Tooltip("A reference to the GameObject that contains the SDK for the hands.")]
        public GameObject actualHand;

        [Tooltip("A reference to the GameObject that models for the Left Hand Controller.")]
        public GameObject modelAliasLeftController;
        [Tooltip("A reference to the GameObject that models for the Right Hand Controller.")]
        public GameObject modelAliasRightController;

        public event LoadEventHandler Loaded;
        public event LoadEventHandler Unloaded;

        /// <summary>
        /// The info of the SDK to use to deal with all system actions. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo systemSDKInfo
        {
            get
            {
                return cachedSystemSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>()[0];
                if (cachedSystemSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedSystemSDK);
#else
                Destroy(cachedSystemSDK);
#endif
                cachedSystemSDK = null;

                cachedSystemSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        /// <summary>
        /// The info of the SDK to use to utilize room scale boundaries. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo boundariesSDKInfo
        {
            get
            {
                return cachedBoundariesSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>()[0];
                if (cachedBoundariesSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedBoundariesSDK);
#else
                Destroy(cachedBoundariesSDK);
#endif
                cachedBoundariesSDK = null;

                cachedBoundariesSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        /// <summary>
        /// The info of the SDK to use to utilize the VR headset. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo headsetSDKInfo
        {
            get
            {
                return cachedHeadsetSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>()[0];
                if (cachedHeadsetSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedHeadsetSDK);
#else
                Destroy(cachedHeadsetSDK);
#endif
                cachedHeadsetSDK = null;

                cachedHeadsetSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        /// <summary>
        /// The info of the SDK to use to utilize the input devices. By setting this to `null` the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo controllerSDKInfo
        {
            get
            {
                return cachedControllerSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>()[0];
                if (cachedControllerSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedControllerSDK);
#else
                Destroy(cachedControllerSDK);
#endif
                cachedControllerSDK = null;

                cachedControllerSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        /// <summary>
        /// The info of the SDK to use to utilize the input devices. By setting this to <see langword="null"/> the fallback SDK will be used.
        /// </summary>
        public VRTK_SDKInfo trackerSDKInfo
        {
            get
            {
                return cachedTrackerSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseTracker, SDK_FallbackTracker, SDK_FallbackTracker>()[0];
                if (cachedTrackerSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedTrackerSDK);
#else
                Destroy(cachedTrackerSDK);
#endif
                cachedTrackerSDKInfo = null;

                cachedTrackerSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        public VRTK_SDKInfo handSDKInfo
        {
            get
            {
                return cachedHandSDKInfo;
            }
            set
            {
                value = value ?? VRTK_SDKInfo.Create<SDK_BaseHand, SDK_FallbackHand, SDK_FallbackHand>()[0];
                if (cachedHandSDKInfo == value)
                {
                    return;
                }

#if UNITY_EDITOR
                DestroyImmediate(cachedHandSDK);
#else
                Destroy(cachedHandSDK);
#endif
                cachedHandSDKInfo = null;

                cachedHandSDKInfo = new VRTK_SDKInfo(value);
                PopulateObjectReferences(false);
            }
        }

        /// <summary>
        /// The selected system SDK.
        /// </summary>
        /// <returns>The currently selected system SDK.</returns>
        public SDK_BaseSystem systemSDK
        {
            get
            {
                if (cachedSystemSDK == null)
                {
                    HandleSDKGetter<SDK_BaseSystem>("System", systemSDKInfo, VRTK_SDKManager.InstalledSystemSDKInfos);
                    cachedSystemSDK = (SDK_BaseSystem)ScriptableObject.CreateInstance(systemSDKInfo.type);
                }

                return cachedSystemSDK;
            }
        }

        /// <summary>
        /// The selected boundaries SDK.
        /// </summary>
        /// <returns>The currently selected boundaries SDK.</returns>
        public SDK_BaseBoundaries boundariesSDK
        {
            get
            {
                if (cachedBoundariesSDK == null)
                {
                    HandleSDKGetter<SDK_BaseBoundaries>("Boundaries", boundariesSDKInfo, VRTK_SDKManager.InstalledBoundariesSDKInfos);
                    cachedBoundariesSDK = (SDK_BaseBoundaries)ScriptableObject.CreateInstance(boundariesSDKInfo.type);
                }

                return cachedBoundariesSDK;
            }
        }

        /// <summary>
        /// The selected headset SDK.
        /// </summary>
        /// <returns>The currently selected headset SDK.</returns>
        public SDK_BaseHeadset headsetSDK
        {
            get
            {
                if (cachedHeadsetSDK == null)
                {
                    HandleSDKGetter<SDK_BaseHeadset>("Headset", headsetSDKInfo, VRTK_SDKManager.InstalledHeadsetSDKInfos);
                    cachedHeadsetSDK = (SDK_BaseHeadset)ScriptableObject.CreateInstance(headsetSDKInfo.type);
                }

                return cachedHeadsetSDK;
            }
        }

        /// <summary>
        /// The selected controller SDK.
        /// </summary>
        /// <returns>The currently selected controller SDK.</returns>
        public SDK_BaseController controllerSDK
        {
            get
            {
                if (cachedControllerSDK == null)
                {
                    HandleSDKGetter<SDK_BaseController>("Controller", controllerSDKInfo, VRTK_SDKManager.InstalledControllerSDKInfos);
                    cachedControllerSDK = (SDK_BaseController)ScriptableObject.CreateInstance(controllerSDKInfo.type);
                }

                return cachedControllerSDK;
            }
        }

        /// <summary>
        /// The selected controller SDK.
        /// </summary>
        /// <returns>The currently selected controller SDK.</returns>
        public SDK_BaseTracker trackerSDK
        {
            get
            {
                if (cachedTrackerSDK == null)
                {
                    HandleSDKGetter<SDK_BaseTracker>("Tracker", trackerSDKInfo, VRTK_SDKManager.InstalledTrackerSDKInfos);
                    cachedTrackerSDK = (SDK_BaseTracker)ScriptableObject.CreateInstance(trackerSDKInfo.type);
                }

                return cachedTrackerSDK;
            }
        }

        public SDK_BaseHand handSDK
        {
            get
            {
                if(cachedHandSDK == null)
                {
                    HandleSDKGetter<SDK_BaseHand>("Hand", handSDKInfo, VRTK_SDKManager.InstalledHandSDKInfos);
                    cachedHandSDK = (SDK_BaseHand)ScriptableObject.CreateInstance(handSDKInfo.type);
                }
                return cachedHandSDK;
            }
        }

        /// <summary>
        /// The VR device names used by the currently selected SDKs.
        /// </summary>
        public string[] usedVRDeviceNames
        {
            get
            {
                // VIRTUOSO Note: Do not include HandSDK here since it should be VR device agnostic
                VRTK_SDKInfo[] infos = { systemSDKInfo, boundariesSDKInfo, headsetSDKInfo, controllerSDKInfo, trackerSDKInfo };
                return infos.Select(info => info.description.vrDeviceName)
                            .Distinct()
                            .ToArray();
            }
        }

        /// <summary>
        /// Whether it's possible to use the Setup. See `GetSimplifiedErrorDescriptions` for more info.
        /// </summary>
        public bool isValid
        {
            get
            {
                return GetSimplifiedErrorDescriptions().Length == 0;
            }
        }

        [SerializeField]
        private VRTK_SDKInfo cachedSystemSDKInfo = VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>()[0];
        [SerializeField]
        private VRTK_SDKInfo cachedBoundariesSDKInfo = VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>()[0];
        [SerializeField]
        private VRTK_SDKInfo cachedHeadsetSDKInfo = VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>()[0];
        [SerializeField]
        private VRTK_SDKInfo cachedControllerSDKInfo = VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>()[0];
        [SerializeField]
        private VRTK_SDKInfo cachedTrackerSDKInfo = VRTK_SDKInfo.Create<SDK_BaseTracker, SDK_FallbackTracker, SDK_FallbackTracker>()[0];
        [SerializeField]
        private VRTK_SDKInfo cachedHandSDKInfo = VRTK_SDKInfo.Create<SDK_BaseHand, SDK_FallbackHand, SDK_FallbackHand>()[0];

        private SDK_BaseSystem cachedSystemSDK;
        private SDK_BaseBoundaries cachedBoundariesSDK;
        private SDK_BaseHeadset cachedHeadsetSDK;
        private SDK_BaseController cachedControllerSDK;
        private SDK_BaseTracker cachedTrackerSDK;
        private SDK_BaseHand cachedHandSDK;

        /// <summary>
        /// The PopulateObjectReferences method populates the object references by using the currently set SDKs.
        /// </summary>
        /// <param name="force">Whether to ignore `autoPopulateObjectReferences` while deciding to populate.</param>
        public void PopulateObjectReferences(bool force)
        {
            if (!(force || autoPopulateObjectReferences))
            {
                return;
            }

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && VRTK_SDKManager.ValidInstance())
            {
                VRTK_SDKManager.instance.SetLoadedSDKSetupToPopulateObjectReferences(this);
            }
#endif
            VRTK_SDK_Bridge.InvalidateCaches();

#if UNITY_EDITOR
            Undo.RecordObject(this, "Populate Object References");
#endif

            actualBoundaries = null;
            actualHeadset = null;
            actualLeftController = null;
            actualRightController = null;
            modelAliasLeftController = null;
            modelAliasRightController = null;
            actualTrackers = new List<GameObject>();
            actualHand = null;

            Transform playAreaTransform = boundariesSDK.GetPlayArea();
            Transform headsetTransform = headsetSDK.GetHeadset();
            Transform handTransform = handSDK.GetRootTransform();

            actualBoundaries = playAreaTransform == null ? null : playAreaTransform.gameObject;
            actualHeadset = headsetTransform == null ? null : headsetTransform.gameObject;
            actualLeftController = controllerSDK.GetControllerLeftHand(true);
            actualRightController = controllerSDK.GetControllerRightHand(true);
            actualHand = handTransform == null ? null : handTransform.gameObject;
            modelAliasLeftController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Left);
            modelAliasRightController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Right);

            GameObject[] trackerObjects = trackerSDK.GetAllTrackers();
            
            if (trackerObjects != null)
            {
                foreach (GameObject tracker in trackerObjects)
                {
                    if (!(tracker.Equals(actualHeadset) 
                        || tracker.Equals(actualLeftController) 
                        || tracker.Equals(actualRightController)
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
                        || tracker.GetComponent<Valve.VR.SteamVR_TrackedObject>().index == Valve.VR.SteamVR_TrackedObject.EIndex.Hmd
#elif VRTK_DEFINE_SDK_STEAMVR
                        || tracker.GetComponent<SteamVR_TrackedObject>().index == SteamVR_TrackedObject.EIndex.Hmd
#endif
                        ))
                    {
                        // for now, we are not including HMDs or controllers in this list. We will revisit later, depending on the
                        // progress of gesture recognition and Oculus integration levels. See JIRA ticket VIRTUOSO-119 for details
                        actualTrackers.Add(tracker);
                    }
                }
            }
        }

        /// <summary>
        /// The GetSimplifiedErrorDescriptions method checks the setup for errors and creates an array of error descriptions.
        /// </summary>
        /// <remarks>
        /// The returned error descriptions handle the following cases for the current SDK infos:
        /// <list type="bullet">
        /// <item> <description>Its type doesn't exist anymore.</description> </item>
        /// <item> <description>It's a fallback SDK.</description> </item>
        /// <item> <description>It doesn't have its scripting define symbols added.</description> </item>
        /// <item> <description>It's missing its vendor SDK.</description> </item>
        /// </list>
        /// Additionally the current SDK infos are checked whether they use multiple VR Devices.
        /// </remarks>
        /// <returns>An array of all the error descriptions. Returns an empty array if no errors are found.</returns>
        public string[] GetSimplifiedErrorDescriptions()
        {
            List<string> sdkErrorDescriptions = new List<string>();

            ReadOnlyCollection<VRTK_SDKInfo>[] installedSDKInfosList = {
                VRTK_SDKManager.InstalledSystemSDKInfos,
                VRTK_SDKManager.InstalledBoundariesSDKInfos,
                VRTK_SDKManager.InstalledHeadsetSDKInfos,
                VRTK_SDKManager.InstalledControllerSDKInfos,
                VRTK_SDKManager.InstalledTrackerSDKInfos,
                VRTK_SDKManager.InstalledHandSDKInfos
            };
            VRTK_SDKInfo[] currentSDKInfos = { systemSDKInfo, boundariesSDKInfo, headsetSDKInfo, controllerSDKInfo, trackerSDKInfo, handSDKInfo };

            for (int index = 0; index < installedSDKInfosList.Length; index++)
            {
                ReadOnlyCollection<VRTK_SDKInfo> installedSDKInfos = installedSDKInfosList[index];
                VRTK_SDKInfo currentSDKInfo = currentSDKInfos[index];

                Type baseType = VRTK_SharedMethods.GetBaseType(currentSDKInfo.type);
                if (baseType == null)
                {
                    continue;
                }

                if (currentSDKInfo.originalTypeNameWhenFallbackIsUsed != null)
                {
                    sdkErrorDescriptions.Add(string.Format("The SDK '{0}' doesn't exist anymore.", currentSDKInfo.originalTypeNameWhenFallbackIsUsed));
                }
                else if (currentSDKInfo.description.describesFallbackSDK)
                {
                    sdkErrorDescriptions.Add("A fallback SDK is used. Make sure to set a real SDK.");
                }
                else if (!installedSDKInfos.Contains(currentSDKInfo))
                {
                    sdkErrorDescriptions.Add(string.Format("The vendor SDK for '{0}' is not installed.", currentSDKInfo.description.prettyName));
                }
            }

            if (usedVRDeviceNames.Except(new[] { "None" }).Count() > 1)
            {
                sdkErrorDescriptions.Add("The current SDK selection uses multiple VR Devices. It's not possible to use more than one VR Device at the same time.");
            }

            return sdkErrorDescriptions.Distinct().ToArray();
        }

        /// <summary>
        /// The OnLoaded method determines when an SDK Setup has been loaded.
        /// </summary>
        /// <param name="sender">The SDK Manager that has loaded the SDK Setup.</param>
        public void OnLoaded(VRTK_SDKManager sender)
        {
            List<SDK_Base> sdkBases = new SDK_Base[] { systemSDK, boundariesSDK, headsetSDK, controllerSDK, trackerSDK, handSDK }.ToList();
            sdkBases.ForEach(sdkBase => sdkBase.OnBeforeSetupLoad(this));

            gameObject.SetActive(true);
            VRTK_SDK_Bridge.InvalidateCaches();
            SetupHeadset();
            SetupControllers();
            SetupTrackers();
            SetupHands();
            boundariesSDK.InitBoundaries();

            sdkBases.ForEach(sdkBase => sdkBase.OnAfterSetupLoad(this));

            LoadEventHandler handler = Loaded;
            if (handler != null)
            {
                handler(sender, this);
            }
        }

        /// <summary>
        /// The OnUnloaded method determines when an SDK Setup has been unloaded.
        /// </summary>
        /// <param name="sender">The SDK Manager that has unloaded the SDK Setup.</param>
        public void OnUnloaded(VRTK_SDKManager sender)
        {
            List<SDK_Base> sdkBases = new SDK_Base[] { systemSDK, boundariesSDK, headsetSDK, controllerSDK, trackerSDK, handSDK }.ToList();
            sdkBases.ForEach(sdkBase => sdkBase.OnBeforeSetupUnload(this));

            gameObject.SetActive(false);

            sdkBases.ForEach(sdkBase => sdkBase.OnAfterSetupUnload(this));

            LoadEventHandler handler = Unloaded;
            if (handler != null)
            {
                handler(sender, this);
            }
        }

        private void OnEnable()
        {
            if (VRTK_SDKManager.ValidInstance())
            {
                PopulateObjectReferences(false);
            }
        }

#if UNITY_EDITOR
        static VRTK_SDKSetup()
        {
            //call AutoPopulateObjectReferences when the currently active scene changes
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += AutoPopulateObjectReferences;
#else
            EditorApplication.hierarchyWindowChanged += AutoPopulateObjectReferences;
#endif
        }

        [DidReloadScripts(2)]
        private static void AutoPopulateObjectReferences()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            foreach (VRTK_SDKSetup setup in VRTK_SharedMethods.FindEvenInactiveComponents<VRTK_SDKSetup>(true))
            {
                setup.PopulateObjectReferences(false);
            }
        }
#endif

        /// <summary>
        /// Handles the various SDK getters by logging potential errors.
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to handle the getter for. Must be a subclass of SDK_Base.</typeparam>
        /// <param name="prettyName">The pretty name of the base SDK to use when logging errors.</param>
        /// <param name="info">The SDK info of which the SDK getter was called.</param>
        /// <param name="installedInfos">The installed SDK infos of which the SDK getter was called.</param>
        private static void HandleSDKGetter<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
        {
            if (VRTK_SharedMethods.IsEditTime())
            {
                return;
            }

            string sdkErrorDescription = GetSDKErrorDescription<BaseType>(prettyName, info, installedInfos);
            if (!string.IsNullOrEmpty(sdkErrorDescription))
            {
                VRTK_Logger.Error(sdkErrorDescription);
            }
        }

        /// <summary>
        /// Returns an error description in case any of these are true for the current SDK info:
        /// <list type="bullet">
        /// <item> <description>Its type doesn't exist anymore.</description> </item>
        /// <item> <description>It's a fallback SDK.</description> </item>
        /// <item> <description>It doesn't have its scripting define symbols added.</description> </item>
        /// <item> <description>It's missing its vendor SDK.</description> </item>
        /// </list>
        /// </summary>
        /// <typeparam name="BaseType">The SDK base type of which to return the error description for. Must be a subclass of SDK_Base.</typeparam>
        /// <param name="prettyName">The pretty name of the base SDK to use when returning error descriptions.</param>
        /// <param name="info">The SDK info of which to return the error description for.</param>
        /// <param name="installedInfos">The installed SDK infos.</param>
        /// <returns>An error description if there is one, else `null`.</returns>
        private static string GetSDKErrorDescription<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
        {
            Type selectedType = info.type;
            Type baseType = typeof(BaseType);
            Type fallbackType = VRTK_SDKManager.SDKFallbackTypesByBaseType[baseType];

            if (selectedType == fallbackType)
            {
                return string.Format("The fallback {0} SDK is being used because there is no other {0} SDK set in the SDK Setup.", prettyName);
            }

            if (!VRTK_SharedMethods.IsTypeAssignableFrom(baseType, selectedType) || VRTK_SharedMethods.IsTypeAssignableFrom(fallbackType, selectedType))
            {
                string description = string.Format("The fallback {0} SDK is being used despite being set to '{1}'.", prettyName, selectedType.Name);

                if (installedInfos.Select(installedInfo => installedInfo.type).Contains(selectedType))
                {
                    return description + " Its needed scripting define symbols are not added. You can click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and choose to automatically let the manager handle the scripting define symbols." + VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SCRIPTING_DEFINE_SYMBOLS_NOT_FOUND);
                }

                return description + " The needed vendor SDK isn't installed.";
            }

            return null;
        }

        private void SetupHeadset()
        {
            if (actualHeadset != null && !actualHeadset.GetComponent<VRTK_TrackedHeadset>())
            {
                actualHeadset.AddComponent<VRTK_TrackedHeadset>();
            }
        }

        private void SetupControllers()
        {
            Action<GameObject, GameObject> setParent = (scriptAliasGameObject, actualGameObject) =>
            {
                if (scriptAliasGameObject == null)
                {
                    return;
                }

                Transform scriptAliasTransform = scriptAliasGameObject.transform;
                Transform actualTransform = actualGameObject.transform;

                if (scriptAliasTransform.parent != actualTransform)
                {
                    Vector3 previousScale = scriptAliasTransform.localScale;
                    scriptAliasTransform.SetParent(actualTransform);
                    scriptAliasTransform.localScale = previousScale;
                }

                scriptAliasTransform.localPosition = Vector3.zero;
                scriptAliasTransform.localRotation = Quaternion.identity;
            };

            if (actualLeftController != null && VRTK_SDKManager.ValidInstance())
            {
                setParent(VRTK_SDKManager.instance.scriptAliasLeftController, actualLeftController);

                if (actualLeftController.GetComponent<VRTK_TrackedController>() == null)
                {
                    actualLeftController.AddComponent<VRTK_TrackedController>();
                }
            }

            if (actualRightController != null && VRTK_SDKManager.ValidInstance())
            {
                setParent(VRTK_SDKManager.instance.scriptAliasRightController, actualRightController);

                if (actualRightController.GetComponent<VRTK_TrackedController>() == null)
                {
                    actualRightController.AddComponent<VRTK_TrackedController>();
                }
            }
        }

        private void SetupTrackers()
        {
            foreach (GameObject tracker in actualTrackers)
            {
                if (tracker.GetComponent<VRTK_TrackedObject>() == null)
                {
                    tracker.AddComponent<VRTK_TrackedObject>();
                }
            }
        }

        private void SetupHands()
        {
            Action<GameObject, GameObject> SetParent = (parentGameObject, actualGameObject) =>
            {
                if (parentGameObject == null)
                {
                    return;
                }

                Transform parentTransform = parentGameObject.transform;
                Transform actualTransform = actualGameObject.transform;

                if (parentTransform.parent != actualTransform)
                {
                    Vector3 previousScale = actualTransform.localScale;
                    actualTransform.SetParent(parentTransform);
                    actualTransform.localScale = previousScale;
                }

                actualTransform.localPosition = Vector3.zero;
                actualTransform.localRotation = Quaternion.identity;
            };

            if (actualHand != null)
            {
                SetParent(handSDK.GetNeededParent(), actualHand);

                actualHand.gameObject.SetActive(true);
            }
        }
    }
}