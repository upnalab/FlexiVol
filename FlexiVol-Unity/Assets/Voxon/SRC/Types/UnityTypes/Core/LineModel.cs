using System;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class LineModel
    {
        private GameObject _parent;
        private Vector3[] _vPoints;
        private point3d[] _p3dPoints;

        #region data_manipulation
        public void Update()
        {
            if(_p3dPoints == null || _p3dPoints.Length != _vPoints.Length) _p3dPoints = new point3d[_vPoints.Length];

            Matrix4x4 mat = Matrix4x4.identity;
            if (_parent)
            {
                mat = _parent.transform.localToWorldMatrix;
            }

            VXProcess.ComputeTransform(ref mat, ref _vPoints, ref _p3dPoints);
        }
        #endregion

        #region getters_setters
        public Vector3[] Points
        {
            get => _vPoints;
            set { _vPoints = value; Update(); }
        }

        public point3d[] Point3dPoints => _p3dPoints;

        public GameObject Parent
        {
            get => _parent;
            set { _parent = value; Update(); }
        }

        public void SetColor(Color32 color)
        {
            Color = (color.ToInt() & 0xffffff) >> 0;
        }

        public int Color { get; private set; }

        #endregion
    }
}
