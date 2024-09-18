using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class ControlCube : MonoBehaviour
{
	public float posX, posY, posZ, scale;
	private Vector3 viewFinderScale;

    // Start is called before the first frame update
    void Start()
    {
        posX = this.transform.localPosition.x;
        posY = this.transform.localPosition.y;
        posZ = this.transform.localPosition.z;
		viewFinderScale = GameObject.Find("constrained_size").transform.localScale*10;
		scale = 0.1f;

    }

    // Update is called once per frame
    void Update()
    {
    	if(Voxon.Input.GetKey("4"))
    	{
    		posX = posX - 0.05f;
    	}
    	if(Voxon.Input.GetKey("6"))
    	{
    		posX = posX + 0.05f;
    	}

    	if(Voxon.Input.GetKey("8"))
    	{
			posZ = posZ - 0.05f;

    	}
    	if(Voxon.Input.GetKey("5"))
    	{
    		posZ = posZ + 0.05f;	
    	}

    	if(Voxon.Input.GetKey("1"))
    	{
    		posY = posY - 0.05f;
    	}
    	if(Voxon.Input.GetKey("3"))
    	{
    		posY = posY + 0.05f;
    	}

    	if(Voxon.Input.GetKey("7"))
    	{
    		scale = scale - 0.005f;
    	}
    	if(Voxon.Input.GetKey("9"))
    	{
    		scale = scale + 0.005f;
    	}

    	this.transform.localScale = new Vector3(scale, scale, scale);

    	if(posX > (float)viewFinderScale.x/2)
    	{
    		posX = (float)viewFinderScale.x/2;
    	}
    	if(posX < -(float)viewFinderScale.x/2)
    	{
    		posX = -(float)viewFinderScale.x/2;
    	}

    	if(posY > (float)viewFinderScale.y/2)
    	{
    		posY = (float)viewFinderScale.y/2;
    	}
    	if(posY < -(float)viewFinderScale.y/2)
    	{
    		posY = -(float)viewFinderScale.y/2;
    	}

    	if(posZ > (float)viewFinderScale.z/2)
    	{
    		posZ = (float)viewFinderScale.z/2;
    	}
    	if(posZ < -(float)viewFinderScale.z/2)
    	{
    		posZ = -(float)viewFinderScale.z/2;
    	}

		this.transform.position = new Vector3(Mathf.Clamp(posX, -(float)viewFinderScale.x/2, (float)viewFinderScale.x/2), Mathf.Clamp(posY, -(float)viewFinderScale.y/2, (float)viewFinderScale.y/2), Mathf.Clamp(posZ, -(float)viewFinderScale.z/2, (float)viewFinderScale.z/2));

    }
}
