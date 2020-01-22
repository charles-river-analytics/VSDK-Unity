using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// This class provides guides in the editor to help developers place objects for VR experiences.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) Updated January 2020
    /// </summary>
    public class VRObjectGuide : MonoBehaviour
    {
        #region Settings
        public bool drawOnlyWhenSelected = false;

        public bool drawPlayArea = true;
        public bool extractPlayAreaSizeOnRun = true;
        public Vector3 playAreaSize = new Vector3(2, VRGuideConstants.CEILING_HEIGHT, 2);
        public Color playAreaColor = new Color(0, 1, 1, 0.25f);

        public bool drawTableGuide = true;
        public Color tableGuideColor = new Color(1.0f, 0, 0, 0.25f);

        public bool drawCounterGuide = true;
        public Color counterGuideColor = new Color(1.0f, 0, 1.0f, 0.25f);

        public bool drawBarGuide = true;
        public Color barGuideColor = new Color(1.0f, 1.0f, 0, 0.25f);

        public bool drawCabinetGuide = true;
        public Color cabinetGuideColor = new Color(0, 1.0f, 0, 0.25f);

        #endregion

        #region Gizmos Methods
        private void OnDrawGizmos()
        {
            if (! drawOnlyWhenSelected)
            {
                DrawGuides();
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGuides();
        }

        protected void DrawGuides()
        {
            DrawPlayArea();
            if(drawTableGuide)
            {
                DrawSurfaceGuide(VRGuideConstants.TABLE_HEIGHT, tableGuideColor);
            }
            if(drawCounterGuide)
            {
                DrawSurfaceGuide(VRGuideConstants.COUNTER_HEIGHT, counterGuideColor);
            }
            if(drawBarGuide)
            {
                DrawSurfaceGuide(VRGuideConstants.BAR_HEIGHT, barGuideColor);
            }
            if(drawCabinetGuide)
            {
                DrawSurfaceGuide(VRGuideConstants.CABINET_HEIGHT, cabinetGuideColor);
            }
        }

        private void DrawPlayArea()
        {
            if (drawPlayArea)
            {
                Vector3 objectCenter = GetCenter();
                Mesh playAreaCube = PrimitiveMeshGenerator.GenerateInsideOutCube();
                Gizmos.color = playAreaColor;
                if(extractPlayAreaSizeOnRun && PlayerPrefs.HasKey("PlayAreaSize_X") && PlayerPrefs.HasKey("PlayAreaSize_Z"))
                {
                    playAreaSize = new Vector3(PlayerPrefs.GetFloat("PlayAreaSize_X"), VRGuideConstants.CEILING_HEIGHT, PlayerPrefs.GetFloat("PlayAreaSize_Z"));
                }
                Gizmos.DrawMesh(playAreaCube, objectCenter, transform.rotation, playAreaSize);
            }
        }

        private void DrawSurfaceGuide(float height, Color color)
        {

            Vector3 objectCenter = GetCenter() + new Vector3(0, height, 0);
            Mesh surfaceHeightGuideMesh = PrimitiveMeshGenerator.GenerateQuadHorizontal();
            Gizmos.color = color;
            Gizmos.DrawMesh(surfaceHeightGuideMesh, objectCenter, transform.rotation, playAreaSize);
        }

        private Vector3 GetCenter()
        {
            Vector3 objectCenter = transform.position - playAreaSize / 2.0f;
            objectCenter.y = 0;
            return objectCenter;
        }

        private void Update()
        {
            if(extractPlayAreaSizeOnRun)
            {
                Vector3[] boundaryEdge = VRTK.VRTK_SDK_Bridge.GetBoundariesSDK()?.GetPlayAreaVertices();
                if (boundaryEdge == null)
                    return;
                // find min x, min z, max x, and max z: these will be used to set the size of the bounds
                float minX = float.MaxValue;
                float minZ = float.MaxValue;
                float maxX = float.MinValue;
                float maxZ = float.MinValue;
                foreach(Vector3 point in boundaryEdge)
                {
                    if(point.x < minX)
                    {
                        minX = point.x;
                    }
                    if(point.x > maxX)
                    {
                        maxX = point.x;
                    }
                    if(point.z < minZ)
                    {
                        minZ = point.z;
                    }
                    if(point.z > maxZ)
                    {
                        maxZ = point.z;
                    }
                }

                Vector3 size = new Vector3(maxX - minX, VRGuideConstants.CEILING_HEIGHT, maxZ - minZ);
                PlayerPrefs.SetFloat("PlayAreaSize_X", size.x);
                PlayerPrefs.SetFloat("PlayAreaSize_Z", size.z);
                PlayerPrefs.Save();
            }
        }
        #endregion
    }
}
