using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateSinglePlaneRedPixels : MonoBehaviour
{
	public int planeID;
	private int sizeImageH, sizeImageW;
	public Color32[] pixels;
	public RenderTexture tex;
	public Texture2D myTexture, bitPlaneTextures, reMergedTexture;
	public GameObject plane;
	// public Texture mat;
	// public RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        // planeID = this.GetComponent<GetPixelsSingle>().planeID % 8;
        // this.gameObject.AddComponent<MeshRenderer>();
        // this.GetComponent<MeshRenderer>().material.SetTexture("Shaders/SlicedRendererR"+planeID.ToString(), tex);
        
        sizeImageH = this.tex.height;
    	sizeImageW = this.tex.width;

    	pixels = new Color32[sizeImageW*sizeImageH];
    	// this.GetComponent<MeshRenderer>().material = ("Shaders/SlicedRendererR"+planeID.ToString());
    	myTexture = toTexture2D(tex);
		bitPlaneTextures = myTexture;


    }

    // Update is called once per frame
    void Update()
    {
        CalculateThisBitPlane();
    }

    Texture2D toTexture2D(RenderTexture rTex)
	{
	    Texture2D tex = new Texture2D(sizeImageW, sizeImageH, TextureFormat.RGB24, false);
	    RenderTexture.active = rTex;
	    tex.ReadPixels(new UnityEngine.Rect(0, 0, rTex.width, rTex.height), 0, 0);
	    tex.Apply();

	    return tex;
	}

    void CalculateThisBitPlane()
    {
		myTexture = toTexture2D(tex);
		bitPlaneTextures = myTexture;

		Color32[] readPixels = myTexture.GetPixels32(0);

		for(int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = new Color32((byte)((Mathf.Floor(readPixels[i].r / Mathf.Pow(2,(planeID+1))) % 2)*255), 0, 0, 255);
		}

		bitPlaneTextures.SetPixels32(pixels, 0);
		reMergedTexture = bitPlaneTextures;
		reMergedTexture.Apply();
		// rawImage.texture = reMergedTexture;

		
    }
}
