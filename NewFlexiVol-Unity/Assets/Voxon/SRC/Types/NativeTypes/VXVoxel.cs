using UnityEngine;

namespace Voxon
{
    public class VXVoxel : IDrawable
    {
        VoxelView _voxelView;

        public VXVoxel(Vector3 pos, Color32 col) : this(pos, col, null)
        {
        }

        public VXVoxel(Vector3 pos, Color32 col, GameObject parent)
        {
            _voxelView = new VoxelView(pos, col, parent);
            VXProcess.Drawables.Add(this);
        }

        public void Update()
        {
        }

        public void Draw()
        {
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            _voxelView.Draw();
        }
    }
}
