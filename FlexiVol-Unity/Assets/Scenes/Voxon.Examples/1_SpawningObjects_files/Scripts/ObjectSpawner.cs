using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples._1_SpawningObject
{
    /// <summary>
    /// Class that continually spawns objects per frame.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour {

        /// <summary>
        /// Collection of materials to be used by spawned game objects (providing visual variety to shapes).
        /// </summary>
        [FormerlySerializedAs("Mats")] public Material[] mats;

        /// <summary>
        /// Updates Once per frame. Generates a primitive object, with an attached rigid body, every 25 frames which is then randomly positioned
        /// and dropped from the object spawner. This object also has a random material from <see cref="mats"/> applied to it.
        /// </summary>
        private void Update () {
            if (Time.frameCount % 25 != 0) return;
        
            int type = Random.Range(0, 5);

            while (type == 4)
            {
                type = Random.Range(0, 5);
            }
            
            GameObject h = GameObject.CreatePrimitive((PrimitiveType)type);
            
            if (h.GetComponent<Renderer>() == null)
            {
                h.AddComponent<Renderer>();
            }

            h.GetComponent<Renderer>().sharedMaterial = mats[Random.Range(0, mats.Length-1)];

            h.transform.SetPositionAndRotation(new Vector3(Random.Range(-3.5f,3.5f), 7.0f, Random.Range(-3.5f, 3.5f)), new Quaternion (Random.Range(-10f, 10f), Random.Range(-10f, 3.5f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
            var bod = h.AddComponent<Rigidbody>();
            bod.AddForce(new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, -100.0f), Random.Range(-10.0f, 10.0f)));
            h.AddComponent<VXGameObject>();
        }
    }
}
