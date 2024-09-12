using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Globalization;
using System.Linq;

public class PathTracingSim : MonoBehaviour
{

    public bool writeBool;
    public GameObject sphere;
    public string condition;
    public GameObject interactiveObject;
    public GameObject[] circuits;
    public Vector3[] startingPositions;

    public string userID;

    private string pathRead;
    private string[] dataSplit, splitSentence, splitFirst;

    private int config;

    public float stopWatch;
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

    public int frame;

    private float frame_max;
    public string timeRead;

    public Vector3 closeAgain, closest_distance;
    public Vector3[] closestPoint, closestDist;

    public float[] REAL_MIN_DISTANCE;
    public float trueMin;


    // Start is called before the first frame update
    void Start()
    {
        
        circuits = new GameObject[GameObject.Find("Primitives").transform.childCount];
        startingPositions = new Vector3[GameObject.Find("Primitives").transform.childCount];
        for(int i = 0; i < circuits.Length; i++)
        {
            circuits[i] = GameObject.Find("Primitives").transform.GetChild(i).gameObject;
            startingPositions[i] = circuits[i].transform.GetChild(0).transform.position;
        }

        interactiveObject = GameObject.Find("InteractableSphere");

        state = -2;

        if(writeBool)
        {
            RecordDistancesToCircuit();
        }

        state = -1;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Time.fixedDeltaTime = 0.05f;

        switch(state)
        {
            case -1:

                pathRead = "Assets/Resources/CleanedCircuits_"+ config + "-" + condition + ".csv";
                StreamReader sr = new StreamReader(pathRead, true);
                if (sr.Peek() > -1) 
                {
                    string line = sr.ReadToEnd(); 
                    dataSplit = line.Split('\n');
                }
                frame_max = (dataSplit.Length - 1);

                for(int i = 0; i < circuits.Length; i++)
                {
                    if(i != config)
                    {
                        circuits[i].SetActive(false);
                    }
                }

                circuits[config].SetActive(true);
                state = 0;


                break;
            case 0:                
                Debug.Log("Frame: " + frame + " / " + frame_max);

                frame = frame + 1;

                if(frame >= frame_max)
                {
                    Debug.Log("FRAMEMAX");
                    state = 1;
                }

                splitSentence = dataSplit[frame].Split(';');
                timeRead = splitSentence[4];
                interactiveObject.transform.position = new Vector3(float.Parse((splitSentence[5]), CultureInfo.InvariantCulture)/10000, float.Parse((splitSentence[6]), CultureInfo.InvariantCulture)/10000, float.Parse((splitSentence[7]), CultureInfo.InvariantCulture)/10000);
                userID = splitSentence[8];
                
                closestPoint = new Vector3[circuits[config].transform.childCount-1];
                closestDist = new Vector3[circuits[config].transform.childCount-1];

                REAL_MIN_DISTANCE = new float[circuits[config].transform.childCount-1];
                
                for(int i = 0; i < circuits[config].transform.childCount-1; i++)
                {
                    closestPoint[i] = circuits[config].gameObject.transform.GetChild(i+1).GetComponent<Collider>().ClosestPointOnBounds(interactiveObject.transform.position);
                    closestDist[i] = interactiveObject.gameObject.GetComponent<Collider>().ClosestPoint(closestPoint[i]);
                    REAL_MIN_DISTANCE[i] = Vector3.Distance(closestPoint[i], interactiveObject.transform.position);
                }

                trueMin = REAL_MIN_DISTANCE.Min();
                
                for(int i = 0; i < circuits[config].transform.childCount-1; i++)
                {
                    if(trueMin == REAL_MIN_DISTANCE[i])
                    {
                        closest_distance = closestPoint[i];
                    }
                }

                if(trueMin < interactiveObject.transform.localScale.x/2)
                {
                    trueMin = 0;
                }

                sphere.transform.position = closest_distance;

                if(writeBool)
                {
                    // write! user, frame, time, trueMin
                    RecordDistancesToCircuit(frame, timeRead, userID, condition, config, trueMin);
                }

                break;

            case 1:
                config = config + 1;
                if(config > 4)
                {
                    Debug.Break();
                }
                else
                {
                    frame = 0;
                    state = -1;
                }

                break;
        }
    }

    void RecordDistancesToCircuit(int frameID = 0, string clock = "", string user = "", string cond = "", int config = 0, float distance = 0)
    {
        path = "Assets/Resources/DataCollection/CleanedCircuitsDistances-" + condition + ".csv";

        if(state == -2)
        {
            writer = new StreamWriter(path, true);
            writer.WriteLine("Frame;Time;User;Condition;Config;Distance");
            writer.Close();
        }
        else
        {
            writer = new StreamWriter(path, true);
            writer.WriteLine(frameID + ";" + clock + ";" + userID + ";" + condition + ";" + config + ";" + distance.ToString("F4"));
            writer.Close();
        }
    }

    
}
