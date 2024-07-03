namespace OpenCvSharp.Demo
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using OpenCvSharp;
	using UnityEngine.UI;

	public class CreateTexture2D : MonoBehaviour
	{
		public int sizeImage = 64;
		public RenderTexture tex;
		public Texture2D myTexture, reMergedTexture;
		public RawImage rawImage;
		private Texture2D[] bitPlaneTextures;
		private Mat[] testMatrix;
		// public Texture2D[] texturedPlanes;
		
		[Serializable]
		public struct bitPlaneInfo
		{
			public int bitplaneID;
			public Color32[] pixels;
		}

		private bitPlaneInfo[] bitplanes;
		public GenerateFuckinSlice cuttingPlane;


	    // Start is called before the first frame update
	    void Start()
	    {
	    	cuttingPlane = GameObject.FindObjectOfType<GenerateFuckinSlice>();
	        testMatrix = new Mat[3];
	        // texturedPlanes = new Texture2D[3]; // 0 is B, 1 is G, 2 is R

	        bitplanes = new bitPlaneInfo[24];
	    	bitPlaneTextures = new Texture2D[24];

	    	for(int j = 0; j < 8; j++)
			{
				bitplanes[j].bitplaneID = j;
				bitplanes[j].pixels = new Color32[sizeImage*sizeImage];

				bitplanes[j+8].bitplaneID = j+8;
				bitplanes[j+8].pixels = new Color32[sizeImage*sizeImage];

				bitplanes[j+16].bitplaneID = j+16;
				bitplanes[j+16].pixels = new Color32[sizeImage*sizeImage];

			}

			myTexture = toTexture2D(tex);
			for(int i = 0; i < 24; i++)
			{
				bitPlaneTextures[i] = myTexture;// Unity.MatToTexture(testMatrix[0]);

			}

	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        if(Input.GetKeyDown(KeyCode.Space))
	        {
	        	StartCoroutine(CalculateBitPlane());
	        }
	        StartCoroutine(MeasureFrameRate());

	    }

	    Texture2D toTexture2D(RenderTexture rTex)
		{
		    Texture2D tex = new Texture2D(64, 64, TextureFormat.RGB24, false);
		    // ReadPixels looks at the active RenderTexture.
		    RenderTexture.active = rTex;
		    tex.ReadPixels(new UnityEngine.Rect(0, 0, rTex.width, rTex.height), 0, 0);
		    tex.Apply();

		    return tex;
		}

		IEnumerator MeasureFrameRate()
		{
			float timeBegin = Time.time;
			yield return new WaitForEndOfFrame();
			// Debug.Log("Frame: " + (Time.time - timeBegin));
			Debug.Log("Frame: " + Time.deltaTime);

		}

		IEnumerator CalculateBitPlane()
		{
			float timeEntering = Time.time;
			myTexture = toTexture2D(tex);
			Color32[] pixels = myTexture.GetPixels32(0);

			for(int j = 0; j < 8; j++)
			{
				for(int i = 0; i < pixels.Length; i++)
				{
					bitplanes[j].pixels[i] = new Color32((byte)((Mathf.Floor(pixels[i].r / Mathf.Pow(2,(j+1))) % 2)*255), 0, 0, 255);
					bitplanes[j+8].pixels[i] = new Color32(0, (byte)((Mathf.Floor(pixels[i].g / Mathf.Pow(2,(j+1))) % 2)*255), 0, 255);
					bitplanes[j+16].pixels[i] = new Color32(0, 0, (byte)((Mathf.Floor(pixels[i].b / Mathf.Pow(2,(j+1))) % 2)*255), 255);
					
				}
			}
			for(int j = 0; j < 24; j++)
			{
				bitPlaneTextures[j].SetPixels32(bitplanes[j].pixels, 0);
				reMergedTexture = bitPlaneTextures[j];
				reMergedTexture.Apply();
				rawImage.texture = reMergedTexture;
				yield return new WaitForEndOfFrame(); // HERE NEED TO WAIT ILL TIME
				// yield return new WaitForSeconds(1/(cuttingPlane.frequency*cuttingPlane.nbSlices));
				// Debug.Log(cuttingPlane.cutSectionID);
				// yield return new WaitUntil(() => cuttingPlane.cutSectionID == j);
				// Maybe make some kind of bool from the slicing plane
			}

			// yield return new WaitForEndOfFrame();
			float timeDebug = Time.time - timeEntering;
			Debug.Log("BitPlanes: " + timeDebug);
        	StartCoroutine(CalculateBitPlane());


		}

		IEnumerator CalculateBitPlaneBis() // PUT THIS TO MAKE BITPLANE PUBLIC UPDATE LIVE
		{
			myTexture = toTexture2D(tex);
      		Mat mat = Unity.TextureToMat(myTexture);
			testMatrix = Cv2.Split(mat);
			for(int i = 0; i < 24; i++)
			{
				bitPlaneTextures[i] = Unity.MatToTexture(testMatrix[0]);
			// NEED THIS TO UPDATE BITPLANETEXTUREPERFRAME WHEN PUBLIC

			}

			Color32[] pixels = myTexture.GetPixels32(0);

			for(int j = 0; j < 8; j++)
			{
				for(int i = 0; i < pixels.Length; i++)
				{
					bitplanes[j].pixels[i] = new Color32((byte)((Mathf.Floor(pixels[i].r / Mathf.Pow(2,(j+1))) % 2)*255), 0, 0, 255);
					bitplanes[j+8].pixels[i] = new Color32(0, (byte)((Mathf.Floor(pixels[i].g / Mathf.Pow(2,(j+1))) % 2)*255), 0, 255);
					bitplanes[j+16].pixels[i] = new Color32(0, 0, (byte)((Mathf.Floor(pixels[i].b / Mathf.Pow(2,(j+1))) % 2)*255), 255);
				}
			}
			for(int j = 0; j < 24; j++)
			{
				bitPlaneTextures[j].SetPixels32(bitplanes[j].pixels, 0);
			}

			for(int i = 0; i < 24; i++)
			{
				reMergedTexture = bitPlaneTextures[i];
				reMergedTexture.Apply();
				rawImage.texture = bitPlaneTextures[i];
				yield return new WaitForEndOfFrame();

			}

			yield return new WaitForEndOfFrame();
        	StartCoroutine(CalculateBitPlaneBis());

		}

	}

}
