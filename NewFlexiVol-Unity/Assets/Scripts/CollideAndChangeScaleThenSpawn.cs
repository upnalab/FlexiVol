using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is the mushroom script, that scales down and spwans objects
The object should be centered Y-wise around the ground! (to make disappear on the ground, otherwise it's at its center)
 */ 

public class CollideAndChangeScaleThenSpawn : MonoBehaviour
{
	public bool isTouched = false;
	public bool availableToTouch = true;
	public GameObject touchingObject;
	private Vector3 originPosition;
	private Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Collider>().isTrigger = true;
        originalScale = this.transform.localScale;
        availableToTouch = true;
        isTouched = false;
    }

    // Update is called once per frame
    void Update()
    {                
        if(isTouched)
        {
        	availableToTouch = false;
        	Vector3 pushingDownFrom = touchingObject.transform.localPosition - originPosition;
        	this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y + pushingDownFrom.y, this.transform.localScale.z);

        	if(this.transform.localScale.y < 0.01f)
        	{
        		for(int i = 0; i < 3; i++)
        		{
        			Vector3 newPosition = new Vector3(this.transform.localPosition.x + Random.Range(-2.0f, 2.0f), this.transform.localPosition.y, this.transform.localPosition.z + Random.Range(-2.0f, 2.0f));
        			GameObject newObject = (GameObject)Instantiate(this.gameObject, newPosition, Quaternion.identity);
        			newObject.transform.localScale = originalScale;
        		}
        		Destroy(this.gameObject);
        	}
        }
    }

    void OnTriggerEnter(Collider other)
    {
    	if(availableToTouch)
    	{
    		if(other.GetComponent<Collider>().tag == "InteractiveObject")
	    	{
	    		this.isTouched = true;
	    	}
	    	touchingObject = GameObject.Find(other.GetComponent<Collider>().name);
	    	originPosition = touchingObject.transform.localPosition;
		}
    }


    void OnTriggerExit(Collider other)
    {
		if(other.GetComponent<Collider>().name == touchingObject.name)
    	{
    		this.isTouched = false;
    		availableToTouch = true;
    		this.transform.localScale = originalScale;
    	}
    }


}
