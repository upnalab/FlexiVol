using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Voxon;

public class SelectionUX : MonoBehaviour
{
	public string userName;
	public bool interactWithFinger;
	public GameObject primitiveToInstantiate;
	public bool randomizeConditions;
	public int nbBlocMax,trialNumber;
	public Vector3[] sizes;
	private int config;

	public float stopWatch;
	private float startStopWatchTime;

	private Vector3[] positionsSpheres;
	public List<(int, float)> results;

	private string time0;
	private int state;
	private GameObject panelStart, interactiveObject;

	[Tooltip("List of Past Configurations")]
	public List<int> configException;

	private GameObject voxonSpace;
	private GameObject objectToLoad;

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

    }

    // Update is called once per frame
    void Update()
    {        

// Add the configuration into the list for pseudo-randomisation
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
    			// HERE PRESENTS PANEL START
    			for(int i = 0; i < panelStart.transform.childCount; i++)
    			{
    				if(panelStart.transform.GetChild(i).GetComponent<CollideAndDisappear>().isTouched)
    				{
    					panelStart.SetActive(false);
    					state = -1;
    				}
    			}

    			if(UnityEngine.Input.GetKeyDown(KeyCode.Space))
    			{
					panelStart.SetActive(false);
    				state = -1;
    			}
    			break;

    		case -1:
    			if(randomizeConditions)
				{
					config = UnityEngine.Random.Range(0, sizes.Length);
					while(configException.Contains(config))
					{
						config = UnityEngine.Random.Range(0, sizes.Length);
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

    			// LOAD CONDITIONS
    			objectToLoad = (GameObject)Instantiate(primitiveToInstantiate, voxonSpace.transform.parent.transform);
    			objectToLoad.transform.localScale = sizes[config];
    			float randomPosX = UnityEngine.Random.Range(-(float)voxonSpace.transform.localScale.x*10/2, (float)voxonSpace.transform.localScale.x*10/2);
    			float randomPosY = UnityEngine.Random.Range(-(float)voxonSpace.transform.localScale.y*10/2, (float)voxonSpace.transform.localScale.y*10/2);
    			float randomPosZ = UnityEngine.Random.Range(-(float)voxonSpace.transform.localScale.z*10/2, (float)voxonSpace.transform.localScale.z*10/2);

    			objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
    			objectToLoad.transform.eulerAngles = new Vector3(0,0,0);
    			objectToLoad.AddComponent<VXComponent>();
    			objectToLoad.AddComponent<CollideAndDisappear>();
    			// objectToLoad.AddComponent<CorrectionMesh>();
    			startStopWatchTime = Time.time;
    			stopWatch = 0;
    			state = 1;
    			// update countDown = 0
    			// instantiate new sphere, condition i, position xxx

    			break;

    		case 1:
    			stopWatch = Time.time - startStopWatchTime;
    			if(objectToLoad.GetComponent<CollideAndDisappear>().isTouched)
    			{
    				state = 2;
    			}

    			// StartCoroutine(WaitForCollision());
    			// in coroutine -> record time
    			break;

    		case 2:
		    	Destroy(objectToLoad);
		    	RecordPerformance(nbBloc, trialNumber, config, stopWatch);
		    	trialNumber = trialNumber + 1;
    			if(configException.Count >= sizes.Length)
				{
					nbBloc = nbBloc + 1;
					if(nbBloc >= nbBlocMax)
					{
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

    			// Destroy sphere
    			// if nbBloc == nbBlocMax, trial=trialMax - Finish
    			// else go back to 0
    			break;

    	}

    }


    // IEnumerator WaitForCollision()
    // {
    // 	stopWatch = Time.time - startStopWatchTime;
    // 	yield return new WaitUntil (() => objectToLoad.GetComponent<CollideAndDisappear>().isTouched);
    // 	RecordPerformance(nbBloc, trialNumber, config, stopWatch);
    // 	state = 2;
    // }

    void RecordPerformance(int blockID = 0, int trialID = 0, int configID = 0, float clock = 0)
    {
    	path = "Assets/Resources/DataCollection/SelectionUX/" + userName + "-" + time0 + ".csv";

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
}
