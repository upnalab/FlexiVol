using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParentsAtCollision : MonoBehaviour
{
	public bool indexCollider, thumbCollider;
	public bool realGame;
	public Material materialCaught, materialDefault;
	public bool caught, sticky;
    // Start is called before the first frame update
    void Start()
    {
        realGame = GameObject.FindObjectOfType<DockingUX>().realGame;
        materialCaught = Resources.Load("Materials/2_Blue", typeof(Material)) as Material;
        materialDefault = this.transform.GetChild(0).transform.GetComponent<MeshRenderer>().material;
        sticky = GameObject.FindObjectOfType<DockingUX>().sticky;


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
        	this.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material = materialCaught;
            if(!sticky)
            {
                this.transform.forward = -(GameObject.FindGameObjectWithTag("Thumb").transform.position - GameObject.FindGameObjectWithTag("Index").transform.position);
            }
        }
        else
        {
        	this.transform.parent = GameObject.Find("_camera").transform;
        	this.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material = materialDefault;

        }
        if(!realGame)
        {
        	if(Input.GetKeyDown(KeyCode.Keypad1))
        	{
        		indexCollider = !indexCollider;
        		thumbCollider = !thumbCollider;
        	}
        }
        else
        {
        	indexCollider = this.transform.GetChild(0).transform.GetComponent<CollisionWithFingers>().indexCollider;
	    	thumbCollider = this.transform.GetChild(0).transform.GetComponent<CollisionWithFingers>().thumbCollider;
	    	
        }
    }


}
