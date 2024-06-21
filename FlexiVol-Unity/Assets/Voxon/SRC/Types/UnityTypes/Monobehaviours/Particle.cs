using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;


namespace Voxon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particle : MonoBehaviour
    {
        public enum ParticleStyle
        {
            BILLBOARD,
            BOX,
            MESH,
            SPHERE
        };

        public bool useWorldSpace = false;

        [Range(-1, 30)]
        [Tooltip("Variable used to reduce the color density of the particle - higher values a dimmer particle. -1 is off")]
        public int reduceColorDensity = -1;

        [Range(-0.001f, 10)]
        [Tooltip("Adjust the size of the particle based on a multiplier from the base scale, 1 is natural. Decimal numbers make the particle smaller.")]
        public float sizeMultiplier = 1;

        [Range(-1, 1)]
        [Tooltip("Tweak the height of the particle. As certain Z aspect ratios can be a bit off. 0 is natural")]
        public float zOffset = 0;


        // Editor View (Won't use after initialization)
        [FormerlySerializedAs("particle_style")] public ParticleStyle particleStyle = ParticleStyle.BILLBOARD;

        // Associated Models
        private ParticleModel _particleModel;

        // Associated Views
        private ParticleView _particle;

        // Use this for initialization
        private void Start()
        {
            try
            {
                var ps = GetComponent<ParticleSystem>();
                _particleModel = new ParticleModel { ParticleSystem = ps };
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"({name}) Failed to load suitable Line", e);
                Destroy(this);
            }

            // TODO : port particle system with extra values to other types

            switch (particleStyle)
            {
                case ParticleStyle.BILLBOARD:
                    _particle = new ParticleViewBillBoard(_particleModel, useWorldSpace, reduceColorDensity, sizeMultiplier, zOffset);
                    break;
                case ParticleStyle.BOX:
                    _particle = new ParticleViewBox(_particleModel, useWorldSpace, reduceColorDensity, sizeMultiplier, zOffset);
                    break;
                case ParticleStyle.SPHERE:
                    _particle = new ParticleViewSphere(_particleModel, useWorldSpace, reduceColorDensity, sizeMultiplier, zOffset);
                    break;
                case ParticleStyle.MESH:
                    _particle = new ParticleViewMesh(_particleModel, GetComponent<ParticleSystemRenderer>().mesh, useWorldSpace, reduceColorDensity, sizeMultiplier, zOffset);
                    break;
            }

            if (_particle != null && _particleModel != null) return;

            Debug.LogError($"Particle.cs - Particle Error? {(_particle != null)} ParticleModel? {(_particleModel != null)}");
            Destroy(this);
        }
    }
}