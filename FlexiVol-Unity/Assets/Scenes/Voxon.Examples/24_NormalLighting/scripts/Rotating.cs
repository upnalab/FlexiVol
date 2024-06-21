using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._24_NormalLighting
{
    /// <summary>
    /// Rotates gameObject in circle around z axis
    /// World positions range is x: [-1,1], y: [-1,1]
    /// </summary>
    public class Rotating : MonoBehaviour
    {
        /// <summary>
        /// Total Time passed
        /// </summary>
        float totalTime = 0;

        /// <summary>
        /// Called Per Frame
        /// Update Object position in x/y based on
        /// total time passed
        /// </summary>
        void Update()
        {
            totalTime += Time.deltaTime;

            Vector3 pos = gameObject.transform.position;

            pos.x = Mathf.Cos(totalTime);
            pos.y = Mathf.Sin(totalTime);

            gameObject.transform.position = pos;
        }
    }
}