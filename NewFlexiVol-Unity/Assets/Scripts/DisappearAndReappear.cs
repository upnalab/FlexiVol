using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class DisappearAndReappear : MonoBehaviour
{
	public bool ascending;
	public float newPosY;

    // Start is called before the first frame update
    void Start()
    {
        newPosY = this.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
    	if(this.transform.localPosition.y < -0.25f)
		{
			ascending = true;

		}
		if(this.transform.localPosition.y > 0.02f)
		{
    		ascending = false;

		}

    	if(Voxon.Input.GetKey("Space"))
    	{
    		if(ascending)
    		{
	    		newPosY = newPosY + 0.005f;
    		}
    		else
    		{
	    		newPosY = newPosY - 0.005f;

    		}
    	}
		this.transform.localPosition = new Vector3(this.transform.localPosition.x, newPosY, this.transform.localPosition.z);

    }
}
