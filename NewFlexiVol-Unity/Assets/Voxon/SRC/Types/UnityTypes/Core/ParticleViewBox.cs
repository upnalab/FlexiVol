using UnityEngine;

namespace Voxon
{
    public class ParticleViewBox : ParticleView
    {

        private int reduceColorDensity = 1;
        private float sizeMultiplier = 1;
        private float zOffset = 0;
        private bool useWorldSpace = false;
        float sizeScalar = 0.05f;
        #region Constructors



        public ParticleViewBox(ParticleModel particle, bool useWorldSpace = false, int reduceColorDensity = -1, float sizeMultiplier = 1, float zOffset = 0, GameObject parent = null) : base(particle, parent) {

            this.useWorldSpace = useWorldSpace;
            this.reduceColorDensity = reduceColorDensity;
            this.sizeMultiplier = sizeMultiplier;
            this.zOffset = zOffset;

        }
        #endregion

        #region drawing
        public override void Draw()
        {
            if (VXProcess.Instance.active == false || VXProcess.Runtime == null || VXProcess.Instance.IsClosingVXProcess() == true) return;

            int particles = Model.ParticleCount;
            sizeMultiplier = 2;
 

            for (var idx = 0; idx < particles; ++idx)
            {

                float size = (Model.GetParticleSize(idx) * sizeScalar) * sizeMultiplier;
                point3d point;
                if (useWorldSpace)
                {
                    point = Model.GetParticleWorld(idx);
                }
                else
                {
                    point = Model.GetParticle(idx);
                }

                point3d min;
                min.x = point.x - size;
                min.y = point.y - size; 
                min.z = point.z - size + zOffset;

                point3d max;
                max.x = point.x + size;
                max.y = point.y + size;
                max.z = point.z + size + zOffset;

       

                // scale model
                if (reduceColorDensity != -1)
                {
                    VXProcess.Runtime.DrawBox(ref min, ref max, 2, colorHexDivide(Model.GetParticleColour(idx), reduceColorDensity));
                }
                else
                {
                    VXProcess.Runtime.DrawBox(ref min, ref max, 2, Model.GetParticleColour(idx));
                }

                
            }
        }
        #endregion

        // dims a color by a certain amount
        int colorHexDivide(int _colour, int _divideAmount)
        {
            if (_divideAmount == -1) return _colour;
            int b, g, r;

            b = (_colour & 0xFF);
            g = (_colour >> 8) & 0xFF;
            r = (_colour >> 16) & 0xFF;


            if (_divideAmount == 0) return 0;

            b /= _divideAmount;
            g /= _divideAmount;
            r /= _divideAmount;

            return (r << 16) | (g << 8) | (b);
        }
    }
}
