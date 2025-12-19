using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._22_DynamicMeshDeformation
{
    /// <summary>
    /// Creates a mesh from scratch, and then allows the mesh
    /// to be changed and updated by external scripts
    /// </summary>
    public class MeshGenerator : MonoBehaviour
    {
        /// <summary>
        /// Generated Mesh
        /// </summary>
        Mesh mesh;
        /// <summary>
        /// Generated MeshFilter
        /// </summary>
        MeshFilter mf;

        /// <summary>
        /// Generated MeshRenderer
        /// </summary>
        MeshRenderer mr;

        /// <summary>
        /// Vertices of Mesh
        /// </summary>
        Vector3[] vertices;
        /// <summary>
        /// Indices of Mesh
        /// </summary>
        int[] triangles;

        /// <summary>
        /// Called on Start.
        /// Generates Mesh, MeshFilter, MeshRenderer and attaches a
        /// VXDynaminComponent
        /// </summary>
        void Start()
        {
            mf = gameObject.AddComponent<MeshFilter>();
            mesh = new Mesh();
            mf.mesh = mesh;

            mr = gameObject.AddComponent<MeshRenderer>();

            CreateShape();
            UpdateMesh();

            gameObject.AddComponent<Voxon.VXDynamicComponent>();
        }

        /// <summary>
        /// Generates a quad's vertices, indices
        /// </summary>
        void CreateShape()
        {
            vertices = new Vector3[]
            {

            new Vector3 (0,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,0),
            new Vector3 (1,0,1)


            };

            triangles = new int[]
            {
            0,1,2,
            1,3,2
            };

        }

        /// <summary>
        /// Update Mesh based on current Vertices and Indices (triangles)
        /// </summary>
        void UpdateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }
    }
}