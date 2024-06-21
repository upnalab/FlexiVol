using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{

    [Serializable]
    public struct Vboi
    {
        public Vector3 vector;
        [FormerlySerializedAs("voxel_color")] public Color voxelColor;
    }

    public class Voxel : MonoBehaviour
    {
        [FormerlySerializedAs("voxel_params")] [SerializeField]
        public List<Vboi> voxelParams = new List<Vboi>();
        //public Vector3 vector = Vector3.zero;
        //public Color voxel_color = Color.white;

        private List<VXVoxel> _voxels = new List<VXVoxel>();
        // Use this for initialization
        private void Start()
        {
            foreach(Vboi vp in voxelParams)
            {
                _voxels.Add(new VXVoxel(vp.vector, vp.voxelColor));
            }
        }

        private void OnDrawGizmos()
        {
            foreach (Vboi vp in voxelParams)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawIcon(vp.vector, "Light Gizmo.tiff", false);
            }
        
        }

    }
}