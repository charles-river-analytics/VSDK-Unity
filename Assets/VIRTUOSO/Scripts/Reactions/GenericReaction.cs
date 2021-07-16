﻿using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// The base script that any custom reaction needs to inherit from. It requires that the derive class
    /// implements two functions, {Start/Stop}Reaction. Reactions should always be thought of being independent 
    /// of the class generating the events to start the reaction, so care should be taken (but it's fine and possible)
    /// to not create any dependencies in reaction scripts.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class GenericReaction : MonoBehaviour
    {
        [Tooltip("The intended GameObject of the reaction. If not set, reaction should default to the caller of the reaction.")]
        public GameObject target;

        public virtual void StartReaction(object o, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void StopReaction(object o, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}