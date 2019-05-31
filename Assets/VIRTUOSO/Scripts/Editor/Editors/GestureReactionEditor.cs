using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// Custom editor for the GestureReaction. The main point of the custom editor is to hide
    /// the start/stop reaction variable if the given ReactionEvent has one of it's methods marked
    /// as HideMethodFromInspector.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2018
    /// </summary>
    public class GestureReactionEditor : GenericReactionEditor
    {
        #region UnityFunctions
        protected override void OnEnable()
        {
            genericStringName = "gestureEvent";

            base.OnEnable();
        }
        #endregion
    }
}