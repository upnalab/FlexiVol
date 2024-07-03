using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantiatePlanes : MonoBehaviour
{
	public GameObject firstPlane, secondPlane;
	public RenderTexture[] renderTextures;
	public Material[] renderMaterials;
	// public RawImage rawImage;

	[Serializable]
	public struct PlaneInstance
	{
		public GameObject planeObj;
		public int bitplaneID;
		// public Color32[] pixels;
		// public RenderTexture rTex;
	}

	private PlaneInstance[] planes;
    // Start is called before the first frame update
    void Start()
    {
    	planes = new PlaneInstance[24];
        for(int i = 0; i < 24; i++)
        {
        	planes[i].planeObj = (GameObject)Instantiate(firstPlane, this.transform);
        	planes[i].planeObj.transform.position = new Vector3(firstPlane.transform.position.x, firstPlane.transform.position.y, firstPlane.transform.position.z + (float)i/24);
        	// planes[i].AddComponent<GetPixelsSingle>();
        	// planes[i].GetComponent<GetPixelsSingle>().planeID = i;
        	planes[i].bitplaneID = i;

        	if(planes[i].bitplaneID < 8)
	        {
	        	planes[i].planeObj.gameObject.AddComponent<CalculateSinglePlaneRedPixels>();
	        	planes[i].planeObj.gameObject.GetComponent<CalculateSinglePlaneRedPixels>().planeID = planes[i].bitplaneID % 8;
	        	planes[i].planeObj.gameObject.GetComponent<CalculateSinglePlaneRedPixels>().tex = renderTextures[i];
	        	planes[i].planeObj.gameObject.GetComponent<CalculateSinglePlaneRedPixels>().plane = (GameObject)Instantiate(secondPlane, secondPlane.gameObject.transform);
	        	planes[i].planeObj.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = renderMaterials[i];

	        }
	        else
	        {
	        	if(planes[i].bitplaneID < 16)
	        	{
		        	planes[i].planeObj.gameObject.AddComponent<CalculateSinglePlaneGreenPixels>();
		        	// planes[i].planeObj.gameObject.GetComponent<CalculateSinglePlaneGreenPixels>().tex = renderTextures[i];
	        	}
	        	else
	        	{
		        	planes[i].planeObj.gameObject.AddComponent<CalculateSinglePlaneBluePixels>();        		
	        	}
	        }



        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
