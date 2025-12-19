using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPixelsSingle : MonoBehaviour
{
	public int planeID;

    // Start is called before the first frame update
    void Start()
    {
        if(planeID < 8)
        {
        	this.gameObject.AddComponent<CalculateSinglePlaneRedPixels>();
        }
        else
        {
        	if(planeID < 16)
        	{
	        	this.gameObject.AddComponent<CalculateSinglePlaneGreenPixels>();

        	}
        	else
        	{
	        	this.gameObject.AddComponent<CalculateSinglePlaneBluePixels>();        		
        	}
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
