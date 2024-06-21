using UnityEngine;

namespace Voxon
{
    public class BoxGizmoView
    {
        private BoxGizmoModel _boxGizmoModel;

        public BoxGizmoView(BoxModel boxModel)
        {
            _boxGizmoModel = new BoxGizmoModel(boxModel);
        }

        public void DrawGizmo()
        {
            _boxGizmoModel.Update();
            foreach (Vector3 corner in _boxGizmoModel.Corners)
            {
                Gizmos.DrawIcon(corner, "Negative.tif", true);
            }

            Gizmos.DrawCube(_boxGizmoModel.Center, _boxGizmoModel.Size);
        }
    }
}

