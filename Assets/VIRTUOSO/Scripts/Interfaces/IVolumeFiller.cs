using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Interfaces
{
    /// <summary>
    /// Interface that must be implemented by any interactable object that wants to interact with
    /// the volume interaction area.
    ///  
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public interface IVolume
    {
        float GetVolumeAmount();
    }
}