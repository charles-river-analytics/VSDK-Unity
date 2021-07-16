using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Helper utility for constants that may be used in several files, defined here so they
    /// will not exist in multiple places.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public static class Constants
    {
        public const float MS_TO_SECONDS = 1000.0f;
        public const float FT_TO_M = 0.3048f;
        public const int MAX_OPENVR_OBJECTS = 16;

        public static string EditorPrefLocation
        {
            get
            {
                return Application.productName + ".VIRTUOSO.";
            }
        }

        public static readonly string[] reactionTriggerMethods = { "StartReaction", "StopReaction" };

        public enum ReactionFireMethods
        {
            StartReaction,
            StopReaction
        }
    }
}