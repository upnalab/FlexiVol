using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParentsAtCollision : MonoBehaviour
{
	public bool indexCollider, thumbCollider;
	public bool realGame;
    // Start is called before the first frame update
    void Start()
    {
        realGame = GameObject.FindObjectOfType<DockingUX>().realGame;
    }

    // Update is called once per frame
    void Update()
    {
        if(indexCollider && thumbCollider)
        {
        	this.transform.parent = GameObject.Find("PinchPosition").transform;
        }
        else
        {
        	this.transform.parent = GameObject.Find("_camera").transform;
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

    void OnTriggerEnter(Collider other)
    {
    	if(realGame)
    	{
	    	if(other.GetComponent<Collider>().tag == "Index")
	    	{
	    		this.indexCollider = true;
	    	}
	    	if(other.GetComponent<Collider>().tag == "Thumb")
	    	{
	    		this.thumbCollider = true;
	    	}
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
	    		this.thumbCollider = false;
	    	}
    	}
    }
}
