using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Scriptable
{
    /// <summary>
    /// Contains all the necessary start up info for creating a line renderer in a script. Since
    /// a line renderer requires a material component, by having a simple ScriptableObject, any
    /// line renderer that reuses the scriptable object has a good amount of memory savings by
    /// referencing the same material (and it makes set up easy with the line renderer with reuse).
    /// 
    /// Written by: Nicolas Herrera (nherrera@cra.com), 2018
    /// </summary>
    [CreateAssetMenu(fileName = "New Line Renderer", menuName = "VIRTUOSO/Create Line")]
    public class ScriptableLineRenderer : ScriptableObject
    {
        public Material lineMaterial;
        public Color lineStartColor;
        public Color lineEndColor;
        public float startLineWidth;
        public float endLineWidth;
        public float widthMultiplier;
        public int numCornerVertices;
        public int numCapVertices;
        public bool receiveShadows;
        public bool allowOcclusionWhenDynamic;
    }
}