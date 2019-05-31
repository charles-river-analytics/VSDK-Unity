using System;

namespace CharlesRiverAnalytics.Virtuoso
{
    /// <summary>
    /// A helper utility for the InteractionAreaReaction and GenericReaction scripts. Since some GenericReactions have an
    /// empty StopReaction, use this script to hide the corresponding variable to set that method in any of the 
    /// general reaction scripts.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HideMethodFromInspectorAttribute : Attribute
    {
        // No logic is needed as the presence of this attribute indicates it should be hidden
    }
}