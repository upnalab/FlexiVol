using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._19_Lighting
{
    /// <summary>
    /// Cat (object) mover.
    /// Has an object for to it's right per frame
    /// ~~~ Reasoning: Cat mesh is turned 90 degrees
    /// </summary>
    public class forward_kitty : MonoBehaviour
    {
        /// <summary>
        /// Distance cat will move per second (meters)
        /// </summary>
        public float speed = 1;

        /// <summary>
        /// Called per Frame
        /// Updates position of cat based on delta time and speed
        /// </summary>
        void Update()
        {
            transform.position = transform.position + (transform.right * Time.deltaTime * speed);
        }
    }
}