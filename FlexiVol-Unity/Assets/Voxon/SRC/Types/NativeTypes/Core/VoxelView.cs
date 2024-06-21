using UnityEngine;

namespace Voxon
{
    public class VoxelView : IDrawable
    {
        private VoxelModel _model = new VoxelModel();

        #region Constructors

        public VoxelView(Vector3 position, Color32 col, GameObject parent = null)
        {
            // Build Data
            _model.Position = new[] { position };
            _model.Parent = parent;

            _model.SetColor(col);
            _model.Update();

            VXProcess.Drawables.Add(this);
        }

        public VoxelView(VoxelModel voxel)
        {
            _model = voxel;
            VXProcess.Drawables.Add(this);
            _model.Update();
        }

        public void Destroy()
        {
            VXProcess.Drawables.Remove(this);
        }
        #endregion

        #region drawing
        public void Draw()
        {
            _model.Update();
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            VXProcess.Runtime.DrawVoxel(ref _model.Point3dPosition[0], _model.Color);
        }
        #endregion

    }
}
