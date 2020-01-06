using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// A destructive reaction that finds all objects of a specific tag and uses the Destroy function
    /// to remove them. Should not be used with objects in an object pool.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class DestroyAllByTagReaction : GenericReaction
    {
        #region PublicVariables
        public string tagToDestroy;
        #endregion
        
        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            if(!string.IsNullOrEmpty(tagToDestroy))
            {
                GameObject[] allGameObjectsWithTag = GameObject.FindGameObjectsWithTag(tagToDestroy);

                foreach(GameObject destroyThisObject in allGameObjectsWithTag)
                {
                    Destroy(destroyThisObject);
                }
            }
        }
        #endregion
    }
}