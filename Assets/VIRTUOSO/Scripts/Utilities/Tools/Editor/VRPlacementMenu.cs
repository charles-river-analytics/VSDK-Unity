using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Utility for placing objects in the editor.
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) Updated: January 2020
    /// </summary>
    public class VRPlacementMenu : MonoBehaviour
    {
        static void PlaceSelectedObjectAtHeight(float height)
        {
            if (Selection.activeTransform != null)
            {
                Vector3 newPosition = Selection.activeTransform.position;
                newPosition.y = height;
                Selection.activeTransform.position = newPosition;
            }
        }

        [MenuItem("VIRTUOSO/Place Selected Object/Table Height")]
        static void PlaceSelectedTableHeight()
        {
            PlaceSelectedObjectAtHeight(VRGuideConstants.TABLE_HEIGHT);
        }

        [MenuItem("VIRTUOSO/Place Selected Object/Counter Height")]
        static void PlaceSelectedCounterHeight()
        {
            PlaceSelectedObjectAtHeight(VRGuideConstants.COUNTER_HEIGHT);
        }

        [MenuItem("VIRTUOSO/Place Selected Object/Bar Height")]
        static void PlaceSelectedBarHeight()
        {
            PlaceSelectedObjectAtHeight(VRGuideConstants.BAR_HEIGHT);
        }

        [MenuItem("VIRTUOSO/Place Selected Object/Cabinet Height")]
        static void PlaceSelectedCabinetHeight()
        {
            PlaceSelectedObjectAtHeight(VRGuideConstants.CABINET_HEIGHT);
        }
    }
}
