using System;
using UnityEngine;

namespace Voxon
{
    public class ParticleViewMesh : ParticleView
    {
        private RegisteredMesh _mesh;
        private poltex[] _transformedMesh;

        private int _drawFlags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer

        private bool useWorldSpace = false;
        private int reduceColorDensity = -1;
        private float sizeMultiplier = 1;
        private float zOffset = 0;
        #region Constructors

        public ParticleViewMesh(ParticleModel particle, Mesh inMesh, bool useWorldSpace = false, int reduceColorDensity = -1, float sizeMultiplier = 1, float  zOffset = 0, GameObject parent = null) : base(particle, parent)
        {
            try
            {
                _mesh = MeshRegister.Instance.get_registed_mesh(ref inMesh);
                _transformedMesh = new poltex[_mesh.vertexCount];
                this.useWorldSpace = useWorldSpace;
                this.reduceColorDensity = reduceColorDensity;
                this.sizeMultiplier = sizeMultiplier;
                this.zOffset = zOffset;
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Loading Mesh: {inMesh.name}", e);
            }
        }
        #endregion

        //    public setLocal( bool)

        #region drawing
        public override void Draw()
        {
            if (VXProcess.Instance.active == false || VXProcess.Runtime == null || VXProcess.Instance.IsClosingVXProcess() == true) return;
        
            int particles = Model.ParticleCount;

            for (var idx = 0; idx < particles; ++idx)
            {

                Matrix4x4 matrix;

                // Unity Style
                if (useWorldSpace)
                {
                    matrix = (VXProcess.Instance.Transform * Model.GetMatrixWorld(idx)) ;
                }
                else
                {
                    matrix = VXProcess.Instance.Transform * Model.GetMatrix(idx);
                }


                _mesh.compute_transform_cpu(matrix, ref _transformedMesh);

                for (int idy = _mesh.submeshCount - 1; idy >= 0; --idy)
                {
                    if (reduceColorDensity != -1)

                        VXProcess.Runtime.DrawUntexturedMesh(_transformedMesh, _mesh.vertexCount, _mesh.indices[idy], _mesh.indexCounts[idy], _drawFlags, Model.GetParticleColour(idx));
                    else

                        VXProcess.Runtime.DrawUntexturedMesh(_transformedMesh, _mesh.vertexCount, _mesh.indices[idy], _mesh.indexCounts[idy], _drawFlags, colorHexDivide(Model.GetParticleColour(idx), reduceColorDensity));

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
