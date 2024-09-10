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
	public bool sticky;
	public bool rotationActivated;
	public string userName;
	public bool interactWithFinger;
	public bool rightHanded;
	public GameObject primitiveToInstantiate, phantomToInstantiate;
	public bool randomizeConditions;
	public int nbBlocMax, trialNumber;

	public float addAngle, addDistance;

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
	private float[] finalRotationsCubes, targetRotationsCubes;
	public List<(int, float)> results;

	private string time0;
	[HideInInspector]
	public int state;
	private GameObject objectStart;

	[Tooltip("List of Past Configurations")]
	public List<int> configException, allConfigs;

	private GameObject voxonSpace;
	private GameObject objectToLoad, phantomObject;

	private string path;
	private StreamWriter writer;
	private int nbBloc;
	private bool startOver;

	private bool goodDistance = false;
	private bool goodOrientation = false;

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

   //      	GameObject.FindGameObjectWithTag("Index").AddComponent<VXDynamicComponent>();
			// GameObject.FindGameObjectWithTag("Index").AddComponent<CorrectionMesh>();

			// GameObject.FindGameObjectWithTag("Thumb").AddComponent<VXDynamicComponent>();
		 //    GameObject.FindGameObjectWithTag("Thumb").AddComponent<CorrectionMesh>();

        }
        else
        {
        	GameObject.Find("HandsUpdate").SetActive(false);
        }

        if(rotationActivated)
        {
        	tasks = new TasksStruct[15];
        }
        else
        {
        	tasks = new TasksStruct[7];
        }
        
        for(int i = 0; i < tasks.Length; i++)
        {
        	if(i < Enum.GetNames(typeof(directionTask)).Length)
        	{
        		tasks[i].nameTask = Enum.GetName(typeof(task), 0);
        		tasks[i].direction = Enum.GetName(typeof(directionTask), i);
        	}
        	if(rotationActivated)
        	{
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
        }

        objectStart = GameObject.Find("ObjectStart");

        voxonSpace = GameObject.Find("constrained_size");
        RecordPerformance();
        RecordCubePositions();
        finalPositionsCubes = new Vector3[nbBlocMax*sizes.Length*tasks.Length];
        targetPositionsCubes = new Vector3[nbBlocMax*sizes.Length*tasks.Length];

        finalRotationsCubes = new float[nbBlocMax*sizes.Length*tasks.Length];
        targetRotationsCubes = new float[nbBlocMax*sizes.Length*tasks.Length];

    	sizeObject = new Vector3();
    	config = -1;

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
	    		if(objectStart.GetComponent<VXComponent>() != null)
		    	{
		        	Destroy(objectStart.GetComponent<VXComponent>());
		    	}
		    	for(int i = 0; i < objectStart.transform.childCount; i++)
		    	{
		    		if(objectStart.transform.GetChild(i).gameObject.GetComponent<VXComponent>() != null)
			    	{
			        	Destroy(objectStart.transform.GetChild(i).gameObject.GetComponent<VXComponent>());
			        	objectStart.transform.GetChild(i).gameObject.AddComponent<VXDynamicComponent>();
			        	objectStart.transform.GetChild(i).gameObject.AddComponent<CorrectionMesh>();
			    	}
			    	if(objectStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody>() == null)
			    	{
			        	objectStart.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
			        	objectStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody>().isKinematic = true;
			        	objectStart.transform.GetChild(i).gameObject.GetComponent<Rigidbody>().useGravity = false;
			    	}
		    	}

    			if(!interactWithFinger)
    			{
    				if(objectStart.GetComponent<SpaceMouseGrow>() == null)
    				{
    					objectStart.AddComponent<SpaceMouseGrow>();
    				}
    			}
    			else
    			{
    				if(objectStart.GetComponent<ChangeParentsAtCollision>() == null)
    				{
	    				objectStart.AddComponent<ChangeParentsAtCollision>();

    				}
    				if(objectStart.transform.GetChild(0).gameObject.GetComponent<CollisionWithFingers>() == null)
    				{
		    			objectStart.transform.GetChild(0).gameObject.AddComponent<CollisionWithFingers>();
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

    			if(UnityEngine.Input.GetKeyDown(KeyCode.Space))
				{
					objectStart.SetActive(false);
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
					if(configException.Count == 0)
					{
						config = 0;
					}
					else
					{
						config = configException.Count;
					}
				}

				state = 0;
				break;

			case 0:
				configException.Add(config);
				allConfigs.Add(config);
    			// LOAD OBJECT TO MOVE
    			objectToLoad = (GameObject)Instantiate(primitiveToInstantiate, voxonSpace.transform.parent.transform);
    			objectToLoad.transform.localScale = sizeObject;

    			float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
    			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
    			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

    			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
    			objectToLoad.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range((float)0,360),0);

    			phantomObject = (GameObject)Instantiate(phantomToInstantiate, voxonSpace.transform.parent.transform);
    			phantomObject.transform.localScale = sizeObject*1.2f;
    			
    			if(taskToDo.nameTask == "TRANSLATION")
    			{
	    			if(taskToDo.direction == "X")
	    			{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
	    				phantomObject.transform.position = new Vector3(randomPosX, objectToLoad.transform.position.y, objectToLoad.transform.position.z);
	    			}
	    			if(taskToDo.direction == "Y")
	    			{
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
	    				phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, randomPosY, objectToLoad.transform.position.z);
	    			}
	    			if(taskToDo.direction == "Z")
	    			{
	    				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
	    				phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, objectToLoad.transform.position.y, randomPosZ);
	    			}
	    			if(taskToDo.direction == "XY")
    				{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
		    			phantomObject.transform.position = new Vector3(randomPosX, randomPosY, objectToLoad.transform.position.z);
	    			}
	    			if(taskToDo.direction == "XZ")
    				{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
	    				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
		    			phantomObject.transform.position = new Vector3(randomPosX, objectToLoad.transform.position.y, randomPosZ);
	    			}
	    			if(taskToDo.direction == "YZ")
    				{
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
	    				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
		    			phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, randomPosY, randomPosZ);
	    			}
	    			if(taskToDo.direction == "XYZ")
    				{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
		    			randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

		    			phantomObject.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
	    			}

	    			phantomObject.transform.eulerAngles = objectToLoad.transform.eulerAngles;
	    			if(Vector3.Distance(objectToLoad.transform.position, phantomObject.transform.position) < 10*(objectToLoad.transform.localScale.x + phantomObject.transform.localScale.x + 0.01f))
	    			{
	    				StartCoroutine(TryAgain());
	    			}
	    			else
	    			{
	    				goodDistance = true;
	    			}			
    			}

    			if(taskToDo.nameTask == "ROTATION")
    			{
    				phantomObject.transform.position = objectToLoad.transform.position;
	    			phantomObject.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range((float)0,360),0);
	    			if((objectToLoad.transform.eulerAngles.y - phantomObject.transform.eulerAngles.y)%360 < 15)
	    			{
				    	StartCoroutine(TryAgainRot());
	    			}
	    			else
	    			{
	    				goodOrientation = true;
	    			}
		
    			}

    			if(taskToDo.nameTask == "TRANSLATIONROTATION")
    			{
    				if(taskToDo.direction == "X")
	    			{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
	    				phantomObject.transform.position = new Vector3(randomPosX, objectToLoad.transform.position.y, objectToLoad.transform.position.z);
	    			}
	    			if(taskToDo.direction == "Y")
	    			{
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
	    				phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, randomPosY, objectToLoad.transform.position.z);
	    			}
	    			if(taskToDo.direction == "Z")
	    			{
	    				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
	    				phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, objectToLoad.transform.position.y, randomPosZ);
	    			}
	    			if(taskToDo.direction == "XY")
    				{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
		    			phantomObject.transform.position = new Vector3(randomPosX, randomPosY, objectToLoad.transform.position.z);
	    			}
	    			if(taskToDo.direction == "XZ")
    				{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
	    				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
		    			phantomObject.transform.position = new Vector3(randomPosX, objectToLoad.transform.position.y, randomPosZ);
	    			}
	    			if(taskToDo.direction == "YZ")
    				{
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
	    				randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
		    			phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, randomPosY, randomPosZ);
	    			}
	    			if(taskToDo.direction == "XYZ")
    				{
	    				randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
		    			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
		    			randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

		    			phantomObject.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
	    			}

	    			if(Vector3.Distance(objectToLoad.transform.position, phantomObject.transform.position) < 10*(objectToLoad.transform.localScale.x + phantomObject.transform.localScale.x + 0.01f))
	    			{
	    				StartCoroutine(TryAgain());
	    			}
	    			else
	    			{
	    				goodDistance = true;
	    			}	

	    			phantomObject.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range((float)0,360),0);
	    			if((objectToLoad.transform.eulerAngles.y - phantomObject.transform.eulerAngles.y)%360 < 15)
	    			{
	    				StartCoroutine(TryAgainRot());
	    			}
	    			else
	    			{
	    				goodOrientation = true;
	    			}
    			}
    			
    			// Move by pinching or move with mouse?
    			if(interactWithFinger)
    			{
	    			objectToLoad.AddComponent<ChangeParentsAtCollision>();
	    			objectToLoad.transform.GetChild(0).gameObject.AddComponent<CollisionWithFingers>();

    			}
    			else
    			{
    				objectToLoad.AddComponent<SpaceMouseGrow>();
    			}


		    	for(int i = 0; i < objectToLoad.transform.childCount; i++)
		    	{
		    		if(objectToLoad.transform.GetChild(i).gameObject.GetComponent<VXComponent>() != null)
			    	{
			        	Destroy(objectToLoad.transform.GetChild(i).gameObject.GetComponent<VXComponent>());
			    	}
			    	objectToLoad.transform.GetChild(i).gameObject.AddComponent<VXDynamicComponent>();
			    	objectToLoad.transform.GetChild(i).gameObject.AddComponent<CorrectionMesh>();

			    	objectToLoad.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
			    	objectToLoad.transform.GetChild(i).gameObject.GetComponent<Rigidbody>().isKinematic = true;
			    	objectToLoad.transform.GetChild(i).gameObject.GetComponent<Rigidbody>().useGravity = false;

			    	if(phantomObject.transform.GetChild(i).gameObject.GetComponent<VXComponent>() != null)
			    	{
			        	Destroy(phantomObject.transform.GetChild(i).gameObject.GetComponent<VXComponent>());
			    	}
			    	phantomObject.transform.GetChild(i).gameObject.AddComponent<VXDynamicComponent>();
			    	phantomObject.transform.GetChild(i).gameObject.AddComponent<CorrectionMesh>();
		    	}
    			
    			targetPositionsCubes[trialNumber] = phantomObject.transform.position;
    			targetRotationsCubes[trialNumber] = phantomObject.transform.eulerAngles.y;

    			startStopWatchTime = Time.time;
    			addDistance = 0;
    			addAngle = 0;
    			stopWatch = 0;
    			state = 8;

    			break;

    		case 8:
    			if(taskToDo.nameTask == "TRANSLATION")
    			{

    				if(goodDistance)
	    			{
	    				state = 1;
	    			}
    			}
    			if(taskToDo.nameTask == "TRANSLATIONROTATION")
    			{
    				if(goodDistance && goodOrientation)
	    			{
	    				state = 1;
	    			}
    			}
    			if(taskToDo.nameTask == "ROTATION")
    			{
    				if(goodOrientation)
	    			{
	    				state = 1;
	    			}
    			}
    			
    			break;

    		case 1:
    			stopWatch = Time.time - startStopWatchTime;
    			if(UnityEngine.Input.GetKeyDown(KeyCode.Space))
    			{
    				state = 2;
    			}
    			Debug.Log(Vector3.Distance(objectToLoad.transform.position, phantomObject.transform.position));

    			finalPositionsCubes[trialNumber] = objectToLoad.transform.position;
    			finalRotationsCubes[trialNumber] = objectToLoad.transform.eulerAngles.y;

    	// 		if(Voxon.Input.GetKeyDown("GoBack"))
    	// 		{

					// state = -5;
    	// 		}

    			StartCoroutine(ComputeAccumulatedDistances());
    			// StartCoroutine(WaitForCollision());
    			// in coroutine -> record time
    			break;

    		case -5:
    			configException.Remove(config);
				// allConfigs.Remove(allConfigs[allConfigs.Count-1]);
				// allConfigs.Remove(allConfigs[allConfigs.Count-1]);

				config = allConfigs[allConfigs.Count-2];
				goodDistance = false;
	    		goodOrientation = false;
				Destroy(objectToLoad);
	    		Destroy(phantomObject);
    			state = 0;
				break;

    		case 2:
		    	Destroy(objectToLoad);
		    	Destroy(phantomObject);
		    	RecordPerformance(nbBloc, trialNumber, config, stopWatch, addDistance, addAngle);
		    	RecordCubePositions(nbBloc, trialNumber, config, targetPositionsCubes[trialNumber], finalPositionsCubes[trialNumber], targetRotationsCubes[trialNumber], finalRotationsCubes[trialNumber]);
		    	goodDistance = false;
		    	goodOrientation = false;
		    	trialNumber = trialNumber + 1;
    			if(configException.Count >= sizes.Length*tasks.Length)
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

    void RecordPerformance(int blockID = 0, int trialID = 0, int configID = 0, float clock = 0, float addDistance = 0, float addAngle = 0)
    {
		if(interactWithFinger)
    	{
    		if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/DockingUX/Fingers/" + userName + "-" + time0 + ".csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/Fingers/Hand-Docking-Countdown" + time0 + ".csv";
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
		    	path = Application.dataPath + "/3DMouse/3DMouse-Docking-Countdown" + time0 + ".csv";
    		}
    	}
    	if(state == -2)
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine("BlockID;TrialID;Config;StopWatch;AccDistance;AccAngle");
			writer.Close();
    	}
    	else
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine(blockID + ";" + trialID + ";" + configID + ";" + clock.ToString() + ";" + addDistance + ";" + addAngle);
			writer.Close();
    	}
    }

	void RecordCubePositions(int blockID = 0, int trialID = 0, int configID = 0, Vector3 positionTarget = new Vector3(), Vector3 positionFinal = new Vector3(), float rotTarget = 0, float rotFinal = 0)
    {
    	if(interactWithFinger)
    	{
    		if(!realGame)
    		{
		    	path = "Assets/Resources/DataCollection/DockingUX/Fingers/" + userName + "-" + time0 + "-positions.csv";
    		}
    		else
    		{
		    	path = Application.dataPath + "/Fingers/Hand-Docking-Position-" + time0 + ".csv";
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
		    	path = Application.dataPath + "/3DMouse/3DMouse-Docking-Position-" + time0 + ".csv";
    		}
    	}

    	if(state == -2)
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine("BlockID;TrialID;Config;DistanceFinal;PositionTargetX;PositionTargetY;PositionTargetZ;PositionFinalX;PositionFinalY;PositionFinalZ;RotationTarget;RotationFinal");
			writer.Close();
    	}
    	else
    	{
    		writer = new StreamWriter(path, true);
			writer.WriteLine(blockID + ";" + trialID + ";" + configID + ";" + Vector3.Distance(positionTarget, positionFinal).ToString("F4") + ";" + positionTarget.x.ToString("F4") + ";" + positionTarget.y.ToString("F4") + ";" + positionTarget.z.ToString("F4") + ";" + positionFinal.x.ToString("F4") + ";" + positionFinal.y.ToString("F4") + ";" + positionFinal.z.ToString("F4") + ";" + (rotTarget%360) + ";" + (rotFinal%360));
			writer.Close();
    	}
    }
    IEnumerator TryAgain()
	{
		// objectToLoad.GetComponentInChildren<MeshRenderer>().enabled = false;
		// phantomObject.GetComponentInChildren<MeshRenderer>().enabled = false;
		if(taskToDo.direction == "X")
		{
			float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			phantomObject.transform.position = new Vector3(randomPosX, objectToLoad.transform.position.y, objectToLoad.transform.position.z);
		}
		if(taskToDo.direction == "Y")
		{
			float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			phantomObject.transform.position = new Vector3(objectToLoad.transform.position.y, randomPosY, objectToLoad.transform.position.z);
		}
		if(taskToDo.direction == "Z")
		{
			float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
			phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, objectToLoad.transform.position.y, randomPosZ);
		}
		if(taskToDo.direction == "XY")
		{
    		float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			
			randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			phantomObject.transform.position = new Vector3(randomPosX, randomPosY, objectToLoad.transform.position.z);
		}
		if(taskToDo.direction == "XZ")
		{
    		float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			
			randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
			phantomObject.transform.position = new Vector3(randomPosX, objectToLoad.transform.position.y, randomPosZ);
		}
		if(taskToDo.direction == "YZ")
		{
			float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			
			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
			phantomObject.transform.position = new Vector3(objectToLoad.transform.position.x, randomPosY, randomPosZ);
		}
		if(taskToDo.direction == "XYZ")
		{
    		float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));

			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
			
			randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x*10/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x*10/2));
			randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y*10/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y*10/2));
			randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z*10/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z*10/2));
			phantomObject.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
		}

		if(Vector3.Distance(objectToLoad.transform.position, phantomObject.transform.position) < 10*(objectToLoad.transform.localScale.x + phantomObject.transform.localScale.x + 0.01f))
		{
			yield return new WaitForEndOfFrame();
			StartCoroutine(TryAgain());
		}
		else
		{
			goodDistance = true;
		}
		yield return new WaitUntil(() => goodDistance);
		// objectToLoad.GetComponentInChildren<MeshRenderer>().enabled = true;
		// phantomObject.GetComponentInChildren<MeshRenderer>().enabled = true;
		stopWatch = 0;
	}


	IEnumerator TryAgainRot()
	{
		objectToLoad.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range((float)0,360),0);
    	phantomObject.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range((float)0,360),0);
		if((objectToLoad.transform.eulerAngles.y - phantomObject.transform.eulerAngles.y)%360 < 15)
		{
			StartCoroutine(TryAgainRot());
		}
		else
		{
			goodOrientation = true;
		}

		yield return new WaitUntil(() => goodOrientation);

	}

	IEnumerator ComputeAccumulatedDistances()
	{
		Vector3 oldPosition = objectToLoad.transform.position;
		float oldAngle = objectToLoad.transform.eulerAngles.y;
		Vector3 oldOrient = objectToLoad.transform.forward;
		yield return new WaitForEndOfFrame();
		addDistance = addDistance + Vector3.Distance(oldPosition, objectToLoad.transform.position)/10; // 10 represents basescale
		addAngle = addAngle + Vector3.Angle(oldOrient, objectToLoad.transform.forward);//Mathf.Abs((Mathf.Abs(oldAngle) - Mathf.Abs(objectToLoad.transform.eulerAngles.y))%360);
	}
}
