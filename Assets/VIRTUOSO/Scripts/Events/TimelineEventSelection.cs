using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CharlesRiverAnalytics.Virtuoso
{
    [Serializable]
    public class TimelineEventSelection
    {
        // The actual object that will give off the event
        public VirtuosoEvent providedEvent;
        // Hold the string value of an enum and convert it when the type is known at runtime
        public string eventToListen;

        // Used for data persistence in the property drawer, stores the popup index so it can survive the serialization process
        public int selectedPopupValue;
    }
}