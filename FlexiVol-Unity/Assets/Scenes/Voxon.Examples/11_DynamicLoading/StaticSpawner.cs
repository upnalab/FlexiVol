using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Voxon.Examples._11_DynamicLoading
{
    /// <summary>
    /// Spawns <see cref="objCount"/> objects at the start of the program with sizes
    /// rangeing from <see cref="min_size"/> to <see cref="max_size"/>.
    /// </summary>
    public class StaticSpawner : MonoBehaviour
    {
        /// <summary>
        /// Number of objects to be spawned
        /// </summary>
        public int objCount = 100;

        /// <summary>
        /// Minimum size of each object
        /// </summary>
        public float min_size = 0.1f;
        /// <summary>
        /// Maximum size of each object
        /// </summary>
        public float max_size = 3.0f;

        /// <summary>
        /// Minimum Position
        /// </summary>
        public Vector3 minVector = new Vector3();

        /// <summary>
        /// Maximum Position
        /// </summary>
        public Vector3 maxVector = new Vector3();

        /// <summary>
        /// Object to be spawned
        /// </summary>
        public GameObject spawnable;

        /// <summary>
        /// Call on start. Generate objects of random size and place in random locations
        /// </summary>
        void Start()
        {
            GameObject original = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject go;
            for (int x = 0; x < objCount; ++x)
            {
                go = Instantiate(spawnable,
                    new Vector3(
                        Random.Range(minVector.x, maxVector.x),
                        0.5f,
                        Random.Range(minVector.x, maxVector.x)),
                    Quaternion.identity
                );
                float value = Random.Range(min_size, max_size);
                go.transform.localScale = new Vector3(value, value, value);
            }
        }
    }
}