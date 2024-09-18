using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class ControlSphere : MonoBehaviour
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
    	if(Voxon.Input.GetKey("A"))
    	{
    		posX = posX - 0.05f;
    	}
    	if(Voxon.Input.GetKey("D"))
    	{
    		posX = posX + 0.05f;
    	}

    	if(Voxon.Input.GetKey("W"))
    	{
    		posZ = posZ - 0.05f;
    	}
    	if(Voxon.Input.GetKey("S"))
    	{
    		posZ = posZ + 0.05f;
    	}

    	if(Voxon.Input.GetKey("Z"))
    	{
    		posY = posY - 0.05f;
    	}
    	if(Voxon.Input.GetKey("X"))
    	{
    		posY = posY + 0.05f;
    	}

    	if(Voxon.Input.GetKey("Q"))
    	{
    		scale = scale - 0.005f;
    	}
    	if(Voxon.Input.GetKey("E"))
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
