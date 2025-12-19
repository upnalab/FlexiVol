using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._25_TintingTextures
{
    /// <summary>
    /// Iterates between multiple colors and applies 
    /// then to the attached meshes material
    /// </summary>
    [RequireComponent(typeof(Material))]
    public class AdjustingColours : MonoBehaviour
    {
        /// <summary>
        /// Material of Attached Mesh
        /// </summary>
        Material mat;

        /// <summary>
        /// Array of possible colors for material
        /// </summary>
        public Color[] colors;

        /// <summary>
        /// Index of active color
        /// </summary>
        public int currentIndex = 0;
        /// <summary>
        /// Next index to move to (looping)
        /// </summary>
        private int nextIndex;

        /// <summary>
        /// How many seconds a color will stay 
        /// active for
        /// </summary>
        public float changeColourTime = 2.0f;

        /// <summary>
        /// Current running timer for color change
        /// </summary>
        private float timer = 0.0f;

        /// <summary>
        /// Called on Start
        /// Gets attached material
        /// Tests Colors array for a minimum of 2 colors
        /// </summary>
        void Start()
        {
            mat = GetComponent<Renderer>().material;
            if (colors == null || colors.Length < 2)
                Debug.Log("Need to setup colors array in inspector");

            nextIndex = (currentIndex + 1) % colors.Length;
        }

        /// <summary>
        /// Called Per Frame
        /// Every <see cref="changeColourTime"/> seconds, will move to the 
        /// next color in <see cref="colors"/>. Updates the material with
        /// this new color.
        /// </summary>
        void Update()
        {

            timer += Time.deltaTime;

            if (timer > changeColourTime)
            {
                currentIndex = (currentIndex + 1) % colors.Length;
                nextIndex = (currentIndex + 1) % colors.Length;
                timer = 0.0f;

            }
            mat.color = Color.Lerp(colors[currentIndex], colors[nextIndex], timer / changeColourTime);
        }
    }
}