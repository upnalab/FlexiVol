using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples._4_MouseInput
{
    /// <summary>
    /// Collect Mouse input data from the Voxon Runtime
    /// </summary>
    public class MouseInput : MonoBehaviour {
        /// <summary>
        /// Minimum X, Z values for Mouse
        /// </summary>
        private Vector3 _minLimits = new Vector3(-4.5f, -1, -4.5f);
        /// <summary>
        /// Maximum X, Z values for Mouse
        /// </summary>
        private Vector3 _maxLimits = new Vector3(4.5f, -1, 4.5f);

        /// <summary>
        /// Set of Meshes to iterate through
        /// </summary>
        [FormerlySerializedAs("Meshes")] public Mesh[] meshes = new Mesh[4];

        /// <summary>
        /// Called every X milliseconds (not linked to framerate)
        /// Updates gameobjects position based on mouse position
        /// Input Test:
        /// "Left": Randomise Mesh Color
        /// "Right": Randomise Mesh Scale (X,Y,Z)
        /// </summary>
        private void FixedUpdate () {
            MousePosition mp = Voxon.Input.GetMousePos();

            Vector3 position = gameObject.transform.position;
            position = new Vector3(Mathf.Clamp(position.x + mp.X, _minLimits.x, _maxLimits.x),
                Mathf.Clamp(position.y + mp.Z, _minLimits.y, _maxLimits.y),
                Mathf.Clamp(position.z - mp.Y, _minLimits.z, _maxLimits.z));
            gameObject.transform.position = position;

            if(Voxon.Input.GetMouseButtonDown("Left"))
            {
                gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
            }

            if (Voxon.Input.GetMouseButtonDown("Right"))
            {
                transform.localScale = new Vector3(Random.Range(0.25f, 3f), Random.Range(0.25f, 3f), Random.Range(0.25f, 3f));
            }

        }
    }
}
