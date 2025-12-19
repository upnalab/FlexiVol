using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Voxon
{
	/// <summary>
	///  The model is independent of the user interface.
	/// It doesn't know if it's being used from a text-based, graphical, or web interface
	/// </summary>
	[Serializable]
	public class ShadowModel
	{
		private GameObject _parent;
		private RenderTexture _depth;
		private RenderTexture _colour;
		private tiletype _texture;

		// Consider using DrawMeshTex instead
		private poltex[] _vertices;

		private point3d _position;
		private point3d _forward;
		private point3d _right;
		private point3d _down;

		// Buffers
		private Texture2D colourBuffer;
		private Texture2D depthBuffer;
		private Rect rectBuffer;


		#region data_manipulation
		WaitForSeconds waitTime = new WaitForSeconds(0.1F);
		WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

		public IEnumerator UpdateLight()
		{
			if (_colour == null) yield return waitTime;

			if (_texture.first_pixel == IntPtr.Zero || rectBuffer == null || depthBuffer == null)
			{
				Debug.Log("Generating Buffers");

				// Create TileType
				_vertices = new poltex[_colour.width * _colour.height];


				// TextureFormat.BGRA32
				colourBuffer = new Texture2D(_colour.width, _colour.height, TextureFormat.ARGB32, false);
				depthBuffer = new Texture2D(_colour.width, _colour.height, TextureFormat.ARGB32, false);
				rectBuffer = new Rect(0, 0, _colour.width, _colour.height);

				// Copy Texture Data
				_texture = new tiletype
				{
					height = (IntPtr)colourBuffer.height,
					width = (IntPtr)colourBuffer.width,
					pitch = (IntPtr)(colourBuffer.width * Marshal.SizeOf(typeof(Color32))),
					first_pixel =
						Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * colourBuffer.GetRawTextureData().Length)
				};
			}

			while (true) { 
				yield return frameEnd;
				/** Update Colour **/

				// Read from Colour Camera
				RenderTexture.active = _colour;
				colourBuffer.ReadPixels(rectBuffer, 0, 0);
				colourBuffer.Apply();
				RenderTexture.active = null;

				// Copy to Texture
				Marshal.Copy(
					colourBuffer.GetRawTextureData(), 
					0, 
					_texture.first_pixel,
					colourBuffer.GetRawTextureData().Length);

				var vert_cols = colourBuffer.GetPixels32();

				/** Update Depth **/
				// Read from Depth Camera
				RenderTexture.active = _depth;
				depthBuffer.ReadPixels(rectBuffer, 0, 0);
				depthBuffer.Apply();
				RenderTexture.active = null;

				var depth_vals = depthBuffer.GetPixels32();

				float width = 1;
				float height = 1;
				float halfwidth = width * 0.5f;
				float halfheight = height * 0.5f;

				float widthstep = width / _colour.width;
				float heightstep = height / _colour.height;

				for (int idx = 0; idx < _colour.width; idx++)
				{
					var x_offset = idx * _colour.height;
					for (int idy = 0; idy < _colour.height; idy++)
					{
						int index = x_offset + idy;

						_vertices[index].x = idx * widthstep - halfwidth;
						_vertices[index].y = idy * heightstep - halfheight;

						_vertices[index].z = 0;// depth_vals[index].r/255.0f;

						_vertices[index].col = vert_cols[index].ToInt();

						if (depth_vals[index].b != 205 || depth_vals[index].a != 205)
						{
							Debug.Log(depth_vals[index]);
						}
					}
				}

			}
		}
		#endregion

		#region getters_setters
		public tiletype Texture
		{
			get => _texture;
		}

		public point3d Position { get => _position; set { _position = value; } }
		public point3d Forward { get => _forward; set { _forward = value; } }
		public point3d Right { get => _right; set { _right = value; } }
		public point3d Down { get => _down; set { _down = value; } }

		public RenderTexture Colour
		{
            get => _colour;
            set { _colour = value;}
        }

		public RenderTexture Depth
		{
			get => _depth;
			set { _depth = value; }
		}

		public GameObject Parent
        {
            get => _parent;
            set { _parent = value; }
        }

		public poltex[] Vertices
		{
			get => _vertices;
		}

        #endregion
    }
}
