
namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System.Collections;
	using OpenCvSharp;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEditor.Scripting.Python;

public class GenerateBitPlanes : MonoBehaviour
{
	private int sizeImageH, sizeImageW;
	public RenderTexture tex;
	public Texture2D myTexture, reMergedTexture;
	public RawImage rawImage;
	private Texture2D[] bitPlaneTextures;

	[Serializable]
	public struct bitPlaneInfo
	{
		public int bitplaneID;
		public Color32[] pixels;
	}

	private bitPlaneInfo[] bitplanes;
	public GenerateSlice cuttingPlane;


    // Start is called before the first frame update
    void Start()
    {
        sizeImageH = this.tex.height;
    	sizeImageW = this.tex.width;
    	cuttingPlane = this.GetComponent<GenerateSlice>();

        bitplanes = new bitPlaneInfo[24];
    	bitPlaneTextures = new Texture2D[24];

    	for(int j = 0; j < 8; j++)
		{
			bitplanes[j].bitplaneID = j;
			bitplanes[j].pixels = new Color32[sizeImageW*sizeImageH];

			bitplanes[j+8].bitplaneID = j+8;
			bitplanes[j+8].pixels = new Color32[sizeImageW*sizeImageH];

			bitplanes[j+16].bitplaneID = j+16;
			bitplanes[j+16].pixels = new Color32[sizeImageW*sizeImageH];

		}

		myTexture = toTexture2D(tex);
		for(int i = 0; i < 24; i++)
		{
			bitPlaneTextures[i] = myTexture;

		}
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
        	// StartCoroutine(CalculateBitPlane());
        	StartCoroutine(CalculateGrayScale());
        	StartCoroutine(RunPythonRoutineFile());
        }
        StartCoroutine(MeasureFrameRate());
        

    }

    Texture2D toTexture2D(RenderTexture rTex)
	{
	    Texture2D tex = new Texture2D(sizeImageW, sizeImageH, TextureFormat.RGB24, false);
	    // ReadPixels looks at the active RenderTexture.
	    RenderTexture.active = rTex;
	    tex.ReadPixels(new UnityEngine.Rect(0, 0, rTex.width, rTex.height), 0, 0);
	    tex.Apply();

	    return tex;
	}

	IEnumerator CalculateGrayScale()
	{
		myTexture = toTexture2D(tex);
		Mat mat = Unity.TextureToMat (myTexture);
		Mat grayMat = new Mat ();
		Cv2.CvtColor (mat, grayMat, ColorConversionCodes.BGR2GRAY); 
		Texture2D texture = Unity.MatToTexture (grayMat);
		SaveTextureAsPNG(texture, "./Assets/Shaders/Materials/TextureAsPNG.png");
		rawImage.texture = texture;
		yield return new WaitForEndOfFrame();

		StartCoroutine(CalculateGrayScale());

	}

	public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes =_texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length/1024  + "Kb was saved as: " + _fullPath);
    }

	IEnumerator RunPythonRoutineFile()
    {
    	PythonRunner.RunFile($"./Assets/Python/CreateBitPlanes.py");
    	yield return new WaitForEndOfFrame();
    	StartCoroutine(RunPythonRoutineFile());
    }

	IEnumerator MeasureFrameRate()
	{
		float timeBegin = Time.time;
		yield return new WaitForEndOfFrame();
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
			// Debug.Log(j + ";" + bitplanes[j].pixels[150]);
			reMergedTexture = bitPlaneTextures[j];
			reMergedTexture.Apply();
			rawImage.texture = reMergedTexture;
			// bitPlaneTextures[2*j].SetPixels32(bitplanes[2*j].pixels, 0);
			// reMergedTexture = bitPlaneTextures[2*j];
			// reMergedTexture.Apply();
			// rawImage.texture = reMergedTexture;
			yield return new WaitForEndOfFrame();
			// yield return new WaitForFixedUpdate();

		}
		// yield return new WaitForFixedUpdate();
		// yield return new WaitForEndOfFrame();
		float timeDebug = Time.time - timeEntering;
		Debug.Log("BitPlanes: " + timeDebug);

		// Probably need to launch it frequency per second -> not every frame ((1/Time.deltaTime)/11)
    	StartCoroutine(CalculateBitPlane());


	}
}
}