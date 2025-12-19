using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSlice : MonoBehaviour
{
	public float frequency;
	public int phase;
    public int cutSectionID;
	public float nbSlices = 12;
    private float time0, timeIndex, positionPlane;
    private float maxDistance, minDistance;

    public GameObject cuttingPlane;

    AudioSource audioSource;
    private int sampleRate = 1;
    private float posX, posY;


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
        
        audioSource.loop = true;
        posX = cuttingPlane.transform.position.x;
        posY = cuttingPlane.transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {

        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     if(!audioSource.isPlaying)
        //     {
        //         time0 = Time.unscaledTime;  //resets timer before playing sound
        //         audioSource.Play();
        //     }
        //     else
        //     {
        //         audioSource.Stop();
        //     }
        // }


        timeIndex = Time.unscaledTime - time0;
        // cuttingPlane.transform.localPosition = new Vector3(cuttingPlane.transform.position.x, cuttingPlane.transform.position.y, positionPlane);
        positionPlane = this.GetComponent<GenerateBitPlanes>().numberToRun / nbSlices * maxDistance;
        cuttingPlane.transform.localPosition = new Vector3(posX, posY, positionPlane);

        cutSectionID = (int)Mathf.Floor((positionPlane / maxDistance)*nbSlices);


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
            // positionPlane = Mathf.Abs(CreateSine(timeIndex, frequency, sampleRate, phase));    
        }

    }

        //Creates a sinewave
    public float CreateSine(float timeIndex, float frequency, float sampleRate, int phase = 0)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate + phase/(180*Mathf.PI));
    }

 
}
