using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Voxon.Examples._19_Lighting
{
	/// <summary>
	/// Generates random cat game objects (<see cref="kitty"/>) that trigger to run (animation)
	/// across the camera's view area. Each is slightly modified in position and scale.
	/// </summary>
	public class SpawnCat : MonoBehaviour
	{
		/// <summary>
		/// Total number of cats to maintain
		/// </summary>
		public int cat_count;
		/// <summary>
		/// Base animated mesh used for cats
		/// </summary>
		public GameObject kitty;
		/// <summary>
		/// Array of each spawned cat
		/// </summary>
		GameObject[] cat_array;
		/// <summary>
		/// Distance from center the cat will spawn in
		/// </summary>
		public float spawn_radius = 3;
		/// <summary>
		/// Distance from center that will trigger a despawn of cat
		/// </summary>
		public float despawn_radius = 3;

		/// <summary>
		/// Base Y position of each cat (high enough to sit on ground, but not fall)
		/// </summary>
		float y = -0.255f;

		/// <summary>
		/// Called on Start.
		/// Creates the <see cref="cat_array"/> with <see cref="cat_count"/> spaces
		/// </summary>
		void Start()
		{
			cat_array = new GameObject[cat_count];
		}

		/// <summary>
		/// Called per Frame.
		/// Iterates through <see cref="cat_array"/> and creates a cat if the value is empty; else
		/// checks if the cat is far enough out to despawn.
		/// </summary>
		void Update()
		{
			for (int idx = 0; idx < cat_array.Length; idx++)
			{
				if (cat_array[idx] == null)
				{
					MakeKitty(idx);
				}
				else if (Vector3.Distance(cat_array[idx].transform.position, transform.position) > despawn_radius)
				{
					Destroy(cat_array[idx]);
					cat_array[idx] = null;

					MakeKitty(idx);
				}
			}
		}

		/// <summary>
		/// Spawn a cat at a random location in range, facing across the center and give it 
		/// movement and animation
		/// </summary>
		/// <param name="idx">Index where cat will be stored in <see cref="cat_array"/></param>
		void MakeKitty(int idx)
		{
			float angle = UnityEngine.Random.Range(0.0f, 1f) * (float)Math.PI * 2f;
			float x = (float)Math.Cos(angle) * spawn_radius;
			float z = (float)Math.Sin(angle) * spawn_radius;

			cat_array[idx] = GameObject.Instantiate(kitty);
			cat_array[idx].transform.localScale = new Vector3(
				UnityEngine.Random.Range(0.8f, 1.2f),
				UnityEngine.Random.Range(0.8f, 1.2f),
				UnityEngine.Random.Range(0.8f, 1.2f));

			cat_array[idx].transform.position = new Vector3(x, y, z);
			Vector3 dir = transform.position;
			dir.x += UnityEngine.Random.Range(-1f, 1f);
			dir.y = y;
			dir.z += UnityEngine.Random.Range(-1f, 1f);

			cat_array[idx].transform.LookAt(dir);
			cat_array[idx].transform.right = -cat_array[idx].transform.forward;

			float speed = UnityEngine.Random.Range(-2f, -1f);
			cat_array[idx].GetComponent<forward_kitty>().speed = speed;
			cat_array[idx].GetComponent<Animator>().SetFloat("Kitty_Speed", -speed);
		}
	}
}