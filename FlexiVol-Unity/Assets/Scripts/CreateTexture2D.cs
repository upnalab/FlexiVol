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
		public int showMe;
		public RenderTexture tex;
		public Texture2D myTexture, reMergedTexture;
		public RawImage rawImage;
		public Texture2D[] bitPlaneTextures; //, bitPlaneTexturesG, bitPlaneTexturesB;
		// public Color32[] pixelsR;//, pixelsG, pixelsB;
		private Mat[] testMatrix;
		public Texture2D[] texturedPlanes;
		// public float[,] dataR;
		
		[Serializable]
		public struct bitPlaneInfo
		{
			public int bitplaneID;
			public Color32[] pixels;
		}

		public bitPlaneInfo[] bitplanes;


	    // Start is called before the first frame update
	    void Start()
	    {
	    	
	        testMatrix = new Mat[3];
	        texturedPlanes = new Texture2D[3]; // 0 is B, 1 is G, 2 is R

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

	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        if(Input.GetKeyDown(KeyCode.Space))
	        {
	        	StartCoroutine(GetTheTextureStuff());
	        }

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

		IEnumerator GetTheTextureStuff()
		{
			yield return new WaitForEndOfFrame();

			myTexture = toTexture2D(tex);

	        Mat mat = Unity.TextureToMat(myTexture);
			testMatrix = Cv2.Split(mat);


			for(int i = 0; i < 3; i++)
			{
				texturedPlanes[i] = Unity.MatToTexture(testMatrix[i]);

			}

			// Debug.Log(testMatrix[0]);
			Texture2D texture = Unity.MatToTexture(testMatrix[showMe]);
			// texture.Apply();

			// rawImage.texture = texture;
			StartCoroutine(CalculateBitPlane());

		}

		IEnumerator CalculateBitPlane()
		{
			yield return new WaitForEndOfFrame();

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

			StartCoroutine(CreateBinaryPlanes());

		}

		IEnumerator CreateBinaryPlanes() // Now we need to get each bitplane pixel and ID and make it into a texture, then merge the texture
		{
			yield return new WaitForSeconds(3);
			// Merge is B G R order
			// for(int i = 23; i > -1; i--)
			// {
			Mat[] newmat = new Mat[24];
			for(int i = 0; i < 24; i++)
			{
				newmat[i] = Unity.TextureToMat(bitPlaneTextures[i]);
			}
			Mat materialTest = new Mat();
			Cv2.Merge(newmat, materialTest);
			// }
			
			reMergedTexture = Unity.MatToTexture(materialTest);
			reMergedTexture.Apply();
			rawImage.texture = reMergedTexture;
			// fullImage = (2 * bitplanes[7].pixels);

			// for(int j = 7; j > 0; j++)
			// {
			// 	fullImage += bitplanes[j-1].pixels;
			// 	fullImage *= 2;
			// 	// bitPlaneTexturesR[j].SetPixels32(new Color32((byte)bitplanes[j].pixels, 0,0,1),0);
			// }
			// for(int j = 15; j > 8; j++)
			// {
			// 	fullImage += bitplanes[j-1].pixels;
			// 	fullImage *= 2;
			// 	// bitPlaneTexturesR[j].SetPixels32(new Color32((byte)bitplanes[j].pixels, 0,0,1),0);
			// }
			
		}


	}

}
