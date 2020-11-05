using System;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// TODO
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    [Serializable]
    public class EventBusHierarchy
    {
        #region PublicVariables

        public string hierarchyValue;

        #endregion

        public EventBusHierarchy(string hierarchy)
        {
            hierarchyValue = hierarchy;
        }
    }
}