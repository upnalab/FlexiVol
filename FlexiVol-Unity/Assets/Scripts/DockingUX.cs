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
	public bool realGame;
	public string userName;
	public bool interactWithFinger;
	public bool rightHanded;
	public GameObject primitiveToInstantiate, phantomToInstantiate;
	public bool randomizeConditions;
	public int nbBlocMax, trialNumber;

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
 	public enum task{
	    TRANSLATION,
	    ROTATION,
	    TRANSLATIONROTATION
 	}

	[Serializable]
	public struct TasksStruct{
		public string nameTask;
		public string direction;
	}

	private TasksStruct[] tasks;
	public Vector3[] sizes;

	public TasksStruct taskToDo;
	public Vector3 sizeObject;

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
        	if(rightHanded)
        	{
	        	interactiveObject = GameObject.Find("RightIndex");
	        	GameObject.Find("LeftIndex").SetActive(false);
        	}
        	else
        	{
	        	interactiveObject = GameObject.Find("LeftIndex");
	        	GameObject.Find("RightIndex").SetActive(false);
        	}
        }
        else
        {
        	GameObject.Find("HandsUpdate").SetActive(false);
        }

        // interactiveObject.tag = "InteractiveObject";
        // interactiveObject.AddComponent<Rigidbody>();
        // interactiveObject.GetComponent<Rigidbody>().isKinematic = true;
        // interactiveObject.GetComponent<Rigidbody>().useGravity = false;
        // panelStart = GameObject.Find("PanelStart");

        tasks = new TasksStruct[15];
        for(int i = 0; i < tasks.Length; i++)
        {
        	if(i < Enum.GetNames(typeof(directionTask)).Length)
        	{
        		tasks[i].nameTask = Enum.GetName(typeof(task), 0);
        		tasks[i].direction = Enum.GetName(typeof(directionTask), i);
        	}
        	if(i == Enum.GetNames(typeof(directionTask)).Length)
        	{
        		tasks[i].nameTask = Enum.GetName(typeof(task), 1);
        		tasks[i].direction = Enum.GetName(typeof(directionTask), 0);
        	}
        	if(i > Enum.GetNames(typeof(directionTask)).Length)
        	{
        		tasks[i].nameTask = Enum.GetName(typeof(task), 2);
        		tasks[i].direction = Enum.GetName(typeof(directionTask), (i-1)%(Enum.GetNames(typeof(directionTask)).Length));
        	}
        	
        }

        voxonSpace = GameObject.Find("constrained_size");
        RecordPerformance();
        RecordCubePositions();
        finalPositionsCubes = new Vector3[nbBlocMax*sizes.Length*tasks.Length];
        targetPositionsCubes = new Vector3[nbBlocMax*sizes.Length*tasks.Length];

    }

    // Update is called once per frame
    void Update()
    {
        for(int l = 0; l < sizes.Length; l++)
        {
        	if((config < tasks.Length*(l+1)) && (config >= tasks.Length*l))
        	{
        		sizeObject = sizes[l];
        		taskToDo = tasks[config % tasks.Length];
        	}
        }

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
	    		// if(interactiveObject.GetComponent<VXComponent>() != null)
		    	// {
		     //    	Destroy(interactiveObject.GetComponent<VXComponent>());
		    	// }
    			// HERE PRESENTS PANEL START
    			// for(int i = 0; i < panelStart.transform.childCount; i++)
    			// {
	    		// 	if(panelStart.transform.GetChild(i).GetComponent<VXComponent>() != null)
			    // 	{
			    //     	Destroy(panelStart.transform.GetChild(i).GetComponent<VXComponent>());
			    // 	}
    			// 	if(panelStart.transform.GetChild(i).GetComponent<CollideAndDisappear>().isTouched)
    			// 	{
    			// 		panelStart.SetActive(false);
    			// 		state = -1;
    			// 	}
    			// }

    			if(Voxon.Input.GetKeyDown("Space"))
    			{
					// panelStart.SetActive(false);
    				state = -1;
    			}
    			break;

    		case -1:
    			if(randomizeConditions)
				{
					config = UnityEngine.Random.Range(0, sizes.Length*tasks.Length);
					while(configException.Contains(config))
					{
						config = UnityEngine.Random.Range(0, sizes.Length*tasks.Length);
					}
				}
				else
				{
					config = configException.Count + 1;
				}

				state = 0;
				break;

			case 0:
				configException.Add(config);
				if(Voxon.Input.GetKeyDown("Space"))
				{
					state = -1;
				}
				break;

    // 		case 0:
				// configException.Add(config);

    // 			// LOAD CONDITIONS
    // 			objectToLoad = (GameObject)Instantiate(primitiveToInstantiate, voxonSpace.transform.parent.transform);
    // 			objectToLoad.transform.localScale = sizes[config];
    // 			float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
    // 			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
    // 			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

    // 			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
    // 			// Debug.Log(objectToLoad.transform.localScale.x*10 + interactiveObject.transform.localScale.x*10);
    // 			// Debug.Log("distance: " + Vector3.Distance(objectToLoad.transform.position, interactiveObject.transform.position));
    // 			while(Vector3.Distance(objectToLoad.transform.position, interactiveObject.transform.position) < 10*(objectToLoad.transform.localScale.x + interactiveObject.transform.localScale.x + 0.01f))
    // 			{
				// 	randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
    // 				randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
    // 				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
	   //  			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
	   //  			// Debug.Log("came here");
    // 			}
    // 			objectToLoad.transform.eulerAngles = new Vector3(0,0,0);
    // 			objectToLoad.AddComponent<VXDynamicComponent>();
    // 			if(interactiveObject.GetComponent<VXComponent>() != null)
		  //   	{
		  //       	Destroy(interactiveObject.GetComponent<VXComponent>());
		  //   	}
    	
    // 			objectToLoad.AddComponent<CollideAndDisappear>();
    // 			// Debug.Log(Application.dataPath);
    // 			// Debug.Log(voxonSpace.name);
    // 			positionsSpheres[trialNumber] = objectToLoad.transform.position;
    // 			objectToLoad.AddComponent<CorrectionMesh>();
    // 			startStopWatchTime = Time.time;
    // 			stopWatch = 0;
    // 			state = 1;
    // 			// update countDown = 0
    // 			// instantiate new sphere, condition i, position xxx

    // 			break;

    // 		case 1:
    // 			stopWatch = Time.time - startStopWatchTime;
    // 			if(objectToLoad.GetComponent<CollideAndDisappear>().isTouched)
    // 			{
    // 				state = 2;
    // 			}

    // 			// StartCoroutine(WaitForCollision());
    // 			// in coroutine -> record time
    // 			break;

    // 		case 2:
		  //   	Destroy(objectToLoad);
		  //   	RecordPerformance(nbBloc, trialNumber, config, stopWatch);
		  //   	RecordSpherePosition(nbBloc, trialNumber, config, positionsSpheres[trialNumber]);
		  //   	trialNumber = trialNumber + 1;
    // 			if(configException.Count >= sizes.Length)
				// {
				// 	nbBloc = nbBloc + 1;
				// 	if(nbBloc >= nbBlocMax)
				// 	{
				// 		Application.Quit();
				// 		Debug.Break();
				// 	}
				// 	else
				// 	{
				// 		startOver = true;
				// 	}
				// }
				// else
				// {
				// 	state = -1;
				// }

				// if(startOver)
				// {
				// 	configException.Clear();
				// 	configException = new List<int>();
				// 	startOver = false;
				// 	state = -1;
				// }

    // 			// Destroy sphere
    // 			// if nbBloc == nbBlocMax, trial=trialMax - Finish
    // 			// else go back to 0
    // 			break;

    	}

    
    }

    void RecordPerformance(int blockID = 0, int trialID = 0, int configID = 0, float clock = 0)
    {
		if(interactWithFinger)
    	{
    		if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/DockingUX/Fingers/" + userName + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/Fingers/" + time0 + ".csv";
    		}

    	}
    	else
    	{
	    	if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/DockingUX/3DMouse/" + userName + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/3DMouse/" + time0 + ".csv";
    		}
    	}
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
    	if(interactWithFinger)
    	{
    		if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/DockingUX/Fingers/" + userName + "-" + time0 + "-positions.csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/Fingers/" + time0 + "-positions.csv";
    		}

    	}
    	else
    	{
	    	if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/DockingUX/3DMouse/" + userName + "-" + time0 + "-positions.csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/3DMouse/" + time0 + "-positions.csv";
    		}
    	}

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
