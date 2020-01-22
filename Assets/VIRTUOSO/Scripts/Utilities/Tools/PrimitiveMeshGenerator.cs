using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlesRiverAnalytics.Virtuoso.Utilities
{
    /// <summary>
    /// Builds common primitive meshes for rendering (e.g. in Gizmos).
    /// 
    /// Author: Dan Duggan (dduggan@cra.com) September 2019
    /// </summary>
    public static class PrimitiveMeshGenerator
    {
        /// <summary>
        /// Generates a basic 1x1x1 Cube mesh
        /// </summary>
        public static Mesh GenerateCube()
        {
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1)
            };

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(0, 1)
            };

            int[] triangles = new int[]
            {
                // front
                0, 1, 5,
                0, 5, 4,
                // right
                4, 5, 7,
                4, 7, 6,
                // back
                6, 7, 3,
                6, 3, 2,
                // left
                2, 3, 1,
                2, 1, 0,
                // top
                3, 7, 5,
                3, 5, 1,
                // bottom
                2, 4, 6,
                2, 0, 4
            };

            Mesh result = new Mesh();
            result.vertices = vertices;
            result.uv = uvs;
            result.triangles = triangles;
            result.RecalculateNormals();
            return result;
        }

        public static Mesh GenerateInsideOutCube()
        {
            Mesh baseCube = GenerateCube();

            int[] triangles = new int[]
            {
                // front
                0, 5, 1,
                0, 4, 5,
                // right
                4, 7, 5,
                4, 6, 7,
                // back
                6, 3, 7,
                6, 2, 3,
                // left
                2, 1, 3,
                2, 0, 1,
                // top
                3, 5, 7,
                3, 1, 5,
                // bottom
                2, 6, 4,
                2, 4, 0
            };

            baseCube.triangles = triangles;
            baseCube.RecalculateNormals();

            return baseCube;
        }

        /// <summary>
        /// Generates a double-sided, 1x1, horizontal quad.
        /// </summary>
        public static Mesh GenerateQuadHorizontal()
        {
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1)
            };

            int[] triangles = new int[]
            {
                // top
                0, 2, 3,
                0, 3, 1,
                // bottom
                0, 3, 2,
                0, 1, 3
            };

            Vector2[] uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            Mesh quad = new Mesh();
            quad.vertices = vertices;
            quad.triangles = triangles;
            quad.uv = uv;
            quad.RecalculateNormals();
            return quad;
        }
    }
}