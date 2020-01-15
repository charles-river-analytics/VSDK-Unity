using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRTK
{
    /// <summary>
    /// This SDK is used for setups without any tracked hands. It differs from Fallback in that Fallback is intended to catch
    /// missing files and prevent running, while no hands is an intended condition.
    /// 
    /// Written by: Dan Duggan (dduggan@cra.com), 2019
    /// </summary>
    [SDK_Description("No Hands", SDK_NoHandDefines.ScriptingDefineSymbol, null, "Standalone")]
    [SDK_Description("No Hands", SDK_NoHandDefines.ScriptingDefineSymbol, null, "Android")]
    public class SDK_NoHand : SDK_BaseHand
    {
        public override bool IsConnected { get { return true; } }

        public override GameObject GetHandController()
        {
            return null;
        }

        public override GameObject GetLeftHand()
        {
            return null;
        }

        public override GameObject GetRightHand()
        {
            return null;
        }

        public override int GetHandCount()
        {
            return 0;
        }

        public override Transform GetRootTransform()
        {
            return null;
        }

        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
            // Noop - Implement for base class
        }

        public override void ProcessUpdate(Dictionary<string, object> options)
        {
            // Noop - Implement for base class
        }

        public override void SetHandCaches(bool forceRefresh = false)
        {
            // Noop - Implement for base class
        }
    }
}
