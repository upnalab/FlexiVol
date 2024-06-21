// https://github.com/przemyslawzaworski
// Set plane position (Y=0.0)
// Collider is generated from vertex shader, using RWStructuredBuffer to transfer data.

using UnityEngine;


namespace Voxon.Examples._19_Lighting
{
	/// <summary>
	/// Structure tracking Depth values and associated
	/// index value applies to
	/// </summary>
	public struct depth
	{
		public float value;         // Depth value at this point
		public int data_index;      // Data index where depth related value is stored
	};

	/// <summary>
	/// The light buffer class operates on 2 primary buffers.
	/// A data buffer that can contain up to a full volumes worth of voxels (it never should, but that's our max). 
	///	This data is organised in an array of running indices and is drawn
	/// A depth buffer that stores the depth value for each pixel in the render texture.
	///	Each new fragment tests itself against the depth buffer using it's x, y and if depth is less than active depth, it will use the existing x,y,z (if they exist), 
	///	and update the data buffer with the fragments issue, before updating the depth buffer to the new values. Deeper values will be discarded.
	/// </summary>
	public class lightbuffer : MonoBehaviour, IDrawable
	{
		/// <summary>
		/// Stores pixel color data
		/// </summary>
		ComputeBuffer light_buffer;
		/// <summary>
		/// Stores pixel depth data (convert plane to volume)
		/// </summary>
		ComputeBuffer depth_buffer;
		/// <summary>
		/// Stores pixel index
		/// </summary>
		ComputeBuffer index_buffer;

		/// <summary>
		/// Shader to clear pixel data
		/// </summary>
		ComputeShader clearShader;

		/// <summary>
		/// Light Shader; not currently used.
		/// Initially Intended to add to objects in scene, but 
		/// requires deeper consideration as that will involve 
		/// a full material construction
		/// </summary>
		Shader lightShader;

		/// <summary>
		/// Active Voxon Camera
		/// </summary>
		VXCamera activeCamera;
		/// <summary>
		/// Unity Camera used to capture light in scene
		/// </summary>
		UnityEngine.Camera cam;
		/// <summary>
		/// Render Target of Unity Camera
		/// </summary>
		RenderTexture rb;
		/// <summary>
		/// Total number of voxels to draw
		/// </summary>
		int voxels = 0;
		/// <summary>
		/// Total number of pixels in texture
		/// </summary>
		int pixels = 0;
		/// <summary>
		/// X / Y resolution of capture
		/// </summary>
		public int resolution = 512;

		/// <summary>
		/// Read Voxel position & color data from
		/// compute buffer stored here
		/// </summary>
		poltex[] poltex_data;
		/// <summary>
		/// Depth data.
		/// Currently not used
		/// </summary>
		depth[] depth_data;
		/// <summary>
		/// Default index values. Use to restore indexes
		/// </summary>
		int[] default_index = new int[] { 0 };
		/// <summary>
		/// Current index values. Updated based on which
		/// indices will be drawn
		/// </summary>
		int[] current_index = new int[] { 0 };

		/// <summary>
		/// Light distance from target (camera)
		/// </summary>
		public float LightDistance = 5;
		/// <summary>
		/// Rounding error value for floats
		/// </summary>
		public float sigma = 0.001f;

		/// <summary>
		/// Buffer used by draw calls
		/// </summary>
		poltex[] poltex_buffer;
		/// <summary>
		/// Total number of compute groups to ensure
		/// 1 megabyte is processed per group
		/// </summary>
		int group_size = (1024 * 1024) / 24; // How many poltex per Megabyte
		/// <summary>
		/// Total number of groups (division of pixels by group size)
		/// </summary>
		int group_count = 0;

		/// <summary>
		/// ID for each shader parameter
		/// </summary>
		static readonly int
			resolutionId = Shader.PropertyToID("_Resolution"),
			indexId = Shader.PropertyToID("_Index"),
			cameraId = Shader.PropertyToID("_ActiveCamera"),
			dataId = Shader.PropertyToID("_Data"),
			depthId = Shader.PropertyToID("_Depth");


