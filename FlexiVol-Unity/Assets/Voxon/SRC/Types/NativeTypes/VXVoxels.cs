using System;
using UnityEngine;

namespace Voxon
{
    public class VXVoxels : IDrawable
    {
        private Vector3[] _points;

        private point3d[] _voxelPositions;
        private int _voxelCount = 0;
        private Int32[] _voxelColors;

        public VXVoxels(ref Vector3[] points, ref Color32[] colors)
        {
            if (points.Length != colors.Length)
            {
                Debug.LogError("VXVoxels: Point size != Color size");
            }
            
            set_points(ref points);
            set_colors(ref colors);

            VXProcess.Drawables.Add(this);
        }

        public void Draw()
        {
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            VXProcess.Runtime.DrawVoxels(ref _voxelPositions, _voxelCount, ref _voxelColors);
        }

        public void set_icolor(int idx, int value)
        {
            // Debug.Log(value);
            _voxelColors[idx] = value;
        }
        public void set_colors(ref Color32[] cols)
        {
            int count = cols.Length;
            _voxelColors = new Int32[count];
            for (int i = 0; i < count; ++i)
            {
                _voxelColors[i] = (cols[i].ToInt() & 0xffffff);
            }
        }

		public void set_posz(int idx, float y)
		{
			// Debug.Log(value);
			_voxelPositions[idx].z = y;
		}

		public void set_points(ref Vector3[] points)
        {
            this._voxelCount = points.Length;
            this._points = points;
            this._voxelPositions = new point3d[_voxelCount];
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