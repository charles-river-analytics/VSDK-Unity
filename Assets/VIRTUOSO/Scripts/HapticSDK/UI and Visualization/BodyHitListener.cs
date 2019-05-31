#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace CharlesRiverAnalytics.Virtuoso.Haptic
{
    /// <summary>
    /// Works with the HapticPatternWindow class to work in the In-Scene UI for creating haptic
    /// pattern. This class is responsible for listening for double clicks on the inflection
    /// points for the pattern. If they are double clicked on, it will open up a menu that will
    /// allow the developer to edit the values for the point.
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2019
    /// </summary>
    public sealed class BodyHitListener : IDisposable
    {
        #region PrivateVariables
        private BodyHitUI lastUIHit;
        #endregion

        #region BodyHitMethods
        public BodyHitListener()
        {
            SceneView.onSceneGUIDelegate += GetMouseClicks;
        }

        public void Dispose()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            SceneView.onSceneGUIDelegate -= GetMouseClicks;
        }
        #endregion

        #region ListenerMethods
        public void GetMouseClicks(SceneView sceneView)
        {
            // Only care about mouse down events
            if (Event.current.type != EventType.MouseDown)
            {
                return;
            }

            // convert GUI coordinates to screen coordinates
            Vector3 screenPosition = Event.current.mousePosition;
            Vector3 cameraScreenPosition = screenPosition;
            cameraScreenPosition.y = Camera.current.pixelHeight - cameraScreenPosition.y;

            Ray ray = Camera.current.ScreenPointToRay(cameraScreenPosition);
            RaycastHit hit;

            // Wait for double clicks on left mouse
            if (Event.current.clickCount == 2 && Event.current.button == 0)
            {
                // Cast ray into the scene
                if (Physics.Raycast(ray, out hit))
                {
                    Event.current.Use();

                    BodyCoordinate bodyPart = hit.collider.GetComponent<BodyCoordinate>();

                    if (bodyPart != null)
                    {
                        BodyCoordinateHit hitLocation = bodyPart.CalculateBodyCoordinateHitFromPosition(hit.point);

                        OnBodyPartHit(bodyPart, hitLocation, hit.point);

                        return;
                    }

                    BodyHitUI uiHit = hit.collider.GetComponent<BodyHitUI>();

                    if (uiHit != null)
                    {
                        // Make sure that another UI is not already displaying
                        lastUIHit?.HideUI(screenPosition, true);

                        lastUIHit = uiHit;

                        lastUIHit.DisplayUI(screenPosition);
                        return;
                    }
                }
            }
            else if (Event.current.clickCount == 1)
            {
                // Close the hitUI if they click outside of it
                if (lastUIHit != null)
                {
                    // Right/middle clicks will force close the UI
                    bool wasHidden = (Event.current.button != 0) ? lastUIHit.HideUI(screenPosition, true) : lastUIHit.HideUI(screenPosition);

                    if (wasHidden)
                    {
                        lastUIHit = null;
                    }
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<HapticInformation> BodyPartHit;

        public void OnBodyPartHit(BodyCoordinate bodyCoordinate, BodyCoordinateHit hitInfo, Vector3 hitLocation)
        {
            if (BodyPartHit != null)
            {
                BodyPartHit(this, new HapticInformation(bodyCoordinate, hitInfo, hitLocation));
            }
        }

        public class HapticInformation : EventArgs
        {
            public BodyCoordinate bodyPart;
            public BodyCoordinateHit bodyHitInfo;
            public Vector3 worldHitLocation;

            public HapticInformation(BodyCoordinate bodyCoordinate, BodyCoordinateHit hitInfo, Vector3 hitLocation)
            {
                bodyPart = bodyCoordinate;
                bodyHitInfo = hitInfo;
                worldHitLocation = hitLocation;
            }
        }
        #endregion
    }
}
#endif