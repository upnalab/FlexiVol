using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor.Scripting.Python;

public class InstantiateAllCutSections : MonoBehaviour
{
	public int numberSlicesPerSweep;
	public GameObject planeToReplicate;
	public RenderTexture tex;

	[Range(0f,100f)]
	public float timeScaleChange;

	public GameObject[] allBitPlanes; 

	private int sizeImageH, sizeImageW;
	private float time0;
	public bool newPatternUpload;

	public bool displayOnUnityPlane;
	private GameObject planeDisplay;
    // Start is called before the first frame update
    void Start()
    {
    	sizeImageH = this.tex.height;
    	sizeImageW = this.tex.width;
        allBitPlanes = new GameObject[24*numberSlicesPerSweep];
        for(int i = 0; i < allBitPlanes.Length; i++)
        {
        	Vector3 positionNewPlanes = new Vector3(planeToReplicate.transform.position.x, planeToReplicate.transform.position.y, (float)i/allBitPlanes.Length);
        	Quaternion orientNewPlanes = planeToReplicate.transform.rotation;

        	allBitPlanes[i] = (GameObject)Instantiate(planeToReplicate, planeToReplicate.transform.position, orientNewPlanes, GameObject.Find("Slicing Camera").transform);
        	allBitPlanes[i].transform.localPosition = positionNewPlanes;
        	allBitPlanes[i].SetActive(false);
        }
        planeToReplicate.SetActive(false);
		StartCoroutine(SweepUp());
    	time0 = Time.unscaledTime;

    	planeDisplay = GameObject.Find("Plane (1)");
    }

    // Update is called once per frame
    void Update()
    {
		// time0 = Time.unscaledTime;
    	Time.timeScale = timeScaleChange;
    	// Debug.Log("delta:" + Time.unscaledDeltaTime + " ; Freq: " + 1/Time.unscaledDeltaTime);
    	if(Input.GetKeyDown(KeyCode.Space))
        {
			StartCoroutine(SweepUp());
        }
        if(displayOnUnityPlane)
        {
        	planeDisplay.GetComponent<ChangeTexture>().enabled = true;
        }
        else
        {
        	planeDisplay.GetComponent<ChangeTexture>().enabled = false;

        }

		// StartCoroutine(GetPlanes());
    }


    IEnumerator SweepUp()
    {
    	float time1 = Time.unscaledTime;
    	int countSlicesPerVol = 0;
    	for(int i = 0; i < allBitPlanes.Length; i++)
        {
        	for(int j = 0; j < allBitPlanes.Length; j++)
        	{
        		if(j != i)
        		{
        			allBitPlanes[j].SetActive(false);
        		}
        		else
        		{
        			allBitPlanes[j].SetActive(true);
        			CalculatePNG(j%24);
        		}
        	}
    		if((i % 24) == 23)
			{
	        	RunPythonScript();
	        	countSlicesPerVol++;
    	    	time0 = Time.unscaledTime;

			}
			yield return new WaitForFixedUpdate();
	    	newPatternUpload = false;

    	}
    	Debug.Log("Slices Sweep Up: " + countSlicesPerVol);
    	Debug.Log("Sweep time: " + (Time.unscaledTime - time1));
    	// StartCoroutine(SweepDown());
    }


	IEnumerator SweepDown()
	{
    	float time1 = Time.unscaledTime;
    	int countSlicesPerVol = 0;

    	for(int i = allBitPlanes.Length-1; i > -1; i--)
        {
        	for(int j = allBitPlanes.Length-1; j > -1; j--)
        	{
        		if(j != i)
        		{
        			allBitPlanes[j].SetActive(false);
        		}
        		else
        		{
        			allBitPlanes[j].SetActive(true);
        			CalculatePNG((j+23)%24);
        		}
        	}
    		if((i % 24) == 0)
			{
	        	RunPythonScript();
	        	countSlicesPerVol++;
    	    	time0 = Time.unscaledTime;

			}
			yield return new WaitForFixedUpdate();
        	newPatternUpload = false;

    	}
    	Debug.Log("Slices Sweep Down: " + countSlicesPerVol);
    	Debug.Log("Sweep time: " + (Time.unscaledTime - time1));

    	StartCoroutine(SweepUp());
    }

    Texture2D toTexture2D(RenderTexture rTex)
	{
	    Texture2D tex = new Texture2D(sizeImageW, sizeImageH, TextureFormat.RGB24, false);
	    // ReadPixels looks at the active RenderTexture.
	    RenderTexture.active = rTex;
	    tex.ReadPixels(new UnityEngine.Rect(0, 0, rTex.width, rTex.height), 0, 0);
	    // tex.Apply();

	    return tex;
	}

	public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes =_texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        // Debug.Log(_bytes.Length/1024  + "Kb was saved as: " + _fullPath);
    }

	void CalculatePNG(int numberToRun)
	{
    	Texture2D texture = toTexture2D(tex);
		SaveTextureAsPNG(texture, "./Assets/Shaders/Materials/BitPlanes/TextureAsPNG-"+numberToRun+".png");	
	}

	void RunPythonScript()
    {
    	PythonRunner.RunFile($"./Assets/Python/CreateBitPlanes-all.py");
    	Debug.Log(Time.unscaledTime - time0);
    	newPatternUpload = true;
    }
}
