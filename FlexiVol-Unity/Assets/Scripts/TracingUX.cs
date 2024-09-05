using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Voxon;

public class TracingUX : MonoBehaviour
{
	public bool realGame;
	public bool sticky;
	public string userName;
	public bool interactWithFinger;
	public bool rightHanded;
	public int nbBlocMax, trialNumber;

	public float addAngle, addDistance;
	public GameObject interactiveObject;
	public GameObject[] circuits;
	public Vector3[] startingPositions;

	private int config;

	public float stopWatch;
	private int frame;
	private float startStopWatchTime;

	private string time0;
	[HideInInspector]
	public int state;
	[Tooltip("List of Past Configurations")]
	public List<int> configException;

	private GameObject voxonSpace;

	private string path;
	private StreamWriter writer;
	private int nbBloc;
	private bool startOver;

    // Start is called before the first frame update
    void Start()
    {
        time0 = System.DateTime.Now.ToString("ddMMyyyy-HHmm");
		state = -2;

        if(interactWithFinger)
        {
        	if(rightHanded)
        	{
        		GameObject.Find("RightIndex").tag = "Index";
	        	GameObject.Find("RightThumb").tag = "Thumb";
    	        
    	        GameObject.Find("RightIndex").AddComponent<Rigidbody>();
				GameObject.Find("RightIndex").GetComponent<Rigidbody>().isKinematic = true;
				GameObject.Find("RightIndex").GetComponent<Rigidbody>().useGravity = false;

				GameObject.Find("RightIndex").AddComponent<VXDynamicComponent>();
				GameObject.Find("RightIndex").AddComponent<CorrectionMesh>();

				GameObject.Find("RightThumb").AddComponent<Rigidbody>();
				GameObject.Find("RightThumb").GetComponent<Rigidbody>().isKinematic = true;
				GameObject.Find("RightThumb").GetComponent<Rigidbody>().useGravity = false;

				GameObject.Find("RightThumb").AddComponent<VXDynamicComponent>();
				GameObject.Find("RightThumb").AddComponent<CorrectionMesh>();

				if(sticky)
				{
					GameObject.Find("RightThumb").SetActive(false);

				}

	        	GameObject.Find("LeftIndex").SetActive(false);
	        	GameObject.Find("LeftThumb").SetActive(false);
        	}
        	else
        	{
	        	GameObject.Find("LeftIndex").tag = "Index";
	        	GameObject.Find("LeftThumb").tag = "Thumb";
    	        
    	        GameObject.Find("LeftIndex").AddComponent<Rigidbody>();
				GameObject.Find("LeftIndex").GetComponent<Rigidbody>().isKinematic = true;
				GameObject.Find("LeftIndex").GetComponent<Rigidbody>().useGravity = false;

				GameObject.Find("LeftIndex").AddComponent<VXDynamicComponent>();
				GameObject.Find("LeftIndex").AddComponent<CorrectionMesh>();

				GameObject.Find("LeftThumb").AddComponent<Rigidbody>();
				GameObject.Find("LeftThumb").GetComponent<Rigidbody>().isKinematic = true;
				GameObject.Find("LeftThumb").GetComponent<Rigidbody>().useGravity = false;

				GameObject.Find("LeftThumb").AddComponent<VXDynamicComponent>();
				GameObject.Find("LeftThumb").AddComponent<CorrectionMesh>();

				if(sticky)
				{
					GameObject.Find("LeftThumb").SetActive(false);

				}

	        	GameObject.Find("RightIndex").SetActive(false);
	        	GameObject.Find("RightThumb").SetActive(false);
        	}
        	// GameObject.Find("PinchPosition").SetActive(true);
        }
        else
        {
        	GameObject.Find("HandsUpdate").SetActive(false);
        }

        circuits = new GameObject[GameObject.Find("Primitives").transform.childCount];
        startingPositions = new Vector3[GameObject.Find("Primitives").transform.childCount];
        for(int i = 0; i < circuits.Length; i++)
        {
        	circuits[i] = GameObject.Find("Primitives").transform.GetChild(i).gameObject;
        	startingPositions[i] = circuits[i].transform.GetChild(0).transform.position;
        }

        interactiveObject = GameObject.Find("InteractableSphere");
        voxonSpace = GameObject.Find("constrained_size");

        RecordPerformance();
        RecordTrajectory();

        config = -1;
    }

