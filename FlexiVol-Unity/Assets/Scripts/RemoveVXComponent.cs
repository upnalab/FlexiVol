using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class RemoveVXComponent : MonoBehaviour
{
	int state = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	switch(state)
    	{
    		case 0:
	    		if(this.GetComponent<VXComponent>() != null)
		        {
		        	Destroy(this.GetComponent<VXComponent>());
		        	state = 1;
		        }
		        break;

		    case 1:

		    	break;
    	}
       
    }
}
