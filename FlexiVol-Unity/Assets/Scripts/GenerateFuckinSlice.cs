using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFuckinSlice : MonoBehaviour
{
	public float frequency;
	public float phase;
	public Camera cameraFront, cameraBack;
	public int nbSlices = 12;
    private float time0;
    private float maxDistance, minDistance;

    public GameObject cuttingPlane;


//  I THINK WE NEED THE BITMAP WHATEVER STUFF TO CUT THE PLANES

    // void Awake()
    // {
    //     cameraBack.clearFlags = CameraClearFlags.Nothing;

    // }

    // Start is called before the first frame update
    void Start()
    {
        maxDistance = 1;
        minDistance = 0;
        time0 = Time.time;
        // cameraBack.clearFlags = CameraClearFlags.Depth;

        // cameraBack.gameObject.transform.forward = -cameraFront.gameObject.transform.forward;
        // cameraBack.gameObject.transform.up = cameraFront.gameObject.transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.K))
        // {
        //     // cameraBack.targetDisplay = 0;
        //     if(cameraBack.clearFlags == CameraClearFlags.Depth)
        //     {
        //         cameraBack.clearFlags = CameraClearFlags.Nothing;
        //     }
        //     else
        //     {
        //         cameraBack.clearFlags = CameraClearFlags.Depth;

        //     }
        // }

        
    		// cameraFront.nearClipPlane = minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))); //currentSlice/nbSlices *
	     //    cameraFront.farClipPlane = cameraFront.nearClipPlane + maxDistance/nbSlices;
    	
    		// cameraBack.farClipPlane = maxDistance + (minDistance - maxDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))); //currentSlice/nbSlices *
	     //    cameraBack.nearClipPlane = cameraBack.farClipPlane - maxDistance/nbSlices;

        cuttingPlane.transform.localPosition = new Vector3(cuttingPlane.transform.position.x, cuttingPlane.transform.position.y, minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))));

		        // cameraBack.farClipPlane = minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase)); //currentSlice/nbSlices *
		        // cameraBack.nearClipPlane = cameraBack.farClipPlane - maxDistance/nbSlices;
        // 	}

        // 	// Debug.Log("FRONT:" + cameraFront.nearClipPlane + "; BACK: " + cameraBack.nearClipPlane);
        // }
		if(Input.GetKey(KeyCode.RightArrow))
		{
			phase = phase + 1;
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			phase = phase - 1;
		}
    }
}