		/// <summary>
		/// Called on Start.
		/// Gets attached unity Camera
		/// Ensures depth mode is accurate
		/// determines light layer resolution
		/// Configures Shaders, Buffers, groups, and Drawable action
		/// </summary>
		void Start()
		{
			cam = GetComponent<UnityEngine.Camera>();
			cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
			rb = cam.targetTexture;

			//resolution = rb.width;

			if(resolution == 0)
			{
				Debug.LogError("Render Texture resolution is 0; stopping light system");
				return;
			}

			clearShader = (ComputeShader)Resources.Load("LightClear");


			// Voxel Setup
			// voxels = xz_resolution * xz_resolution * y_resolution; // Default to 200 Y resolution
			voxels = resolution * resolution;

			// Determine Groups based on group size and set up buffer
			poltex_buffer = new poltex[group_size];
			group_count = Mathf.CeilToInt(voxels / (float)group_size);

			// Debug.Log($"Groups: {group_count} - Last Group Remainder {voxels % group_size}");

			poltex_data = new poltex[voxels];

			int poltex_stride = 4 /*32 bit*/ * (3 /* Vector3 */ + 2 /* Vector2 */ + 1 /* int */);
			light_buffer = new ComputeBuffer(voxels, poltex_stride, ComputeBufferType.Default);

			// Depth Setup
			// pixels = xz_resolution * xz_resolution;
			pixels = voxels;

			depth_data = new depth[pixels];
			int pixel_stride = 4 /* int */ + 4 /* float */;
			depth_buffer = new ComputeBuffer(pixels, pixel_stride, ComputeBufferType.Default);

			// Index Setup
			int index_count = 1;
			int index_stride = 4;
			index_buffer = new ComputeBuffer(index_count, index_stride, ComputeBufferType.Default);

			lightShader = Shader.Find("Voxon/LightShader");

			Graphics.ClearRandomWriteTargets();
			Shader.SetGlobalBuffer(dataId, light_buffer);
			Shader.SetGlobalBuffer(depthId, depth_buffer);
			Shader.SetGlobalBuffer(indexId, index_buffer);

			index_buffer.SetData(default_index);
			Shader.SetGlobalInt(resolutionId, resolution);
			
			Graphics.SetRandomWriteTarget(1, light_buffer, false);
			Graphics.SetRandomWriteTarget(2, depth_buffer, false);
			Graphics.SetRandomWriteTarget(3, index_buffer, false);

			// Should IDrawable call this? Probably not since it's an interface but worth considering
			VXProcess.Drawables.Add(this); 

			int groups = Mathf.CeilToInt(resolution / 64);
			clearShader.Dispatch(0, groups, groups, 1);
		}

		/// <summary>
		/// Called per Frame
		/// Ensures activeCamera is valid
		/// updates light camera position, direction, and updates shaders to current values
		/// </summary>
		void Update()
		{
			activeCamera = VXProcess.Instance.Camera;
			
			cam.transform.LookAt(activeCamera.transform);

			Vector3 lightToCamVector = (activeCamera.transform.position - transform.position);
			float lightToCamDist = Vector3.Magnitude(lightToCamVector);
			float offset = lightToCamDist - LightDistance;

			if (Mathf.Abs(offset) > sigma)
			{
				transform.position = transform.position + Vector3.Normalize(lightToCamVector) * offset;
			}

			Shader.SetGlobalMatrix(cameraId, activeCamera.transform.worldToLocalMatrix);
		}

		/*
		void OnDisable()
		{
			if (light_buffer != null) { 
				light_buffer.Release();
				light_buffer = null;
			}

			if (depth_buffer != null)
			{
				depth_buffer.Release();
				depth_buffer = null;
			}

			if (index_buffer != null)
			{
				index_buffer.Release();
				index_buffer = null;
			}
		}
		*/

		/// <summary>
		/// Called on Application Quit.
		/// Releases all compute buffers
		/// </summary>

		void OnApplicationQuit()
		{
			if (light_buffer != null)
			{
				light_buffer.Release();
				light_buffer = null;
			}

			if (depth_buffer != null)
			{
				depth_buffer.Release();
				depth_buffer = null;
			}

			if (index_buffer != null)
			{
				index_buffer.Release(); // or Dispose?
				index_buffer = null;
			}
		}

		/// <summary>
		/// Called at end of Frame
		/// Dispatches clear action for pixels
		/// Forces light camera to render (no graphics pipeline)
		/// </summary>
		void LateUpdate()
		{
			// Last Actions before Graphics calls
			int groups = Mathf.CeilToInt(resolution / 8);
			clearShader.Dispatch(0, groups, groups, 1);

			cam.Render(); 
		}

		/// <summary>
		/// Called by VXProcess per frame
		/// Collects voxel data
		/// tracks depth of each captured voxel 
		/// (only grab the voxel closest to the light per line)
		/// Draws all voxels
		/// </summary>
		public void Draw()
		{
			if (!gameObject.activeInHierarchy || !enabled)
			{
				// Debug.Log($"{gameObject.name}: Skipping");
				return;
			}

			light_buffer.GetData(poltex_data); // Can get native buffers

			index_buffer.GetData(current_index);

			if(current_index[0] > poltex_data.Length)
			{
				// Debug.Log($"Failed to Clear Index Buffer Correct. Recieved Count {current_index[0]}");
				current_index[0] = poltex_data.Length;
			}

			int voxel_count;
			int groups = Mathf.CeilToInt((float)current_index[0] / (float)group_size);

			// Debug.Log($"Indices({current_index[0]}) / Group_Size({group_size}) = Count({groups})");
			for(int idx = 0; idx < groups; idx++)
			{
				// Group size unless last group
				voxel_count = idx < (groups - 1) ? group_size : (current_index[0] % group_size);
				//Debug.Log(voxel_count);

				if((voxel_count + idx * group_size) > poltex_data.Length)
				{
					Debug.Log($"VoxelCount {voxel_count } : Total {(voxel_count + idx * group_size)} : Length {poltex_data.Length}");
					return;
				}

				System.Array.Copy(poltex_data, idx * group_size, poltex_buffer, 0, voxel_count);

				VXProcess.Runtime.DrawSphereBulk(poltex_buffer, 0.002f);
				// VXProcess.Runtime.DrawUntexturedMesh(poltex_buffer, voxel_count, null, 0, 0, 0xffffff);
			}
		}
	}
}