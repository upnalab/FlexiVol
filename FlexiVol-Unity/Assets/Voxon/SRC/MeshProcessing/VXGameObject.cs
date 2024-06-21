using UnityEngine;

namespace Voxon
{
    public class VXGameObject : MonoBehaviour
    {

        // Lifespan Variables
        private const float MaxLifeSpan = 300.0f;
        private bool _canDegen = true;
        private bool _degen;
        private float _lifeSpan = MaxLifeSpan;

        public void Start()
        {
            // We always want animations to be computed (as otherwise they would only appear when a camera was active)
            if (gameObject.GetComponent<Animator>())
            {
                var anima = transform.root.gameObject.GetComponent<Animator>();
                if(anima)
                {
                    anima.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }

                Animator[] animators = transform.root.gameObject.GetComponentsInChildren<Animator>();
                foreach(Animator a in animators)
                {
                    a.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }
            }

            foreach (Renderer child in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (child.gameObject.CompareTag("VoxieHide"))
                {
                    continue;
                }
                if(child.gameObject.GetComponent<ParticleSystem>() || child.gameObject.GetComponent<LineRenderer>())
                {
                    continue;
                }
                // Child will add self to VXProcess _components
                if (!child.gameObject.GetComponent<VXComponent>())
                {
                    child.gameObject.AddComponent<VXComponent>();
                }                
            }

            VXProcess.Gameobjects.Add(this);
        }

        public void OnDestroy()
        {
            foreach (Renderer child in gameObject.GetComponentsInChildren<Renderer>())
            {
                Destroy(child.gameObject.GetComponent<VXComponent>());
            }
        }

        /// <summary>  
        ///  To reduce load on VX1, we want Drawables to be removed a few seconds off screen.
        ///  This won't impact the actual model, just stop it being computed for drawing until it reenters the scene
        ///  </summary>
        private void Update()
        {
            if (_lifeSpan <= 0)
            {
                Debug.Log($"Destroying {gameObject.name} due to degen (out of collider for too long)");
                Destroy(this);
            }
            else if (_canDegen && _degen)
            {
                _lifeSpan--;
            }
        }

        /// <summary>  
        ///  Set Degen on the object; triggered true when drawable leaves capture volume, false when entering
        ///  </summary>
        public void Set_Degen(bool startDegen)
        {
            if (!startDegen)
            {
                _lifeSpan = MaxLifeSpan;
            }
            _degen = startDegen;
        }
    }
}
