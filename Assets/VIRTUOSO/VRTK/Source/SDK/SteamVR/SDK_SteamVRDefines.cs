// SteamVR Defines|SDK_SteamVR|001
namespace VRTK
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Handles all the scripting define symbols for the SteamVR SDK.
    /// </summary>
    public static class SDK_SteamVRDefines
    {
        /// <summary>
        /// The scripting define symbol for the SteamVR SDK.
        /// </summary>
        public const string ScriptingDefineSymbol = SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "SDK_STEAMVR";

        private const string BuildTargetGroupName = "Standalone";

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_2_0_0", BuildTargetGroupName)]
        private static bool IsPluginVersion200OrNewer()
        {
            // 2.0 version adds namespaces to SteamVR
            Type controllerManagerClass = VRTK_SharedMethods.GetTypeUnknownAssembly("Valve.VR.SteamVR_Events");
            return (controllerManagerClass != null);
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_INPUT_COMPILED", BuildTargetGroupName)]
        private static bool IsSteamVRInputCompiled()
        {
            // determines if the developer has built the SteamVR inputs. Otherwise, because of how Unity compiles things it will be unable to build the inputs because it can't build the whole project
            Type steamVRInputClass = VRTK_SharedMethods.GetTypeUnknownAssembly("Valve.VR.SteamVR_Input_ActionSet_naturalistic");
            return steamVRInputClass != null;
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_1_2_3", BuildTargetGroupName)]
        private static bool IsPluginVersion123OrNewer()
        {
            Type controllerManagerClass = VRTK_SharedMethods.GetTypeUnknownAssembly("SteamVR_ControllerManager");
            if (controllerManagerClass == null)
            { 
                return false;
            }

            bool version123Identified = controllerManagerClass.GetMethod("SetUniqueObject", BindingFlags.NonPublic | BindingFlags.Instance) != null;
            return version123Identified && !IsPluginVersion200OrNewer();
        }

        [SDK_ScriptingDefineSymbolPredicate(ScriptingDefineSymbol, BuildTargetGroupName)]
        [SDK_ScriptingDefineSymbolPredicate(SDK_ScriptingDefineSymbolPredicateAttribute.RemovableSymbolPrefix + "STEAMVR_PLUGIN_VERSION_UNSUPPORTED", BuildTargetGroupName)]
        private static bool IsPluginVersionUnsupported()
        {
            return (!(IsPluginVersion123OrNewer() || IsPluginVersion200OrNewer()));
        }
    }
}