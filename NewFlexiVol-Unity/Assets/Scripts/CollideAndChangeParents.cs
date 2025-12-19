using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideAndChangeParents : MonoBehaviour
{
	public bool indexCollider, thumbCollider;
	public bool realGame, sticky;
	public Material materialCaught, materialDefault;
	public bool caught;
    // Start is called before the first frame update
    void Start()
    {
    	this.GetComponent<MeshCollider>().convex = true;
        this.GetComponent<MeshCollider>().isTrigger = true;
        sticky = GameObject.FindObjectOfType<TracingUX>().sticky;

        realGame = GameObject.FindObjectOfType<TracingUX>().realGame;
        materialCaught = Resources.Load("Materials/2_Blue", typeof(Material)) as Material;
        materialDefault = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
    	if(sticky)
    	{
    		if(indexCollider)
	        {
	        	caught = true;
	        }
    	}
    	else
    	{
    		if(indexCollider && thumbCollider)
	        {
	        	caught = !caught;
	        }
    	}
        

        if(indexCollider && caught)
        {
        	this.transform.parent = GameObject.FindGameObjectWithTag("Index").transform;
        	this.transform.GetComponent<MeshRenderer>().material = materialCaught;
        }
        else
        {
        	this.transform.parent = GameObject.Find("_camera").transform;
        	this.transform.GetComponent<MeshRenderer>().material = materialDefault;
        }


        if(!realGame)
        {
        	if(Input.GetKeyDown(KeyCode.Keypad1))
        	{
        		indexCollider = !indexCollider;
        		thumbCollider = !thumbCollider;
        	}
        }
    }

    void LateUpdate()
    {
    	this.thumbCollider = false;
    }

    void OnTriggerEnter(Collider other)
    {
    	if(realGame)
    	{
	    	if(other.GetComponent<Collider>().tag == "Index")
	    	{
	    		this.indexCollider = true;
	    	}
	    	// if(other.GetComponent<Collider>().tag == "Thumb")
	    	// {
	    	// 	this.thumbCollider = true;
	    	// }
    	}

    }

    void OnTriggerExit(Collider other)
    {
    	if(realGame)
    	{
	    	if(other.GetComponent<Collider>().tag == "Index")
	    	{
	    		this.indexCollider = false;
	    	}
	    	if(other.GetComponent<Collider>().tag == "Thumb")
	    	{
	    		this.thumbCollider = true;
	    	}
    	}
    }
}
