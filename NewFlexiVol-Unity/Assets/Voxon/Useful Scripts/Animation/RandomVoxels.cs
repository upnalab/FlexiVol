using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR

#endif

namespace Voxon.Examples.Animation
{
    // [ExecuteInEditMode]
    public class RandomVoxels : MonoBehaviour
    {
        public int voxelCount = 1000;
        //public Vector3 vector = Vector3.zero;
        //public Color voxel_color = Color.white;

        private List<VXVoxel> _voxels = new List<VXVoxel>();

        private bool _spawned = false;
        // Use this for initialization
        private void Start()
        {
            
        }

        private void Update()
        {
            if (!_spawned && VXProcess.Instance.Transform != Matrix4x4.zero)
            {
                AddVoxels();
                _spawned = true;
            }
        }

        private void AddVoxels()
        {
            for (var idx = 0; idx < voxelCount; idx++)
            {
                _voxels.Add(new VXVoxel(new Vector3(Random.Range(-5f, 5f), Random.Range(-2f, 2f), Random.Range(-5f, 5f)),
                    Color.white));
            }
        }
    }
}