    // Update is called once per frame
    void Update()
    {
        for(int m = 0; m < configException.Count; m++)
		{
			for(int p = 0; p < configException.Count; p++)
			{
				if(p != m)
				{
					if(configException[m] == configException[p])
					{
						configException.Remove(configException[m]);
					}
				}
			}
		}

		switch(state)
    	{
    		case -2:
	    		if(interactiveObject.GetComponent<VXComponent>() != null)
		    	{
		        	Destroy(interactiveObject.GetComponent<VXComponent>());
		    	}
		    	if(interactiveObject.GetComponent<VXDynamicComponent>() == null)
		    	{
		    		interactiveObject.AddComponent<VXDynamicComponent>();
		        	interactiveObject.AddComponent<CorrectionMesh>();
		    	}
		    	if(interactWithFinger)
		    	{
		    		if(interactiveObject.GetComponent<Rigidbody>() == null)
		    		{
		    			interactiveObject.AddComponent<Rigidbody>();
		    			interactiveObject.GetComponent<Rigidbody>().isKinematic = true;
		    			interactiveObject.GetComponent<Rigidbody>().useGravity = false;		    			
		    		}
		    		if(interactiveObject.GetComponent<CollideAndChangeParents>() == null)
    				{
	    				interactiveObject.AddComponent<CollideAndChangeParents>();
    				}

    				if(GameObject.FindGameObjectWithTag("Index").GetComponent<VXComponent>() != null)
		    		{
		    			Destroy(GameObject.FindGameObjectWithTag("Index").GetComponent<VXComponent>());
		    		}
		    		
		    		if((GameObject.FindGameObjectWithTag("Thumb") != null) && (GameObject.FindGameObjectWithTag("Thumb").GetComponent<VXComponent>() != null))
		    		{
		    			Destroy(GameObject.FindGameObjectWithTag("Thumb").GetComponent<VXComponent>());
		    		}

		    	}
		    	else
		    	{
		    		if(interactiveObject.GetComponent<SpaceMouseGrow>() == null)
    				{
    					interactiveObject.AddComponent<SpaceMouseGrow>();
    				}
		    	}

		    	for(int i = 0; i < circuits.Length; i++)
				{
					circuits[i].SetActive(false);
				}
		    	    			

    			if(Voxon.Input.GetKeyDown("Space"))
				{
					// Wait to upload and put the meshes
					// interactiveObject.SetActive(false);
					interactiveObject.GetComponent<MeshRenderer>().enabled = false;
					state = -1;
				}
    			break;

    		case -1:
    			if(configException.Count == 0)
				{
					config = 0;
				}
				else
				{
					config = configException.Count;
				}

				state = 0;
				break;

			case 0:
				configException.Add(config);

				for(int i = 0; i < circuits.Length; i++)
				{
					if(i != config)
					{
						circuits[i].SetActive(false);
					}
				}

				circuits[config].SetActive(true);

				circuits[config].AddComponent<VXDynamicComponent>();
				circuits[config].GetComponent<VXDynamicComponent>().flags = Voxon.Flags.LINES;
		    	circuits[config].AddComponent<CorrectionMesh>();

		    	if(circuits[config].GetComponent<VXComponent>() != null)
		    	{
		    		Destroy(circuits[config].GetComponent<VXComponent>());
		    	}

				interactiveObject.transform.position = startingPositions[config];

				startStopWatchTime = Time.time;
				frame = 0;
    			addDistance = 0;
    			addAngle = 0;
    			stopWatch = 0;

    			StartCoroutine(WaitForRendererAfterCircuit());
    			state = 1;

				break;

			case 1:
    			stopWatch = Time.time - startStopWatchTime;
    			RecordTrajectory(nbBloc, config, frame, stopWatch, interactiveObject.transform.position.x, interactiveObject.transform.position.y, interactiveObject.transform.position.z);
    			StartCoroutine(ComputeAccumulatedDistances());
    			frame = frame + 1;

    			if(Voxon.Input.GetKeyDown("Space"))
    			{
    				state = 2;
    			}

    			break;

			case 2:
		    	RecordPerformance(nbBloc, config, stopWatch, addDistance, addAngle);
				circuits[config].SetActive(false);
				interactiveObject.GetComponent<MeshRenderer>().enabled = false;
				interactiveObject.GetComponent<CollideAndChangeParents>().indexCollider = false;
		    	trialNumber = trialNumber + 1;
    			if(configException.Count >= circuits.Length)
				{
					nbBloc = nbBloc + 1;
					if(nbBloc >= nbBlocMax)
					{
						Application.Quit();
						Debug.Break();
					}
					else
					{
						startOver = true;
					}
				}
				else
				{
					state = -1;
				}

				if(startOver)
				{
					configException.Clear();
					configException = new List<int>();
					startOver = false;
					state = -1;
				}
    			break;

		}

    }

