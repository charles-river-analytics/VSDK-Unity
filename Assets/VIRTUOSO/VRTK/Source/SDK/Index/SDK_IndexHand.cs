#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
#endif
namespace VRTK
{
    /// <summary>
    /// HandSDK applied to the Valve Index. The Index controllers are SteamVR based and defaults to the SteamVR Controller SDK
    /// methods when possible.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    [SDK_Description("Index Controller", SDK_SteamVRDefines.ScriptingDefineSymbol, "OpenVR", "Standalone")]
    public class SDK_IndexHand
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
        : SDK_BaseHand
#else
        : SDK_FallbackHand
#endif
    {
#if VRTK_DEFINE_STEAMVR_PLUGIN_2_0_0_OR_NEWER
        public override void SetHandCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedHandControllerObject = null;
            }

            SteamVR_PlayArea steamVRPlayArea = VRTK_SharedMethods.FindEvenInactiveComponent<SteamVR_PlayArea>();

            if(steamVRPlayArea != null)
            {
                cachedHandControllerObject = steamVRPlayArea.gameObject;

                // The root for the Index is the SteamVR player area as that is the top of the SteamVR hierarchy
                cachedRootTransform = steamVRPlayArea.transform;

                cachedGestureLibrary = cachedRootTransform.GetComponentInChildren<SDK_IndexGestureLibrary>(true);
            }
        }

        public override GameObject GetHandController()
        {
            GameObject hand = GetSDKManagerHandController();

            if (hand == null)
            {
                SetHandCaches();

                return cachedHandControllerObject;
            }

            return hand;
        }

        public override GameObject GetLeftHand()
        {
            return VRTK_SDK_Bridge.GetControllerLeftHand(true);
        }

        public override GameObject GetRightHand()
        {
            return VRTK_SDK_Bridge.GetControllerRightHand(true);
        }

        public override int GetHandCount()
        {
            int handCount = 0;

            if(GetLeftHand() != null)
            {
                handCount++;
            }

            if(GetRightHand() != null)
            {
                handCount++;
            }

            return handCount;
        }

        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
            // Noop - Implement for base class
        }

        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            // Noop - Implement for base class
        }

        public override Transform GetRootTransform()
        {
            if(cachedRootTransform == null)
            {
                SetHandCaches();
            }

            return cachedRootTransform;
        }
#endif
    }

}