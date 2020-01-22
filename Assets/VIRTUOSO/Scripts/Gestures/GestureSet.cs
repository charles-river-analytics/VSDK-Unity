using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Gestures
{
    /// <summary>
    /// A GestureSet is a collection of Gestures that can be enabled at the same time. They might correspond to a 
    /// system state or object state. For example, when you grab a spray bottle, the gestures you use to work with
    /// that object will be different than any other object. To include a gesture with this set, the GestureInteraction
    /// script must be made a child in the hierarchy to this script.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), September 2019
    /// </summary>
    public class GestureSet : MonoBehaviour
    {
        #region PublicVariables
        public string setName;
        public bool startOnAwake;
        #endregion

        #region PrivateVariables
        private GestureInteraction[] controlledGestures;
        #endregion

        #region PublicAPI
        public void ChangeGestureSetState(bool newState)
        {
            foreach (GestureInteraction gesture in controlledGestures)
            {
                gesture.gameObject.SetActive(newState);
            }
        }
        #endregion

        #region UnityFunctions
        private void Awake()
        {
            controlledGestures = GetComponentsInChildren<GestureInteraction>();
        }
        #endregion
    }
}