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

        #region UnityFunctions
        public void Awake()
        {
            materialToChange = GetComponent<Renderer>().material;
        }
        #endregion

        #region ReactionEventImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            materialToChange.color = startColorToChangeTo;
        }

        public override void StopReaction(object o, EventArgs e)
        {
            materialToChange.color = endColorToChangeTo;
        }
        #endregion
    }
}