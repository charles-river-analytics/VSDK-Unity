using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Events
{
    /// <summary>
    /// TODO
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
            // TODO unsubscribe? What if similiar 
        }

        #endregion
    }

    [Serializable]
    public struct ReactionFireInfo
    {
        public GenericReaction reaction;
        // TODO If HideMethodAttribute is used, it will be ignored. Make custom Property drawer?
        public Utilities.Constants.ReactionFireMethods fireMethod;
    }
}