using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Reacts to interaction area events by changing the color of the object this is attached to.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class ColorChangeReaction : GenericReaction
    {
        #region PublicVariables

        public Color startColorToChangeTo = Color.blue;
        public Color endColorToChangeTo = Color.blue;

        #endregion

        #region ProtectedVariables

        protected Material materialToChange;

        #endregion

        #region ReactionEventImplementation

        public override void StartReaction(object o, EventArgs e)
        {
            if(target)
            {
                materialToChange = target.GetComponent<Renderer>().material;
            }
            else
            {
                materialToChange = (o as GameObject)?.GetComponent<Renderer>().material;
            }
            

            if (materialToChange != null)
            {
                materialToChange.color = startColorToChangeTo;
            }
        }

        public override void StopReaction(object o, EventArgs e)
        {
            if (materialToChange != null)
            {
                materialToChange.color = endColorToChangeTo;
            }
        }

        #endregion
    }
}