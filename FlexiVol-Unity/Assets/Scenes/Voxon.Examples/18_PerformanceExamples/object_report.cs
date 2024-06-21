using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._18_PerformanceExamples
{
    /// <summary>
    /// Structure to track number of instances of an object and
    /// how many triangles the base object contains.
    /// </summary>
    public struct MeshTracker
    {
        public int instanceCount;
        public int triangleCount;
    };

    /// <summary>
    /// Reports the name, instance count, and triangle count for all
    /// objects in the scene with a VXComponent
    /// </summary>
    public class object_report : MonoBehaviour
    {
        /// <summary>
        /// Call on Start.
        /// Adjust the maximum number of lines for the logger
        /// </summary>
        private void Start()
        {
            VXProcess.Instance._logger_max_lines = 1;
        }

        /// <summary>
        /// Call per Frame.
        /// Gathers all objects in Scene with VXComponent
        /// Calculated how many per name, and then lists the number of instances of each
        /// and how many triangles per instance
        /// </summary>
        void Update()
        {
            Dictionary<string, MeshTracker> objects = new Dictionary<string, MeshTracker>();

            VXComponent[] components = FindObjectsOfType<VXComponent>();

            foreach (var comp in components)
            {
                MeshFilter mf = comp.gameObject.GetComponent<MeshFilter>();
                string meshName;
                int triangle_count = 0;

                if (mf != null)
                {
                    meshName = mf.name;
                    triangle_count = mf.sharedMesh.triangles.Length;
                }
                else
                {
                    SkinnedMeshRenderer smr = comp.gameObject.GetComponent<SkinnedMeshRenderer>();
                    meshName = smr.name;
                    triangle_count = smr.sharedMesh.triangles.Length;
                }

                if (objects.ContainsKey(meshName))
                {
                    MeshTracker mt = objects[meshName];
                    mt.instanceCount += 1;
                    objects[meshName] = mt;
                }
                else
                {
                    objects[meshName] = new MeshTracker() { instanceCount = 1, triangleCount = triangle_count };
                }
            }

            string output = "";

            foreach (var comp in objects)
            {
                output += $"{comp.Value.instanceCount}   {comp.Key}: {comp.Value.triangleCount} triangles\n";
            }

            VXProcess.add_log_line(output);

        }
    }
}