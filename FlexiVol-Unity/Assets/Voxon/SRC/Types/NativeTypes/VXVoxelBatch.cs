using System;
using UnityEngine;

namespace Voxon
{
    public class VXVoxelBatch : IDrawable
    {
        private Vector3[] _points;

        private point3d[] _voxelPositions;
        private int _voxelCount = 0;
        private Int32 _col = 0;

        public VXVoxelBatch(ref Vector3[] points, Color32 col)
        {
            set_points(ref points);
            set_color(col);

            VXProcess.Drawables.Add(this);
        }

        public void Draw()
        {
            // Debug.Log(_voxelPositions.Length + " : " +  _voxelCount + " : Color: " + _col);
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            VXProcess.Runtime.DrawVoxelBatch(ref _voxelPositions, _voxelCount, _col);
        }
            
        public void set_color(Color32 col)
        {
            _col = (col.ToInt() & 0xffffff);
        }

        public void set_points(ref Vector3[] points)
        {
            this._voxelCount = points.Length;
            this._points = points;
            this._voxelPositions = new point3d[this._voxelCount];
            update_transform();
        }

        public void update_transform()
        {
            Matrix4x4 component = VXProcess.Instance.Transform;

            Vector4 inV = Vector4.one;
            for (int idx = 0; idx < _voxelCount; ++idx)
            {
                inV.x = _points[idx].x;
                inV.y = _points[idx].y;
                inV.z = _points[idx].z;
            
                inV = component * inV;
                
                _voxelPositions[idx] = inV.ToPoint3d();
            }
            
        }
    }
}