using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = System.Random;

namespace Voxon.Examples._18_PerformanceExamples
{
    /// <summary>
    /// A struct to contain gameobjects that can be spawned and their human readable name
    /// </summary>
    [Serializable]
    public struct spawnable
    {
        public string Name;
        public GameObject go;
    }

    /// <summary>
    /// Adds controls to allow the spawning of various gameObjects into the scene
    /// </summary>
    public class Object_Spawner : MonoBehaviour
    {
        /// <summary>
        /// List of spawnable game objects
        /// </summary>
        public spawnable[] Spawnables;
        /// <summary>
        /// Dictionary to allow for easy programmatic access to game objects
        /// by name
        /// </summary>
        private Dictionary<string, GameObject> _spawn_list;
        /// <summary>
        /// Random value generator. Used to randomise positions.
        /// </summary>
        Random rng = new Random();
        
        /// <summary>
        /// Called on Start.
        /// Compiles <see cref="_spawn_list"/> from provided <see cref="Spawnables"/> list.
        /// </summary>
        void Start()
        {
            _spawn_list = new Dictionary<string, GameObject>();
            foreach (var VARIABLE in Spawnables)
            {
                _spawn_list[VARIABLE.Name] = VARIABLE.go;
            }
        }

        /// <summary>
        /// Called per frame. Adds the following Controls.
        /// "Reset" : Reset scene, removing all objects with VXComponents
        /// "Cube"  : Spawn a cube at a random location
        /// "Dude"  : Spawn an animated game object at a random location
        /// "Sphere": Spawn a sphere at a random location
        /// "Cylinder": Spawn a cylinder at a random location
        /// "Plane" : Spawn a plane (2d square) at a random location
        /// "BCube" : Spawn a bCube at a random location
        /// </summary>
        /// <remarks>
        /// Warning! Reset will remove *all* gameobjects with VXComponents. 
        /// To correct this behaviour Store instantiated gameobjects in an array and have reset destroy those instead.
        /// </remarks>
        void Update()
        {
            if (Voxon.Input.GetKeyDown("Reset"))
            {
                Reset();
            }

            if (Voxon.Input.GetKeyDown("Cube"))
            {
                // Instantiate(_spawn_list["cube"], Vector3.zero, Quaternion.identity).AddComponent<VXComponent>();

                Vector3 random_pos = new Vector3(rng.Next(-100, 100) / 50f, rng.Next(-100, 100) / 50f,
                    rng.Next(-100, 100) / 50f);
                Instantiate(_spawn_list["cube"], random_pos, Quaternion.identity).AddComponent<VXComponent>();
            }

            if (Voxon.Input.GetKeyDown("Dude"))
            {
                Vector3 random_pos = new Vector3(rng.Next(-100, 100) / 50f, rng.Next(-100, 100) / 50f,
                    rng.Next(-100, 100) / 50f);
                Instantiate(_spawn_list["dude"], random_pos, Quaternion.identity).AddComponent<VXGameObject>();
            }

            if (Voxon.Input.GetKeyDown("Sphere"))
            {
                Vector3 random_pos = new Vector3(rng.Next(-100, 100) / 50f, rng.Next(-100, 100) / 50f,
                    rng.Next(-100, 100) / 50f);
                Instantiate(_spawn_list["sphere"], random_pos, Quaternion.identity).AddComponent<VXComponent>();
            }

            if (Voxon.Input.GetKeyDown("Cylinder"))
            {
                Vector3 random_pos = new Vector3(rng.Next(-100, 100) / 50f, rng.Next(-100, 100) / 50f,
                    rng.Next(-100, 100) / 50f);
                Instantiate(_spawn_list["cylinder"], random_pos, Quaternion.identity).AddComponent<VXComponent>();
            }

            if (Voxon.Input.GetKeyDown("Plane"))
            {
                Vector3 random_pos = new Vector3(rng.Next(-100, 100) / 50f, rng.Next(-100, 100) / 50f,
                    rng.Next(-100, 100) / 50f);
                Instantiate(_spawn_list["plane"], random_pos, Quaternion.identity).AddComponent<VXComponent>();
            }

            if (Voxon.Input.GetKeyDown("BCube"))
            {
                Vector3 random_pos = new Vector3(rng.Next(-100, 100) / 50f, rng.Next(-100, 100) / 50f,
                    rng.Next(-100, 100) / 50f);
                Instantiate(_spawn_list["bcube"], random_pos, Quaternion.identity).AddComponent<VXComponent>();
            }
        }

        /// <summary>
        /// Removes all objects with VXComponents from the scene
        /// </summary>
        private void Reset()
        {
            VXComponent[] comps = FindObjectsOfType<VXComponent>();
            foreach (VXComponent comp in comps)
            {
                Destroy(comp.gameObject);
            }

            // Need to recreate base objects
        }
    }
}