using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPinch : MonoBehaviour
{
	public Transform thumb, index;
    // Start is called before the first frame update
    void Start()
    {
    	if(GameObject.FindObjectOfType<DockingUX>().interactWithFinger)
    	{
    		thumb = GameObject.FindGameObjectWithTag("Thumb").transform;
	        index = GameObject.FindGameObjectWithTag("Index").transform;
    	}
    }

    // Update is called once per frame
    void Update()
    {
    	if(thumb == null)
    	{
    		thumb = GameObject.FindGameObjectWithTag("Thumb").transform;
        	index = GameObject.FindGameObjectWithTag("Index").transform;
    	}
        this.transform.position = (thumb.position + index.position)/2;
		this.transform.forward = (thumb.position - index.position);
        
    }
}
