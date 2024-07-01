using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFuckinSlice : MonoBehaviour
{
	public float frequency;
	public int phase;
	public Camera cameraFront, cameraBack;
	public float nbSlices = 12;
    private float time0, timeIndex, positionPlane;
    private float maxDistance, minDistance;

    public GameObject cuttingPlane;

    AudioSource audioSource;
    private int sampleRate = 1;

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

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
        // cameraBack.clearFlags = CameraClearFlags.Depth;
        audioSource.loop = true;

        // cameraBack.gameObject.transform.forward = -cameraFront.gameObject.transform.forward;
        // cameraBack.gameObject.transform.up = cameraFront.gameObject.transform.up;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!audioSource.isPlaying)
            {
                time0 = Time.time;  //resets timer before playing sound
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
        timeIndex = Time.time - time0;
    
        cuttingPlane.transform.localPosition = new Vector3(cuttingPlane.transform.position.x, cuttingPlane.transform.position.y, positionPlane);

        
    		// cameraFront.nearClipPlane = minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))); //currentSlice/nbSlices *
	     //    cameraFront.farClipPlane = cameraFront.nearClipPlane + maxDistance/nbSlices;
    	
    		// cameraBack.farClipPlane = maxDistance + (minDistance - maxDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))); //currentSlice/nbSlices *
	     //    cameraBack.nearClipPlane = cameraBack.farClipPlane - maxDistance/nbSlices;

        // cuttingPlane.transform.localPosition = new Vector3(cuttingPlane.transform.position.x, cuttingPlane.transform.position.y, minDistance + (maxDistance - minDistance) * Mathf.Abs(Mathf.Sin(2*Mathf.PI * frequency *  (Time.time - time0) + phase/(180*Mathf.PI))));

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

    void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i = 0; i < data.Length; i+= channels)
        {          
            data[i] = CreateSine(timeIndex, frequency, sampleRate);           
            positionPlane = Mathf.Abs(CreateSine(timeIndex, frequency, sampleRate, phase));    
        }

    }

        //Creates a sinewave
    public float CreateSine(float timeIndex, float frequency, float sampleRate, int phase = 0)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate + phase/(180*Mathf.PI));
    }

 
}
