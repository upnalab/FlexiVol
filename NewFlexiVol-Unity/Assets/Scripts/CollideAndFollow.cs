using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is the grasping and rotating script */ 
public class CollideAndFollow : MonoBehaviour
{
	public bool caught;

	public bool index, middle, thumb, recordPos;

	private GameObject indexObject, thumbObject;
    private Vector3 vectorToFollow;
    private int state;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Collider>().isTrigger = true;
        // I made the collider * 1.1 in the inspector window.
        caught = true;
        state = 1;

    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case 1:
                    
                    indexObject = GameObject.Find("1_index_first");
                    thumbObject = GameObject.Find("1_thumb_first");
                    state = 2;
                    break;

                case 2:
                
	                if(index && middle && thumb)
			        {
			            caught = true;
			        }
			        else
			        {
			            caught = false;
			            recordPos = false;
			        }

			        if(caught)
			        {
			        	if(!recordPos)
			        	{
			        		vectorToFollow = (indexObject.transform.position - thumbObject.transform.position).normalized;
				        	this.transform.parent = indexObject.transform;
				        	recordPos = true;
			        	}
			        	else
			        	{
			        		this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y + Vector3.SignedAngle(vectorToFollow, (indexObject.transform.position - thumbObject.transform.position), Vector3.up), this.transform.eulerAngles.z);
		          			vectorToFollow = (indexObject.transform.position - thumbObject.transform.position);
			          	
			        	}
			        	
			        }
			        else
			        {
			        	this.transform.parent = null;
			        }
                	break;

        }
    }

    void OnTriggerEnter(Collider other)
    {    	
        if(other.GetComponent<Collider>().name.Contains("1_index_tip") || other.GetComponent<Collider>().name.Contains("1_index_third"))
    	{
    		index = true;
    	}

        if(other.GetComponent<Collider>().name.Contains("1_middle_tip") || other.GetComponent<Collider>().name.Contains("1_middle_third"))
    	{
    		middle = true;
    	}

        if(other.GetComponent<Collider>().name.Contains("1_thumb_tip") || other.GetComponent<Collider>().name.Contains("1_thumb_third"))
    	{
    		thumb = true;
    	}

    }

    void OnTriggerExit(Collider other)
    {
    	if(other.GetComponent<Collider>().name.Contains("1_index_tip") || other.GetComponent<Collider>().name.Contains("1_index_third"))
    	{
    		index = false;
    	}

        if(other.GetComponent<Collider>().name.Contains("1_middle_tip") || other.GetComponent<Collider>().name.Contains("1_middle_third"))
    	{
    		middle = false;
    	}

        if(other.GetComponent<Collider>().name.Contains("1_thumb_tip") || other.GetComponent<Collider>().name.Contains("1_thumb_third"))
    	{
    		thumb = false;
    	}
    }

   

}
