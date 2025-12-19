using UnityEngine;

namespace Voxon
{
    public class SphereView : IDrawable
    {
        private SphereModel _model = new SphereModel();

        #region Constructors

        public SphereView(Vector3 position, float radius, int fill, Color32 col, GameObject parent = null)
        {
            // Build Data
            _model.Position = new[] { position };
            _model.Fill = fill;
            _model.Parent = parent;
            _model.Radius = radius;

            _model.SetColor(col);
            _model.Update();

            VXProcess.Drawables.Add(this);
        }

        public SphereView(SphereModel sphere)
        {
            _model = sphere;
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
            VXProcess.Runtime.DrawSphere(ref _model.Point3dPosition[0], _model.Radius, _model.Fill, _model.Color);
        }
        #endregion

    }
}
