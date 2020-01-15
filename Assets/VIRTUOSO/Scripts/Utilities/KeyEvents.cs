using System;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Simple class that allows the developer to specify a key and fire off a reaction when the key is pressed down. 
    /// Helpful for debugging.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public class KeyEvents : MonoBehaviour
    {
        #region PublicVariables
        public KeyCode keyToPress = KeyCode.Space;
        #endregion

        #region Events
        public event EventHandler<EventArgs> KeyPressed;

        public void OnKeyPressed(EventArgs e)
        {
            if (KeyPressed != null)
            {
                KeyPressed(this, e);
            }
        }
        #endregion

        #region UnityFunctions
        void Update()
        {
            if (Input.GetKeyDown(keyToPress))
            {
                OnKeyPressed(new EventArgs());
            }
        }
        #endregion
    }
}