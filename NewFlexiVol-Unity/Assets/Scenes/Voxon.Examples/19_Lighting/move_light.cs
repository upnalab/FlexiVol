using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._19_Lighting
{
    /// <summary>
    /// Rotate game Object around a fixed position in x,y,z
    /// </summary>
    public class move_light : MonoBehaviour
    {
        /// <summary>
        /// Call per Frame.
        /// Update position by x (sine time), y avg(sin time + cos time), z (cos time)
        /// </summary>
        void LateUpdate()
        {
            Vector3 pos = gameObject.transform.position;

            pos.x += Mathf.Sin(Time.fixedTime / 5) * 10;
            pos.y += 10 + (Mathf.Sin(Time.fixedTime / 5) + Mathf.Cos(Time.fixedTime)) / 2 * 10;
            pos.z += Mathf.Cos(Time.fixedTime / 5) * 10;

            gameObject.transform.position = pos;
        }
    }
}