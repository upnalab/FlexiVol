using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideAndDisappear : MonoBehaviour
{
	public bool isTouched = false;
    // Start is called before the first frame update
    void Start()
    {
        // this.GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
        	isTouched = true;
        }
    }

	void OnTriggerEnter(Collider other)
    {
    	if(other.GetComponent<Collider>().tag == "InteractiveObject")
    	{
    		this.isTouched = true;
    	}

    }

}
