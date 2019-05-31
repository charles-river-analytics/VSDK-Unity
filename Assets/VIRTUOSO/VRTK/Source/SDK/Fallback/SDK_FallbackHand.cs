using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    /// The Fallback Hand SDK script provides a fallback collection of methods that return null or default hand values.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    [SDK_Description(typeof(SDK_FallbackSystem))]
    public class SDK_FallbackHand : SDK_BaseHand
    {
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