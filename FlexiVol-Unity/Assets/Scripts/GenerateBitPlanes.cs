using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        	StartCoroutine(CalculateBitPlane());
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
			reMergedTexture = bitPlaneTextures[j];
			reMergedTexture.Apply();
			rawImage.texture = reMergedTexture;
			yield return new WaitForEndOfFrame(); // HERE NEED TO WAIT ILL TIME
		}

		// yield return new WaitForEndOfFrame();
		float timeDebug = Time.time - timeEntering;
		Debug.Log("BitPlanes: " + timeDebug);
		// Probably need to launch it frequency per second -> not every frame ((1/Time.deltaTime)/11)
    	StartCoroutine(CalculateBitPlane());


	}
}
