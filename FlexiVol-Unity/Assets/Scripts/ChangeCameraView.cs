using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraView : MonoBehaviour
{

	public int frequency;
	// public int frequencyUnity = 60;

	public int nbSlices = 24;
    public float currentSlice, orderSlices;
	public float amplitude; // amplitude of the device, should convert into the depth of the camera
	public float maxDistance, minDistance;
	public Camera camera;

    public int oldCount, countMe, frame;
    public bool startOver, frameAchieved;
    public float time0;

    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
        maxDistance = this.transform.GetChild(0).gameObject.transform.localScale.z;
        minDistance = 0;
        time0 = Time.time;

    }

    // Update is called once per frame
    void Update()
    {
        float frameRate = 1.0f/Time.deltaTime;
        // Debug.Log(1.0f/Time.deltaTime);
        camera.nearClipPlane = minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0))); //currentSlice/nbSlices *
        // StartCoroutine(SlicingCamera());
        camera.farClipPlane = camera.nearClipPlane + maxDistance/nbSlices;

        currentSlice = Mathf.Round(camera.nearClipPlane/maxDistance * nbSlices);
        Debug.Log(currentSlice);
        // if(currentSlice == 23)
        // {
        //     countMe = countMe + 1;
        // }
        // StartCoroutine(HowManySweeps());
        // if(orderSlices >= nbSlices*2)
        // {
        //     orderSlices = nbSlices*2;
        // }
        // if(orderSlices < 0)
        // {
        //     orderSlices = 0;
        // }
        

        // if(camera.nearClipPlane > 0.9f)
        // {
        //     Debug.Log("bla");
        //     countMe = countMe + 1;

        // }
        // if(countMe > 0)
        // {
        //     startOver = false;

        //     StartCoroutine(HowManySweeps());
        // }
        // if(startOver)
        // {
        //     countMe = 0;
        //     startOver = false;
        // }

        // frame = frame+1;
        // if((frame % (1.0f/Time.deltaTime)) == 0)
        // {
        //     frameAchieved = true;
        //     frame = 0;
        // }
        // else
        // {
        //     frameAchieved = false;
        // }

        // oldCount = countMe;
    }


    IEnumerator SlicingCamera()
    {
        // float frameRate = 1.0f/Time.deltaTime;
    	yield return new WaitForEndOfFrame();//1/frequency/nbSlices);
        // Debug.Log(frameRate/frequency);
        orderSlices = (orderSlices+1)%(nbSlices*2);
        if(orderSlices >= nbSlices)
        {
            currentSlice = (currentSlice-1)%nbSlices;

        }
        else
        {
            currentSlice = (currentSlice+1)%nbSlices;
        }

    }

    IEnumerator HowManySweeps()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Debug.Log(countMe);
        // countMe = 0;
        // startOver = true;
        // frame = 0;

    }

}
