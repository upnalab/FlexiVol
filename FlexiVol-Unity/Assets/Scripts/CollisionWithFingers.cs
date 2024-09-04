using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionWithFingers : MonoBehaviour
{
	public bool indexCollider, thumbCollider;
	public bool realGame;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshCollider>().convex = true;
        this.GetComponent<MeshCollider>().isTrigger = true;
        realGame = GameObject.FindObjectOfType<DockingUX>().realGame;


    }

    // Update is called once per frame
    void Update()
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
