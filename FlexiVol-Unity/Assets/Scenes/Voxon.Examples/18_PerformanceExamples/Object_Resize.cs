using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._18_PerformanceExamples
{
    /// <summary>
    /// Adds controls for adjusting attached gameObject local scale
    /// </summary>
    public class Object_Resize : MonoBehaviour
    {
        /// <summary>
        /// Called per frame.
        /// Key "Increase" increases size by 10%
        /// Key "Decrease" reduces size by 10%
        /// </summary>
        void Update()
        {
            if (Voxon.Input.GetKey("Increase"))
            {
                gameObject.transform.localScale *= 1.1f;
            }
            else if (Voxon.Input.GetKey("Decrease"))
            {
                gameObject.transform.localScale *= 0.9f;
            }
        }
    }
}