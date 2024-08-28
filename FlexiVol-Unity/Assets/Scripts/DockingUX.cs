using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Voxon;

public class DockingUX : MonoBehaviour
{
	public string userName;
	public bool interactWithFinger;
	public GameObject primitiveToInstantiate, phantomToInstantiate;
	public bool randomizeConditions;
	public int nbBlocMax, trialNumber;
	public Vector3[] sizes;

	[Serializable]	
 	public enum directionTask{
	    X,
	    Y,
	    Z,
	    XY,
	    XZ,
	    YZ,
	    XYZ
 	}

	[Serializable]
	public struct TasksStruct{
		public string name;
		public int numberTask;
		public directionTask direction;
	}

	public TasksStruct[] tasks;
	private int config;

	public float stopWatch;
	private float startStopWatchTime;

	private Vector3[] finalPositionsCubes, targetPositionsCubes;
	public List<(int, float)> results;

	private string time0;
	private int state;
	private GameObject panelStart, interactiveObject;

	[Tooltip("List of Past Configurations")]
	public List<int> configException;

	private GameObject voxonSpace;
	private GameObject objectToLoad, phantomObject;

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
        	GameObject.Find("3DMousePosition").SetActive(false);
        	interactiveObject = GameObject.Find("HandSphere");

        }
        else
        {
        	interactiveObject = GameObject.Find("3DMousePosition");
        }

        interactiveObject.tag = "InteractiveObject";
        interactiveObject.AddComponent<Rigidbody>();
        interactiveObject.GetComponent<Rigidbody>().isKinematic = false;
        interactiveObject.GetComponent<Rigidbody>().useGravity = false;
        panelStart = GameObject.Find("PanelStart");

        voxonSpace = GameObject.Find("view_finder");
        RecordPerformance();
        RecordCubePositions();
        finalPositionsCubes = new Vector3[nbBlocMax*sizes.Length*tasks.Length];
        targetPositionsCubes = new Vector3[nbBlocMax*sizes.Length*tasks.Length];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RecordPerformance(int blockID = 0, int trialID = 0, int configID = 0, float clock = 0)
    {
    	path = "Assets/Resources/DataCollection/DockingUX/" + userName + "-" + time0 + ".csv";

    	if(state == -2)
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine("BlockID;TrialID;Config;StopWatch");
			writer.Close();
    	}
    	else
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine(blockID + ";" + trialID + ";" + configID + ";" + clock);
			writer.Close();
    	}
    }

	void RecordCubePositions(int blockID = 0, int trialID = 0, int configID = 0, Vector3 positionTarget = new Vector3(), Vector3 positionFinal = new Vector3())
    {
    	path = "Assets/Resources/DataCollection/DockingUX/" + userName + "-" + time0 + "-positionsCubes.csv";

    	if(state == -2)
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine("BlockID;TrialID;Config;PositionTarget;PositionFinal");
			writer.Close();
    	}
    	else
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine(blockID + ";" + trialID + ";" + configID + ";" + positionTarget + ";" + positionFinal);
			writer.Close();
    	}
    }
}
