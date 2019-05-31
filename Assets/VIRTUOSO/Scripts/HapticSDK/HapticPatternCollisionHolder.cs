using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Component that holds a haptic pattern. When the GameObject that this component is attached 
    /// to hits a body coordinate, it will pull the attached pattern and play that pattern on that
    /// body part.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class HapticPatternCollisionHolder : MonoBehaviour
    {
        #region PublicVariables
        public ScriptableHapticPattern patternToSend;
        #endregion

        #region PublicAPI
        public ScriptableHapticPattern GetHapticPattern()
        {
            return patternToSend;
        }
        #endregion
    }
}