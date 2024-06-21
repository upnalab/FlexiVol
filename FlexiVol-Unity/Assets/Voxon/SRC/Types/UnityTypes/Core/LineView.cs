using UnityEngine;

namespace Voxon
{
    public class LineView : IDrawable
    {
        private LineModel _model = new LineModel();

        #region Constructors

        public LineView(Vector3[] points, Color32 col, GameObject parent = null)
        {
            // Build Data
            _model.Points = points;
            _model.Parent = parent;

            _model.SetColor(col);
            _model.Update();

            VXProcess.Drawables.Add(this);
        }

        public LineView(LineModel line)
        {
            _model = line;
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
            point3d[] points = _model.Point3dPoints;
            int count = points.Length - 1;
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;
            for (var idx = 0; idx < count; idx++)
            {
				VXProcess.Runtime.DrawLine(ref points[idx], ref points[idx + 1], _model.Color);
            }
        }
        #endregion

    }
}