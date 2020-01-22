using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Reaction to set a new material on a specified renderer. 
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class MaterialSwitchReaction : GenericReaction
    {
        #region PublicVariables
        public Material newMaterialToSwitchTo;
        public Renderer rendererToChangeMaterial;
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            if (rendererToChangeMaterial)
            {
                rendererToChangeMaterial.material = newMaterialToSwitchTo;
            }
            else
            {
                GameObject sender = o as GameObject;

                if (sender != null)
                {
                    rendererToChangeMaterial = sender.GetComponentInChildren<Renderer>();

                    if (rendererToChangeMaterial != null)
                    {
                        rendererToChangeMaterial.material = newMaterialToSwitchTo;
                    }
                }
            }
        }
        #endregion
    }
}