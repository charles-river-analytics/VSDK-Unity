using System;

namespace VRTK
{
    /// <summary>
    /// Handles all of the scripting define symbols for the BHaptics SDK
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public static class SDK_BhapticsDefines
    {
        /// <summary>
        /// The scripting define symbol for the Leap Motion SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_BHAPTICS";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        private static bool IsBHapticsInstalled()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("Bhaptics.Tact.Unity.BhapticsManager");

            if (pluginClass == null)
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