    void RecordPerformance(int blockID = 0, int configID = 0, float clock = 0, float addDistance = 0, float addAngle = 0)
    {
		if(interactWithFinger)
    	{
    		if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/TracingUX/Fingers/" + userName + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/Fingers/Hand-PathTracing-Countdown" + time0 + ".csv";
    		}

    	}
    	else
    	{
	    	if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/TracingUX/3DMouse/" + userName + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/3DMouse/3DMouse-PathTracing-Countdown" + time0 + ".csv";
    		}
    	}
    	if(state == -2)
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine("BlockID;Config;StopWatch;AccDistance;AccAngle");
			writer.Close();
    	}
    	else
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine(blockID + ";" + configID + ";" + clock + ";" + addDistance + ";" + addAngle);
			writer.Close();
    	}
    }

    void RecordTrajectory(int blockID = 0, int config = 0, int frameID = 0, float clock = 0, float posX = 0, float posY = 0, float posZ = 0)
    {
		if(interactWithFinger)
    	{
    		if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/TracingUX/Fingers/" + userName + "-Circuit" + config + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/Fingers/Hand-PathTracing-Circuit" + config + "-" + time0 + ".csv";
    		}

    	}
    	else
    	{
	    	if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/TracingUX/3DMouse/" + userName + "-Circuit" + config + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/3DMouse/3DMouse-PathTracing-Circuit" + config + time0 + ".csv";
    		}
    	}
    	if(state == -2)
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine("BlockID;Frame;StopWatch;PosX;PosY;PosZ");
			writer.Close();
    	}
    	else
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine(blockID + ";" + frameID + ";" + clock + ";" + posX.ToString("F4") + ";" + posY.ToString("F4") + ";" + posZ.ToString("F4"));
			writer.Close();
    	}
    }

    IEnumerator ComputeAccumulatedDistances()
	{
		Vector3 oldPosition = interactiveObject.transform.position;
		float oldAngle = interactiveObject.transform.eulerAngles.y;
		Vector3 oldOrient = interactiveObject.transform.forward;
		yield return new WaitForEndOfFrame();
		addDistance = addDistance + Vector3.Distance(oldPosition, interactiveObject.transform.position)/10; // 10 represents basescale
		addAngle = addAngle + Vector3.Angle(oldOrient, interactiveObject.transform.forward);//Mathf.Abs((Mathf.Abs(oldAngle) - Mathf.Abs(objectToLoad.transform.eulerAngles.y))%360);
	}

	IEnumerator WaitForRendererAfterCircuit()
	{
		yield return new WaitForSeconds(2.0f);
		interactiveObject.GetComponent<MeshRenderer>().enabled = true;

	}
}
