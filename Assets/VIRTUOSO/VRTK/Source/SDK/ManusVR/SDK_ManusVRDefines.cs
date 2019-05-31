using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// Handles all of the scripting define symbols for the ManusVR SDK
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), January 2019
    /// </summary>
    public static class SDK_ManusVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the Leap Motion SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_MANUS_VR";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        private static bool IsManusVRInstalled()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("Assets.ManusVR.Scripts.HandData");

            return !(pluginClass == null);
        }
    }
}