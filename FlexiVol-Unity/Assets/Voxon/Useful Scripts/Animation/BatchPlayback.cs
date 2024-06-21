using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxon;

public class BatchPlayback : MonoBehaviour, Voxon.IDrawable
{
	public float playbackSpeed = 1.0f;
	float time = 0.0f;
	int curFrame = 0;
	int lastFrame = 0;

	// [StringInList(typeof(PropertyDrawersHelper), "AllSceneNames")]
	//public string[] Frames;
	public string FrameNaming;
	public string TextureNaming;

	string[] Frames;
	string[] Textures;

	RegisteredMesh rm;
	tiletype rt;

	Transform parentTransform;
	Matrix4x4 matTransform;

	bool initial_loop = true;
	void Start()
	{
		// Static Transform (Recomputer per frame for moving gameobjects / camera);
		parentTransform = gameObject.transform;
		matTransform = transform.localToWorldMatrix;
		// matTransform = VXProcess.Instance.Transform * matTransform;

		string[] keys = MeshRegister.Instance.Keys();
		System.Array.Sort(keys, (x, y) => System.String.Compare(x, y));
		Frames = System.Array.FindAll(keys, c => c.Contains(FrameNaming));
		// Debug.Log($"{Frames.Length} frames loaded");
		VXProcess.Drawables.Add(this);

		keys = TextureRegister.Instance.Keys();
		System.Array.Sort(keys, (x, y) => System.String.Compare(x, y));
		Textures = System.Array.FindAll(keys, c => c.Contains(TextureNaming));
		// Debug.Log($"{Frames.Length} frames loaded");
		VXProcess.Drawables.Add(this);
	}

	// Update is called once per frame
	void Update()
    {
		time += Time.deltaTime * playbackSpeed;
		curFrame = (int)time;

		if (curFrame > Frames.Length - 1)
		{
			time = 0;
			curFrame = 0;
			initial_loop = false;
		}

		if (lastFrame != curFrame)
		{
			lastFrame = curFrame;

			// Debug.Log($"{Frames[curFrame]}:{Textures[curFrame]}");

			rm = MeshRegister.Instance.get_registed_mesh(Frames[curFrame]);
			if (initial_loop)
			{
				rm.compute_transform_cpu(matTransform, ref rm.vertices);
			}
			
			rt = TextureRegister.Instance.get_tile(Textures[curFrame]);

		}
	}

	void IDrawable.Draw()
	{
		if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;

		if (rm != null)
		{
			for (int i = 0; i < rm.submeshCount; i++)
			{
				VXProcess.Runtime.DrawTexturedMesh(ref rt, rm.vertices, rm.vertexCount, rm.indices[i], rm.indexCounts[i], 2 | 1 << 3);
			}
		}
	}
}
