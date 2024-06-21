using System;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class CubeModel
    {
        private GameObject _parent;
        private Vector3 _position = Vector3.zero;
        private Vector3 _rightVector = Vector3.zero;
        private Vector3 _forwardVector = Vector3.zero;
        private Vector3 _downVector = Vector3.zero;

        private int _fill;

        #region data_manipulation
        public void Update()
        {
            Matrix4x4 fMatrix = VXProcess.Instance.Transform;

            Vector3 v;
            if (_parent)
            {
                v = (fMatrix * (Position + _parent.transform.position));
                P3DPosition = v.ToPoint3d();

                Quaternion rotation = _parent.transform.rotation;
                v = (fMatrix * (rotation * Right));
                P3DRight = v.ToPoint3d();

                v = (fMatrix * (rotation * Down));
                P3DDown = v.ToPoint3d();

                v = (fMatrix * (rotation * Forward));
                P3DForward = v.ToPoint3d();
            }
            else
            {
                v = fMatrix * Position;
                P3DPosition = v.ToPoint3d();

                v = fMatrix * Right;
                P3DRight = v.ToPoint3d();

                v = fMatrix * Down;
                P3DDown = v.ToPoint3d();

                v = fMatrix * Forward;
                P3DForward = v.ToPoint3d();
            }

        }
        #endregion

        #region getters_setters
        public void set_color(Color32 col)
        {
            Color = (col.ToInt() & 0xffffff) >> 0;
        }

        public Vector3 Position
        {
            set
            {
                _position = value;
                Update();
            }

            get => _position;
        }

        public Vector3 Down
        {
            set
            {
                _downVector = value;
                Update();
            }
            get => _downVector;
        }

        public Vector3 Right
        {
            set
            {
                _rightVector = value;
                Update();
            }

            get => _rightVector;
        }

        public Vector3 Forward
        {
            set
            {
                _forwardVector = value;
                Update();
            }

            get => _forwardVector;
        }

        public GameObject Parent
        {
            set
            {
                _parent = value;
                Update();
            }

            get => _parent;
        }

        public int Fill
        {
            set => _fill = value;
            get => _fill;
        }

        public int Color { get; private set; }

        public point3d P3DPosition { get; private set; }

        public point3d P3DRight { get; private set; }

        public point3d P3DForward { get; private set; }

        public point3d P3DDown { get; private set; }

        #endregion
    }
}
