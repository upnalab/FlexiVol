using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._19_Lighting
{
	/// <summary>
	/// Voxon Lighting System
	/// </summary>
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class Lighting : MonoBehaviour
	{
		/// <summary>
		/// Camera capturing scene (our 'light')
		/// </summary>

		private UnityEngine.Camera cam;
		/// <summary>
		///  Texture 'light' data (color and height) is rendered to
		/// </summary>
		private RenderTexture rt;
		/// <summary>
		/// Total pixels in light texture
		/// </summary>
		private int PixelCount = 0;
		/// <summary>
		/// Width of light texture
		/// </summary>
		private int width = 0;
		/// <summary>
		/// Height of light texture
		/// </summary>
		private int height = 0;

		/// <summary>
		/// Voxels which will draw light
		/// </summary>
		LightVoxels lv;
		/// <summary>
		/// Defines parameters of view windows
		/// </summary>
		Rect lightWindow;

		/// <summary>
		/// Texture Buffer that light data is stored in before being
		/// transfered to <see cref="lv"/>
		/// </summary>
		Texture2D lightTexture;
		/// <summary>
		/// Colour of each pixel
		/// </summary>
		Color32 [] colours;
		
		/// <summary>
		/// Call on Start.
		/// Collects camera and it's attached render target.
		/// Updates our width, height and pixel counts based on target
		/// Defines light window, texture buffer and sets up lightvoxels
		/// </summary>
		void Start()
		{
			cam = GetComponent<UnityEngine.Camera>();
			rt = cam.targetTexture;
			width = rt.width;
			height = rt.height;
			PixelCount = width * height;

			lightWindow = new Rect(0, 0, width, height);
			lightTexture = new Texture2D(width, height);
			lv = new LightVoxels(width, height);
		}

		/// <summary>
		/// Call per Frame
		/// Forces <see cref="cam"/> to render (due to no graphics pipeline)
		/// Reads texture data from <see cref="rt"/> and applies values to <see cref="lv"/>
		/// </summary>
		void Update()
		{
			cam.Render();
			RenderTexture.active = rt;
			lightTexture.ReadPixels(lightWindow, 0, 0);
			lightTexture.Apply();
			RenderTexture.active = null;

			colours = lightTexture.GetPixels32();
			lv.Update(ref colours);
		}
	}
}