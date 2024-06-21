using UnityEngine;

namespace Voxon
{
    public class ParticleViewSphere : ParticleView
    {

        int reduceColorDensity = 1;
        float sizeMultiplier = 1;
        float zOffset = 0;
        private bool useWorldSpace = false;

        float sizeScalar = 0.1f;
        #region Constructors


        public ParticleViewSphere(ParticleModel particle, bool useWorldSpace = false, int reduceColorDensity = -1, float sizeMultiplier = 1, float zOffset = 0, GameObject parent = null) : base(particle, parent)
        {

            this.useWorldSpace = useWorldSpace;
            this.reduceColorDensity = reduceColorDensity;
            this.sizeMultiplier = sizeMultiplier;
            this.zOffset = zOffset;

        }



        poltex[] poltex_buffer;
        #endregion

        #region drawing
        public override void Draw()
        {
            if (VXProcess.Instance.active == false || VXProcess.Runtime == null || VXProcess.Instance.IsClosingVXProcess() == true) return;


            int particles = Model.ParticleCount;
            poltex_buffer = new poltex[Model.ParticleCount];
            point3d point;
            float size = .0001f; // about a 1 : 1 size 
        
            for (var idx = 0; idx < particles; ++idx)
            {
                size = (Model.GetParticleSize(idx) * sizeScalar) * this.sizeMultiplier;

                if (useWorldSpace) point = Model.GetParticleWorld(idx);
                else
                    point = Model.GetParticle(idx);
                
                poltex_buffer[idx].x = point.x;
                poltex_buffer[idx].y = point.y;
                poltex_buffer[idx].z = point.z + zOffset;
                if (reduceColorDensity != -1) 
                    poltex_buffer[idx].col = colorHexDivide(Model.GetParticleColour(idx), reduceColorDensity);
                else
                    poltex_buffer[idx].col = Model.GetParticleColour(idx);
                

            }

            VXProcess.Runtime.DrawSphereBulk(poltex_buffer, size); 

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
