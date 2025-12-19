using UnityEngine;

namespace Voxon
{
    public class VXCube : IDrawable
    {
        CubeModel _cubeModel;

        public VXCube(Vector3 pp, Vector3 pr, Vector3 pf, Vector3 pd, int fill, Color32 col) : this(pp, pr, pf, pd, fill, col, null)
        {
        }

        public VXCube(Vector3 pp, Vector3 pr, Vector3 pf, Vector3 pd, int fill, Color32 col, GameObject parent)
        {
            _cubeModel = new CubeModel
            {
                Position = pp,
                Right = pr,
                Forward = pf,
                Down = pd,
                Fill = fill,
                Parent = parent
            };


            _cubeModel.set_color(col);

            VXProcess.Drawables.Add(this);
        }

        public void Update()
        {
            _cubeModel.Update();
        }

        public void Draw()
        {
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            point3d[] point3D = new[] { _cubeModel.P3DPosition, _cubeModel.P3DRight, _cubeModel.P3DDown, _cubeModel.P3DForward };
            VXProcess.Runtime.DrawCube(ref point3D[0], ref point3D[1], ref point3D[2], ref point3D[3], _cubeModel.Fill, _cubeModel.Color);
        }
    }
}
