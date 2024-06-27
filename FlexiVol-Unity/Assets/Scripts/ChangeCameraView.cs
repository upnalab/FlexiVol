using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraView : MonoBehaviour
{

	public int frequency;
	public int frequencyUnity = 60;

	public int nbSlices = 24;
	public float amplitude; // amplitude of the device, should convert into the depth of the camera
	public float maxDistance, minDistance;
	public Camera camera;
	private float time0, timeStart, timeFinish;
	private float factor;

	private float meanMe, addToMean, meanTimeFrame;

    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
        time0 = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	timeStart = Time.time;
       

    	// should do a coroutine so it's not per frame -> do it every second, 24 slices
        camera.nearClipPlane = minDistance + (maxDistance - minDistance) * Mathf.Sin(2*Mathf.PI * frequency*frequencyUnity*nbSlices * (Time.time - time0)/1000);
        camera.farClipPlane = camera.nearClipPlane + 0.05f;

    }

    void LateUpdate()
    {
    	// timeFinish = Time.time;
    	// addToMean = (timeFinish - timeFinish);
    	// meanMe = meanMe + 1;

    	// meanTimeFrame = meanTimeFrame + addToMean;
    	// StartCoroutine(GetUnityFrameRate());

    }

    // IEnumerator GetUnityFrameRate()
    // {
    // 	Debug.Log("Mean frameRate = " + meanTimeFrame/meanMe);
    // 	factor = frequency / meanTimeFrame/meanMe;
    // 	yield return new WaitForSeconds(1);
    	
    // 	meanMe = 0;
    // 	meanTimeFrame = 0;
    // }
}
