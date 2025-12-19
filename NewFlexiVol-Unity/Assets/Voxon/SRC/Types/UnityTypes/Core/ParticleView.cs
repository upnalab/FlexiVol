using UnityEngine;

namespace Voxon
{
    public abstract class ParticleView : IDrawable
    {
        protected ParticleModel Model = new ParticleModel();

        #region Constructors

        protected ParticleView() { }

        protected ParticleView(ParticleModel particle, GameObject parent = null)
        {
            Model = particle;
            Model.Parent = parent;
            VXProcess.Drawables.Add(this);
            Model.Update();
        }

        public void Destroy()
        {
            VXProcess.Drawables.Remove(this);
        }
        #endregion

        #region drawing
        public abstract void Draw();
        #endregion

    }
}
