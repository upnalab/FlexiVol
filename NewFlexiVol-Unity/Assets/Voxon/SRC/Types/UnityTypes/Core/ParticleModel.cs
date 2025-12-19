using System;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class ParticleModel
    {
        private const float SizeModifier = 1;

        private GameObject _parent;

        private ParticleSystem _mParticleSystem;
        private ParticleSystem.Particle[] _mParticles;
        private int _nParticles;

        private Matrix4x4 _mat = Matrix4x4.identity;

        #region data_manipulation
        public void Update()
        { }
        #endregion

        #region getters_setters
        public ParticleSystem ParticleSystem
        {
            set
            {
                _mParticleSystem = value;
                ParticleSystem.MainModule main = _mParticleSystem.main;
                _mParticles = new ParticleSystem.Particle[main.maxParticles];

                // Uncomment this to use worldspace
                //main.simulationSpace = ParticleSystemSimulationSpace.World;
            }
        }
        public int ParticleCount => (_mParticleSystem != null) ? _mParticleSystem.GetParticles(_mParticles) : 0;

        public float GetParticleSize(int particleIndex)
        {
            return _mParticles[particleIndex].GetCurrentSize(_mParticleSystem) * SizeModifier;
        }
        // return particle position from local....
        public point3d GetParticle(int particleIndex)
        {
            // original  return (VXProcess.Instance.Transform * _mParticleSystem.transform.InverseTransformPoint(_mParticles[particleIndex].position)).ToPoint3d();

            return (VXProcess.Instance.Transform * _mParticleSystem.transform.TransformPoint(_mParticles[particleIndex].position)).ToPoint3d();
        }
        // retuirn particle postion from world
        public point3d GetParticleWorld(int particleIndex)
        {
            // orginal   return (VXProcess.Instance.Transform * _mParticles[particleIndex].position).ToPoint3d(); orginal



            return (VXProcess.Instance.Transform * _mParticles[particleIndex].position).ToPoint3d();
        }

        public int GetParticleColour(int particleIndex)
        {
            return ((_mParticles[particleIndex].GetCurrentColor(_mParticleSystem)).ToInt() & 0xffffff) >> 0;
        }

        public GameObject Parent
        {
            get => _parent;
            set { _parent = value; Update(); }
        }

        public Matrix4x4 GetMatrix(int particleIndex)
        {
            // comment this to use local space
            Vector3 worldPos = _mParticleSystem.transform.TransformPoint(_mParticles[particleIndex].position);
            _mat.SetTRS(worldPos, Quaternion.Euler(_mParticles[particleIndex].rotation3D), _mParticles[particleIndex].GetCurrentSize3D(_mParticleSystem) * 5);

            // Uncomment this to use worldspace
            //_mat.SetTRS(_mParticles[particleIndex].position, Quaternion.Euler(_mParticles[particleIndex].rotation3D), _mParticles[particleIndex].GetCurrentSize3D(_mParticleSystem) * 5);


            return _mat;
        }

        // get Matrix using world space
        public Matrix4x4 GetMatrixWorld(int particleIndex)
        {

            _mat.SetTRS(_mParticles[particleIndex].position, Quaternion.Euler(_mParticles[particleIndex].rotation3D), _mParticles[particleIndex].GetCurrentSize3D(_mParticleSystem) * 5);

            return _mat;
        }
        #endregion
    }
}
