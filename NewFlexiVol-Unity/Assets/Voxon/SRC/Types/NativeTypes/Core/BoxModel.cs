using System;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class BoxModel
    {
        GameObject _parent;
        Vector3[] _vPosition = new Vector3[2];
        point3d[] _p3dPosition = new point3d[2];
        int _fill;

        #region data_manipulation
        public void Update()
        {
            Matrix4x4 mat = Matrix4x4.identity;
            if (_parent)
            {
                mat = _parent.transform.localToWorldMatrix;
            }

            VXProcess.ComputeTransform(ref mat, ref _vPosition, ref _p3dPosition);
        }
        #endregion

        #region getters_setters
        public Vector3[] Position
        {
            get => _vPosition;
            set { _vPosition = value; Update(); }
        }

        public point3d[] Point3dPosition => _p3dPosition;

        public GameObject Parent
        {
            get => _parent;
            set { _parent = value; Update(); }
        }

        public int Fill
        {
            set => _fill = value;
            get => _fill;
        }

        public void SetColor(Color32 color)
        {
            Color = (color.ToInt() & 0xffffff) >> 0;
        }

        public int Color { get; private set; }

        #endregion
    }
}
