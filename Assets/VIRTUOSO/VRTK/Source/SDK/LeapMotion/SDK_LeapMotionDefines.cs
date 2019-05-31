using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

namespace VRTK
{

    /// <summary>
    /// Handles all of the scripting define symbols for the Leap Motion SDK
    /// 
    /// Written by: Dan Duggan (dduggan@cra.com), 2018
    /// </summary>
    public static class SDK_LeapMotionDefines 
    {
        /// <summary>
        /// The scripting define symbol for the Leap Motion SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_LEAP_MOTION";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Android")]
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        private static bool IsLeapMotionInstalled()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("Leap.Unity.LeapProvider");
            if(pluginClass == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
