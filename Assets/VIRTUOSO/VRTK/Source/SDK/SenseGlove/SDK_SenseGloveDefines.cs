using System;

namespace VRTK
{
    /// <summary>
    /// Handles all of the scripting define symbols for the Sense Glove SDK
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public static class SDK_SenseGloveDefines
    {
        /// <summary>
        /// The scripting define symbol for the Leap Motion SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_SENSE_GLOVE";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, "Standalone")]
        private static bool IsSenseGloveInstalled()
        {
            Type pluginClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SenseGloveCs.SenseGlove");

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