using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.InteractionAreas
{
    /// <summary>
    /// InteractionArea that manages multiple children interaction areas. There is no trigger on this collider, instead it requires every
    /// child to fire their finish interaction area once. Order can be given as well which only allows the next interaction area to fire, 
    /// disabling the future ones until it is their time.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    public class MultiInteractionArea : InteractionArea
    {
        #region PublicVariables
        public bool enforceOrder = false;
        #endregion

        #region PublicProperties
        public InteractionArea NextInteractionArea
        {
            get
            {
                return childrenAreas[completionCount];
            }
        }
        #endregion

        #region ProtectedVariables
        protected int completionCount = 0;
        protected List<InteractionArea> childrenAreas;
        #endregion

        #region UnityFunctions
        protected override void Awake()
        {
            InteractionArea[] allComponentsInChildren = GetComponentsInChildren<InteractionArea>();
            childrenAreas = new List<InteractionArea>();

            // You only need to copy the ones that are direct children of you, no grandchildren and do not keep yourself (so skip index 0)
            for(int n = 1; n < allComponentsInChildren.Length; n++)
            {
                if(allComponentsInChildren[n].transform.parent == transform)
                {
                    childrenAreas.Add(allComponentsInChildren[n]);

                    // Make sure your children have a copy of your VRTK Policy and not some other
                    allComponentsInChildren[n].GetComponent<InteractionArea>().validObjectListPolicy = GetComponent<VRTK.VRTK_PolicyList>();
                }
            }

            // For each child, hook into their interaction area finishing
            for(int n = 0; n < childrenAreas.Count; n++)
            {
                childrenAreas[n].ObjectFinishedInteractionArea += ChildInteractionFinished;
            }

            // Leaves only the first interaction area enabled
            if(enforceOrder)
            {
                SwitchChildren(0);
            }
        }
        #endregion

        #region MultiInteractionAreaFunctions
        protected void ChildInteractionFinished(object o, InteractionAreaEventArgs e)
        {
            int childIndex = (o as InteractionArea).transform.GetSiblingIndex();

            childrenAreas[childIndex].ObjectFinishedInteractionArea -= ChildInteractionFinished;

            // Disable the collider on the interaction area so no one else can hit it
            Collider childCollider = childrenAreas[childIndex].gameObject.GetComponent<Collider>();

            if (childCollider != null)
            {
                childCollider.enabled = false;
            }

            completionCount++;

            if(completionCount == childrenAreas.Count)
            {
                OnObjectFinishedInteractionArea(SetInteractionAreaEvent(gameObject));
            }
            else if (enforceOrder)
            {
                SwitchChildren(completionCount);
            }
        }

        // Enables the given child and deactives all others
        protected void SwitchChildren(int activeChild)
        {
            for(int n = 0; n < childrenAreas.Count; n++)
            {
                if(n == activeChild)
                {
                    childrenAreas[n].gameObject.SetActive(true);
                }
                // Temp work around - if n was not greater then snap interaction areas would disappear
                else if (n > activeChild)
                {
                    childrenAreas[n].gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}
