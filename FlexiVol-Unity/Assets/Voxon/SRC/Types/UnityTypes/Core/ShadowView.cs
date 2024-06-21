using UnityEngine;
using System.Collections;

namespace Voxon
{
    public class ShadowView : IDrawable
    {
        public ShadowModel _model = new ShadowModel();

        #region Constructors

        public ShadowView(RenderTexture Colour, RenderTexture Depth, Light Src, GameObject parent = null)
        {
			// Build Data
			_model.Position = Src.transform.position.ToPoint3d();
			_model.Forward = Src.transform.forward.ToPoint3d();
			_model.Right = Src.transform.right.ToPoint3d();
			_model.Down = (-Src.transform.up).ToPoint3d();

			_model.Parent = parent;

			_model.Colour = Colour;
			_model.Depth = Depth;

            VXProcess.Drawables.Add(this);
        }

        public ShadowView(ShadowModel model)
        {
            _model = model;
            VXProcess.Drawables.Add(this);
        }

        public void Destroy()
        {
            VXProcess.Drawables.Remove(this);
        }
        #endregion

        #region drawing
        public void Draw()
        {
			// Debug.Log($"{_model.Vertices[0]}, {_model.Vertices[1]}, {_model.Vertices[2]}, {_model.Vertices[3]}, {_model.Vertices[4]}");
			tiletype tex = _model.Texture;
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            VXProcess.Runtime.DrawTexturedMesh(ref tex, _model.Vertices, _model.Vertices.Length, null, 0, 0);
			//VXProcess.Runtime.DrawUntexturedMesh(_model.Vertices, _model.Vertices.Length, null, 0, 0, 0x404040);
		}
        #endregion

    }
}