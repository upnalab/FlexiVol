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


//  I THINK WE NEED THE BITMAP WHATEVER STUFF TO CUT THE PLANES

    // Start is called before the first frame update
    void Start()
    {
        maxDistance = 1;
        minDistance = 0;
        time0 = Time.time;
        // cameraBack.gameObject.transform.forward = -cameraFront.gameObject.transform.forward;
        // cameraBack.gameObject.transform.up = cameraFront.gameObject.transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        // for(int i = 0; i < nbSlices; i++)
        // {
        // 	// Debug.Log(i);
        // 	if((i % 2) == 0)
        // 	{
        // 		Debug.Log("FRONT");
        		// cameraFront.enabled = true;

        		// cameraBack.targetDisplay = 2;
        		// cameraFront.targetDisplay = 1;
        		// cameraBack.enabled = false;
        		cameraFront.nearClipPlane = minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))); //currentSlice/nbSlices *
		        cameraFront.farClipPlane = cameraFront.nearClipPlane + maxDistance/nbSlices;
        	// }
        	// else
        	// {
        	// 	Debug.Log("BACK");
        		// cameraBack.enabled = true;
        		// cameraBack.targetDisplay = 1;
        		// cameraFront.targetDisplay = 2;

        		// cameraFront.enabled = false;
        		cameraBack.farClipPlane = maxDistance + (minDistance - maxDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))); //currentSlice/nbSlices *
		        cameraBack.nearClipPlane = cameraBack.farClipPlane - maxDistance/nbSlices;



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
