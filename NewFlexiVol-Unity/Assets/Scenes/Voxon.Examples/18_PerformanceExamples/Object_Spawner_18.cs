using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = System.Random;

[Serializable]
public struct spawnable
{
    public string Name;
    public GameObject go;
}
public class Object_Spawner_18 : MonoBehaviour
{
    public spawnable[] Spawnables;
    private Dictionary<string, GameObject> _spawn_list;
    Random rng = new Random();
    // Start is called before the first frame update
    void Start()
    {
        _spawn_list = new Dictionary<string, GameObject>();
        foreach (var VARIABLE in Spawnables)
        {
            _spawn_list[VARIABLE.Name] = VARIABLE.go;
        }
    }

    // Update is called once per frame
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
