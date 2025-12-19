using UnityEngine;

namespace Voxon
{
    public class BoxView : IDrawable
    {
        private BoxModel _model = new BoxModel();

        #region Constructors

        public BoxView(Vector3 startPt, Vector3 endPt, int fill, Color32 col, GameObject parent = null)
        {
            // Build Data
            _model.Position = new[] { startPt, endPt};
            _model.Fill = fill;
            _model.Parent = parent;

            _model.SetColor(col);
            _model.Update();

            VXProcess.Drawables.Add(this);
        }

        public BoxView(BoxModel box)
        {
            _model = box;
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
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            _model.Update();
			VXProcess.Runtime.DrawBox(ref _model.Point3dPosition[0], ref _model.Point3dPosition[1], _model.Fill, _model.Color);
        }
        #endregion

    }
}
