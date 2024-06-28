using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryCopyPlane : MonoBehaviour
{
	public bool reverse;
	public Camera cameraToCopy, camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
    	if(reverse)
    	{
    		camera.farClipPlane = cameraToCopy.farClipPlane;
			camera.nearClipPlane = cameraToCopy.nearClipPlane;
    	}
    	else
    	{
    		camera.farClipPlane = cameraToCopy.nearClipPlane;
			camera.nearClipPlane = cameraToCopy.farClipPlane;
    	}

    	// if(cameraToCopy.gameObject.GetComponent<ChangeCameraView>())
		

	}
}
