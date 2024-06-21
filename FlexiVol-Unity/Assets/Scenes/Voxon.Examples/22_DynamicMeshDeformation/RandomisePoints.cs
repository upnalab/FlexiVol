using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._22_DynamicMeshDeformation
{
	/// <summary>
	/// Randomises points within an attached Mesh
	/// </summary>
	[RequireComponent(typeof(MeshFilter))]
	public class RandomisePoints : MonoBehaviour
	{
		/// <summary>
		/// Mesh Filter of attached mesh
		/// </summary>
		MeshFilter mf;
		/// <summary>
		/// Reference to attached Mesh
		/// </summary>
		Mesh mesh;
		
		/// <summary>
		/// Called on Start
		/// Get Mesh Filter and Mesh
		/// </summary>
		void Start()
		{
			mf = GetComponent<MeshFilter>();
			mesh = mf.mesh;
		}

		/// <summary>
		/// Called per Frame.
		/// Get Vertices from Mesh
		/// Randomise a single vertice and set back to mesh
		/// </summary>
		void Update()
		{
			Vector3[] verts = mesh.vertices;
			int idx = Random.Range(0, verts.Length);
			verts[idx] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
			mesh.SetVertices(verts);
			mf.mesh = mesh;
		}
	}
}