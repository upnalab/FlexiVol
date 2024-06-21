using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    public class Cube : MonoBehaviour
    {
        public GameObject parent;
        [FormerlySerializedAs("top_left_up")] public Vector3 topLeftUp = Vector3.zero;
        [FormerlySerializedAs("right_vector")] public Vector3 rightVector = Vector3.zero;
        [FormerlySerializedAs("down_vector")] public Vector3 downVector = Vector3.zero;
        [FormerlySerializedAs("forward_vector")] public Vector3 forwardVector = Vector3.zero;

        public int fill = 1;
        [FormerlySerializedAs("box_col")] public Color32 boxCol = Color.white;

        private VXCube _cube;

        // tmp values
        private Vector3 _topRight = Vector3.zero;
        private Vector3 _topDown = Vector3.zero;
        private Vector3 _topForward = Vector3.zero;
        private Vector3 _topFar = Vector3.zero;

        private Vector3 _downRight = Vector3.zero;
        private Vector3 _downForward = Vector3.zero;
        private Vector3 _downFar = Vector3.zero;

        // Use this for initialization
        private void Start()
        {
            _cube = new VXCube(topLeftUp, rightVector, forwardVector, downVector, fill, boxCol, parent);
        }

        private void Update()
        {
            _cube.Update();
        }

        private void OnDrawGizmos()
        {
            if(parent)
            {
                topLeftUp = parent.transform.position;

                Quaternion rotation = parent.transform.rotation;
                Vector3 tRight = rotation * rightVector;
                Vector3 tDown = rotation * downVector;
                Vector3 tForward = rotation * forwardVector;

                _topRight = topLeftUp + tRight;
                _topDown = topLeftUp + tDown;
                _topForward = topLeftUp + tForward;
                _topFar = topLeftUp + tRight + tForward;

                _downRight = _topDown + tRight;
                _downForward = _topDown + tForward;
                _downFar = _topDown + tRight + tForward;
            }
            else
            {
                _topRight = topLeftUp + rightVector;
                _topDown = topLeftUp + downVector;
                _topForward = topLeftUp + forwardVector;
                _topFar = topLeftUp + rightVector + forwardVector;

                _downRight = _topDown + rightVector;
                _downForward = _topDown + forwardVector;
                _downFar = _topDown + rightVector + forwardVector;
            }

            // Markers
            Gizmos.color = new Color(1 - boxCol.r, 1 - boxCol.b, 1 - boxCol.g);
            Gizmos.DrawIcon(topLeftUp, "Exclaim.tif", true);

            Gizmos.color = boxCol;
            Gizmos.DrawIcon(_topRight, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(_topForward, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(_topDown, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(_topFar, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(_downFar, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(_downRight, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(_downForward, "Light Gizmo.tiff", true);

            // Lines
            Gizmos.DrawLine(topLeftUp, _topRight);
            Gizmos.DrawLine(topLeftUp, _topDown);
            Gizmos.DrawLine(topLeftUp, _topForward);
            Gizmos.DrawLine(_downFar, _downForward);
            Gizmos.DrawLine(_downFar, _downRight);
            Gizmos.DrawLine(_downFar, _topFar);
        }

    }
}
