using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSharkMouth : MonoBehaviour
{
	public bool ascending;
	public float newPosX;

    // Start is called before the first frame update
    void Start()
    {
        newPosX = this.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
    	if(this.transform.localPosition.x < -0.0175f)
		{
			ascending = true;

		}
		if(this.transform.localPosition.x > -0.0035f)
		{
    		ascending = false;

		}

    	if(Voxon.Input.GetKey("Open"))
    	{
    		if(ascending)
    		{
	    		newPosX = newPosX + 0.0005f;
    		}
    		else
    		{
	    		newPosX = newPosX - 0.0005f;

    		}
    	}
		this.transform.localPosition = new Vector3(newPosX, this.transform.localPosition.y, this.transform.localPosition.z);

    }
}
