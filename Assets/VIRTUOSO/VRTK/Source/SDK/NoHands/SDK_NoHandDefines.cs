using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

namespace VRTK
{

    /// <summary>
    /// Handles all of the scripting define symbols for the No Hands SDK
    /// This is necessary for VRTK to think No Hands is installed
    /// Written by: Dan Duggan (dduggan@cra.com), 2019
    /// </summary>
    public static class SDK_NoHandDefines
    {
        /// <summary>
        /// The scripting define symbol for the Leap Motion SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_NO_HANDS";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Android")]
        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        private static bool IsNoHandsInstalled()
        {
            return true;
        }

    }
}
