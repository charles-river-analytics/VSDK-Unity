using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// The component that holds the logic between the object that needs to be fired, the event to fire 
    /// the reaction on, and the list of reactions to be fired.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), Apr 2020
    /// </summary>
    public class EventBusEndpoint : MonoBehaviour
    {
        #region PublicVariables

        public string hierarchyString;
        public string eventString;
        public List<ReactionFireInfo> reactionList;

        #endregion

        #region PublicAPI

        public void FireReactions(object sender, EventArgs eventArgs)
        {
            if(isActiveAndEnabled)
            {
                for(int i = 0; i < reactionList.Count; i++)
                {
                    if(reactionList[i].fireMethod == Utilities.Constants.ReactionFireMethods.StartReaction)
                    {
                        reactionList[i].reaction.StartReaction(sender, eventArgs);
                    }
                    else
                    {
                        reactionList[i].reaction.StopReaction(sender, eventArgs);
                    }
                }
            }
        }

        #endregion

        #region UnityFunctions

        protected void OnEnable()
        {
            EventBus.Instance.SubscribeToEvent(this, hierarchyString, eventString);
        }

        protected void OnDisable()
        {
            EventBus.Instance?.UnsubscribeToEvent(this, hierarchyString, eventString);
        }

        #endregion
    }

    [Serializable]
    public struct ReactionFireInfo
    {
        public GenericReaction reaction;
        public Utilities.Constants.ReactionFireMethods fireMethod;
    }
}