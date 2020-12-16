using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// Allows a GameObject to have a string label for categorizing the GameObject
    /// to a known interactable objects in a weak reference within the Event Bus system.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2020
    /// </summary>
    public class EventBusHierarchy : MonoBehaviour
    {
        #region PublicVariables

        public string hierarchyValue;

        #endregion

        #region PrivateVariables

        private string[] hierarchyCombinations;
        private char hierarchySeperator = '/';

        #endregion

        #region PublicAPI

        /// <summary>
        /// Returns the hierarchyValue as the broken up string values seperated by the '/'
        /// </summary>
        public string[] GetSplitHierarchy()
        {
            return hierarchyValue.Split(hierarchySeperator);
        }

        /// <summary>
        /// Returns an array of strings of every combination of the hierarchy
        /// i.e. for the hierarchy Obj/Gun/Shotgun would return [Obj, Obj/Gun, and Obj/Gun/Shotgun]
        /// </summary>
        public string[] GetHierarchyCombination()
        {
            if(hierarchyCombinations == null)
            {
                string[] splitHierarchy = GetSplitHierarchy();
                hierarchyCombinations = new string[splitHierarchy.Length];

                for (int i = 0; i < splitHierarchy.Length; i++)
                {
                    string currentString = "";

                    for (int j = i; j >= 0; j--)
                    {
                        currentString += splitHierarchy[i - j];

                        if (j != 0)
                        {
                            currentString += hierarchySeperator;
                        }
                    }

                    hierarchyCombinations[i] = currentString;
                }
            }

            return hierarchyCombinations;
        }

        #endregion

        #region UnityEvents

        public void Start()
        {
            EventBus.Instance.AddKnownObject(this);
        }

        #endregion
    }
}