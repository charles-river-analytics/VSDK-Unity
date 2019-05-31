using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the enums that are used throughout the Haptic SDK.
/// 
/// Written by: Nicolas Herrera (nherrera@cra.com), 2019
/// </summary>
namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    public enum OffsetUse
    {
        // Don't use the offset to play the pattern
        Ignore,
        // The first point in the pattern will start at the offset
        SetAtFirstPoint
    }

    public enum PatternOvershootResolution
    {
        // Any value outside the range will be ignored
        Discard,
        // Any value outside the range will be set to the max/min value
        Clamp,
        // Any value outside the range will be set to the opposite 
        Wrap
    }

    public enum PatternCollisionResolution
    {
        // Use the minium value from all curves
        Min,
        // Use the maximum value from all curves
        Max,
        // Take the average value from all curves
        Average,
        // Add all the values from all curves
        Add,
        // Multiple the values from all curves
        Multiply,
        // One specified curve value will always be used
        CurvePriority
    }

    public enum PlaybackTiming
    {
        // Use the Update loop to play the pattern
        Update,
        // Use the FixedUpdate loop to play the pattern
        FixedUpdate,
        // Use a custom timing scheme to play the pattern
        Custom
    }
}