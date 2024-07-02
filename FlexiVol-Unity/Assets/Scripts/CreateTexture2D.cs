namespace OpenCvSharp.Demo
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using OpenCvSharp;
	using UnityEngine.UI;


	public class CreateTexture2D : MonoBehaviour
	{
		public int showMe;
		public RenderTexture tex;
		public Texture2D myTexture;
		public RawImage rawImage;
		// public Texture2D[] bitPlaneTexturesR, bitPlaneTexturesG, bitPlaneTexturesB;
		// public Color32[] pixelsR;//, pixelsG, pixelsB;
		private Mat[] testMatrix;
		public Texture2D[] texturedPlanes;

	    // Start is called before the first frame update
	    void Start()
	    {
	    	// myTexture = toTexture2D(tex);

	        // Mat mat = Unity.TextureToMat (myTexture);
	        testMatrix = new Mat[3];
	        texturedPlanes = new Texture2D[3];
	    	// bitPlaneTexturesR = new Texture2D[8];
	        // bitPlaneTexturesG = new Texture2D[8];
	        // bitPlaneTexturesB = new Texture2D[8];

	        // pixelsR = new Color32[myTexture.GetPixels32(0).Length];
	        // pixelsG = new Color32[128];//[myTexture.GetPixels32(0).Length];
	        // pixelsB = new Color32[128];//[myTexture.GetPixels32(0).Length];
	        
	    	// for(int i = 0; i < 8; i++)
	    	// {
	    	// 	bitPlaneTexturesR[i] = (Texture2D)Instantiate(myTexture, this.transform);
	    	// 	// GameObject newPlanes = (GameObject)Instantiate(this.gameObject, this.transform);
	    	// 	// newPlanes.GetComponent<MeshRenderer>().material.SetTexture("Tarace", bitPlaneTexturesR[i]);
	    	// 	// bitPlaneTexturesG[i] = (Texture2D)Instantiate(myTexture, this.transform);
	    	// 	// bitPlaneTexturesB[i] = (Texture2D)Instantiate(myTexture, this.transform);

	    	// }
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        myTexture = toTexture2D(tex);

	        Mat mat = Unity.TextureToMat (myTexture);
			// Mat grayMat = new Mat ();
			// Cv2.CvtColor (mat, grayMat, ColorConversionCodes.BGR2GRAY); 
			// Texture2D texture = Unity.MatToTexture (grayMat);
			// texture.Apply();

			testMatrix = Cv2.Split(mat);

			for(int i = 0; i < 3; i++)
			{
				texturedPlanes[i] = Unity.MatToTexture(testMatrix[i]);
			}
			Texture2D texture = Unity.MatToTexture (testMatrix[showMe]);
			texture.Apply();

			// Use GetPixelData to get an array that points to mipmap level 1
	        // var mip1Data = myTexture.GetPixelData<Color32>(0);
	        // Debug.Log(mip1Data.Length);


                // Utils.matToTexture2D(rgbMat, texture);
	        // Color32[] pixelsTexture = myTexture.GetPixels32(0);

	        // System.Array.SetValue(Mathf.Floor(pixelsTexture.r/ Mathf.Pow(2,j)) % 2);

	        
        	// for(int j = 0; j < 8; j++)
        	// {
        	// 	for(int i = 0; i < mip1Data.Length; i++)
	        // 	{
	        // 		// var dataR = Mathf.Floor(pixelsTexture[i].r/ Mathf.Pow(2,j)) % 2;
	        // 		// var dataG = Mathf.Floor(pixelsTexture[i].g/ Mathf.Pow(2,j)) % 2;
	        // 		// var dataB = Mathf.Floor(pixelsTexture[i].b/ Mathf.Pow(2,j)) % 2;

	        // 		pixelsR[i] = new Color32((byte)(Mathf.Floor(mip1Data[i].r/Mathf.Pow(2,(j+1))) % 2), 0, 0, 1);
	        // 		// pixelsG[i] = new Color32(0, (byte)(Mathf.Floor(pixelsTexture[i].g/Mathf.Pow(2,(j+1))) % 2), 0, 1);
	        // 		// pixelsB[i] = new Color32(0, 0, (byte)(Mathf.Floor(pixelsTexture[i].b/Mathf.Pow(2,(j+1))) % 2), 1);

		       //  	// bitPlaneTexturesR[j].SetPixels(i, Mathf.Floor(pixelsTexture[i].r/ Mathf.Pow(2,j)) % 2);
		       //  	// bitPlaneTexturesG[j].SetPixel(i, Mathf.Floor(pixelsTexture[i].g/ Mathf.Pow(2,j)) % 2);
		       //  	// bitPlaneTexturesB[j].SetPixel(i, Mathf.Floor(pixelsTexture[i].b/ Mathf.Pow(2,j)) % 2);

	        // 	}

	        // 	// bitPlaneTexturesR[j].SetPixels32(pixelsR,0);
	        // 	// bitPlaneTexturesG[j].SetPixels32(pixelsG,0);
	        // 	// bitPlaneTexturesB[j].SetPixels32(pixelsB,0);

	        // 	// bitPlaneTexturesR[j].Apply();
	        // 	// bitPlaneTexturesG[j].Apply();
	        // 	// bitPlaneTexturesB[j].Apply();

	        // }

			rawImage.texture = texture;
			// rawImage.texture = bitPlaneTexturesR[0];

	    }

	    Texture2D toTexture2D(RenderTexture rTex)
		{
		    Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
		    // ReadPixels looks at the active RenderTexture.
		    RenderTexture.active = rTex;
		    tex.ReadPixels(new UnityEngine.Rect(0, 0, rTex.width, rTex.height), 0, 0);
		    tex.Apply();

		    return tex;
		}

		


	}

}