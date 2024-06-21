using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples._2_ComplexMesh
{
    /// <summary>
    /// Spawns a game object at a consistent interval up to maximum number
    /// Destroys oldest game objects at a fixed interval or when maximum has been reached
    /// </summary>
    public class CharacterSpawner : MonoBehaviour {

        /// <summary>
        /// Game object to be spawned at interval
        /// </summary>
        public GameObject spawnable;
        /// <summary>
        /// Maximum number of (spawned) game objects to exist
        /// at once
        /// </summary>
        [FormerlySerializedAs("max_chars")] public int maxChars;
        /// <summary>
        /// List of all spawned game objects
        /// </summary>
        private List<GameObject> _chars;
    
        /// <summary>
        /// Called on Start.
        /// Initialise Chars
        /// </summary>
        private void Start () {
            _chars = new List<GameObject>();
        }
	
        /// <summary>
        /// Called per Frame.
        /// Spawns new game object is at fixed time and below max (or when 0 game objects exist)
        /// Destroys oldest gameobject is at max or at fixed interval
        /// </summary>
        private void Update () {
            if(_chars.Count == 0 || (Time.frameCount % 173 == 0 && _chars.Count < maxChars))
            {
                try
                {
                    _chars.Add(Instantiate(spawnable, new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)), Quaternion.identity));
                    _chars[_chars.Count - 1].AddComponent<VXGameObject>();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while Spawning Models {gameObject.name}: {e.Message}");
                }
            }

            if (Time.frameCount % 180 != 0 || _chars.Count < maxChars) return;
            GameObject fatalChar = _chars[0];
            _chars.RemoveAt(0);
            Destroy(fatalChar);

        }
    }
}
