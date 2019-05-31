using System;
using System.Collections.Generic;

namespace CharlesRiverAnalytics.Virtuoso.Reaction
{
    /// <summary>
    /// Helper class to hold various information for the event-reaction pair in the editor.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), December 2018
    /// </summary>
    [Serializable]
    public class ReactorEditorInfo
    {
        #region PublicVariables
        public bool foldoutOpenStatus;
        public List<GenericReaction> reactionList;
        public List<string> reactionTriggerMethodList;
        public List<int> reactionTriggerIndexSelectionList;
        #endregion

        #region HelperMethods
        public void RemoveElementsAt(int index)
        {
            reactionList.RemoveAt(index);
            reactionTriggerMethodList.RemoveAt(index);
            reactionTriggerIndexSelectionList.RemoveAt(index);
        }
        #endregion
    }
}
