using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._19_Lighting
{

    /// <summary>
    /// Move gameObject back and forth along x-axis in a sine wave based on time
    /// </summary>
    public class Oscillate : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            Vector3 pos = transform.position;
            pos.x = (Mathf.Sin(Time.time) * 0.5f);
            transform.position = pos;

        }
    }
}