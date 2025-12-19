using System;
using UnityEngine;
using Voxon;
using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;

namespace Voxon.Examples._11_DynamicLoading
{
    /// <summary>
    /// Generates a 'pointCloud' of MeshFilter positions within a scene which can then be references
    /// to determine if a mesh is within the camera's view without access to Unity's graphics pipeline
    /// </summary>
    public class MeshReader : MonoBehaviour
    {
        /// <summary>
        /// Time before pointCloud is generated. Allows for objects generated on load to be constructed
        /// before attempting to collect all objects.
        /// </summary>
        public float delayBeforeActivation = 3.0f; // Seconds
        /// <summary>
        /// Base Radius of "Viewing" camera. Objects inside this radius from the camera will be drawn
        /// </summary>
        public float radius = 400;
        /// <summary>
        /// Camera used to display / detect mesh objects (primarily used as a position)
        /// </summary>
        public new GameObject camera;
        /// <summary>
        /// Has a point cloud been initialised yet?
        /// </summary>
        private bool added = false;

        /// <summary>
        /// List of mesh positions
        /// </summary>
        private Vector3[] pointCloud;
        /// <summary>
        /// MeshFilters of related meshes
        /// </summary>
        private MeshFilter[] meshFilters;

        /// <summary>
        /// Total number of points that can occur within a leaf node. Larger numbers reduce the time to find a result
        /// on pivot, but increase the amount of comparison at the 'leaf' level.
        /// </summary>
        int maxPointsPerLeafNode = 32;
        /// <summary>
        /// KDTree structure used to break positions into 3 dimensional binary trees.
        /// </summary>
        KDTree tree;

        /// <summary>
        /// Runs on program start, starts timer for Initialisation.
        /// </summary>
        void Start()
        {
            Invoke("Initialise", delayBeforeActivation);
        }

        /// <summary>
        /// Generates a list of all mesh locations and feeds those into a KDTree for rapid collision testing
        /// during playback.
        /// </summary>
        void Initialise() // Requires all statics to be active at this point
        {
            Debug.Log("Building Point Cloud");
            added = true;

            meshFilters = FindObjectsOfType<MeshFilter>();
            pointCloud = new Vector3[meshFilters.Length];

            for (int i = 0; i < pointCloud.Length; i++)
                pointCloud[i] = meshFilters[i].gameObject.transform.position;

            tree = new KDTree(pointCloud, maxPointsPerLeafNode);
        }

        /// <summary>
        /// Determines which meshes are inside the viewing radius and marks them
        /// active by ensuring they have a VXComponent (Drawable), or removing one
        /// if they are outside the radius.
        /// </summary>
        private void Update()
        {
            if (added)
            {
                KDQuery query = new KDQuery();
                List<int> results = new List<int>();

                Vector3 GroundSphere = camera.transform.position;
                // GroundSphere.y = 0; (force highlighting to specific plane)

                // spherical query
                query.Radius(tree, GroundSphere, radius, results);

                GameObject activeObject;
                VXComponent activeObjectComponent;
                // Debug.Log("Objects Found: " + results.Count);
                for (int i = 0; i < results.Count; ++i)
                {
                    activeObject = meshFilters[results[i]].gameObject;
                    activeObjectComponent = activeObject.GetComponent<VXComponent>();
                    if (activeObjectComponent == null)
                    {
                        activeObjectComponent = activeObject.AddComponent<VXComponent>();
                        activeObjectComponent.CanExpire = true;
                    }
                    else
                    {
                        activeObjectComponent.Refresh();
                    }
                }
            }
        }
    }
}