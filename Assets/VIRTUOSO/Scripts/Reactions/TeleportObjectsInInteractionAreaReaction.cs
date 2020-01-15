using CharlesRiverAnalytics.Virtuoso.InteractionAreas;
using CharlesRiverAnalytics.Virtuoso.Reaction;
using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Teleports the GameObject holding an Interaction Area and all the objects inside the interaction area to
    /// a given Transform location.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class TeleportObjectsInInteractionAreaReaction : GenericReaction
    {
        #region PublicVariables
        public GameObject objectToTeleport;
        public bool teleportIAObjects;
        public Transform teleportationLocation;
        #endregion

        #region ReactionImplementation
        public override void StartReaction(object o, EventArgs e)
        {
            Vector3 originalObjectLocation = objectToTeleport.transform.position;

            objectToTeleport.transform.parent = teleportationLocation;
            objectToTeleport.transform.localPosition = Vector3.zero;
            objectToTeleport.transform.localRotation = Quaternion.identity;

            if (teleportIAObjects)
            {
                InteractionArea attachedIA = objectToTeleport.GetComponentInChildren<InteractionArea>();

                if (attachedIA != null)
                {
                    foreach (GameObject gameObjectInInteractionArea in attachedIA.ObjectsInInteractionAreaList)
                    {
                        Vector3 positionDifference = gameObjectInInteractionArea.transform.position - originalObjectLocation;

                        gameObjectInInteractionArea.transform.position = objectToTeleport.transform.position + positionDifference;
                    }
                }
            }
        }
        #endregion
    }
}