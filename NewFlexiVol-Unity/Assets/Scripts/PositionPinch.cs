using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPinch : MonoBehaviour
{
	public Transform thumb, index;
	private Vector3 viewFinderScale;

    // Start is called before the first frame update
    void Start()
    {
		viewFinderScale = GameObject.Find("constrained_size").transform.localScale*10;

		if(GameObject.FindObjectOfType<DockingUX>() != null)
		{
			if(GameObject.FindObjectOfType<DockingUX>().interactWithFinger)
	    	{
	    		thumb = GameObject.FindGameObjectWithTag("Thumb").transform;
		        index = GameObject.FindGameObjectWithTag("Index").transform;
	    	}
		}
    	if(GameObject.FindObjectOfType<TracingUX>() != null)
    	{
	    	if(GameObject.FindObjectOfType<TracingUX>().interactWithFinger)
	    	{
	    		thumb = GameObject.FindGameObjectWithTag("Thumb").transform;
		        index = GameObject.FindGameObjectWithTag("Index").transform;
	    	}
	    }
    }

    // Update is called once per frame
    void Update()
    {
    	if((thumb == null) && (GameObject.FindGameObjectWithTag("Thumb") != null))
    	{
    		thumb = GameObject.FindGameObjectWithTag("Thumb").transform;
    	}
    	if(index == null)
    	{
    		index = GameObject.FindGameObjectWithTag("Index").transform;
    	}
        this.transform.position = (thumb.position + index.position)/2;
		this.transform.forward = (thumb.position - index.position);

		this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, -(float)viewFinderScale.x/2, (float)viewFinderScale.x/2), Mathf.Clamp(this.transform.position.y, -(float)viewFinderScale.y/2, (float)viewFinderScale.y/2), Mathf.Clamp(this.transform.position.z, -(float)viewFinderScale.z/2, (float)viewFinderScale.z/2));

        
    }
}
