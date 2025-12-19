using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using Voxon;

public class AllFingerData : MonoBehaviour
{

    [Header("Calibration Sizes")]
    [Tooltip("Here enter the norms of calibration vectors; X;Z;Y as X;Y;Z")]
    public Vector3 normOrigin = new Vector3(1.83f, 1.88f, 1.78f);
    [Tooltip("Here enter the size of the calibration coordinate system you've used")]
    public Vector3 coordPhysical = new Vector3(0.05f, 0.05f, 0.05f);
    [Tooltip("Here enter the size of the environment you want your hands in")]
    public Vector3 realEnv = new Vector3(0.2f, 0.08f, 0.2f);


    //Future fingers
    [Header("Object to instantiate")]
    public GameObject objToInstantiate;


    [Header("The rest")]
    private Thread receiveThread;
    private UdpClient udpClient;
    public int port = 12345;
    public string receivedMessage;

    private float x, y, z;

    private GameObject palmObject;
    private string[] fingerNames;
    private GameObject[] insideFingers;
    private string[] insideNames;
    private Vector3[] fullCoords;

    void Start()
    {
        objToInstantiate.AddComponent<VXDynamicComponent>();
        objToInstantiate.AddComponent<CorrectionMesh>();
        if(objToInstantiate.GetComponent<VXComponent>() != null)
        {
            Destroy(objToInstantiate.GetComponent<VXComponent>());
        }
        fingerNames = new string[]{"thumb", "index", "middle", "ring", "pinky"};
        insideNames = new string[]{"first", "second", "third", "tip"};
        palmObject = (GameObject)Instantiate(objToInstantiate, this.transform);
        palmObject.name = "palm_base";
        palmObject.AddComponent<Rigidbody>();
		palmObject.GetComponent<Rigidbody>().isKinematic = true;
		palmObject.GetComponent<Rigidbody>().useGravity = false;
        insideFingers = new GameObject[20];
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                insideFingers[j + i*4] = Instantiate(objToInstantiate, this.transform);
                insideFingers[j + i*4].name = fingerNames[i] + "_" + insideNames[j];
                insideFingers[j + i*4].AddComponent<Rigidbody>();
				insideFingers[j + i*4].GetComponent<Rigidbody>().isKinematic = true;
				insideFingers[j + i*4].GetComponent<Rigidbody>().useGravity = false;
            }
        }

        fullCoords = new Vector3[21];
        Destroy(objToInstantiate);
        
        StartUDPReceiver();
    }

    void OnApplicationQuit()
    {
        StopUDPReceiver();
    }

    private void StartUDPReceiver()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveTheData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"UDP Server listening on port {port}");
    }

    private void StopUDPReceiver()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
            udpClient.Close();
            Debug.Log("UDP Server stopped.");
        }
    }

    private void ReceiveTheData()
    {
        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                receivedMessage = Encoding.UTF8.GetString(data);

                ParseMessage(receivedMessage);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e}");
        }
    }

    private void ParseMessage(string message)
    {
        try
        {
            string[] parts = message.Split(',');
            // Debug.Log(parts.Length);
            if (parts.Length != 64) throw new Exception("Invalid message format.");

            for(int i = 0; i < 21; i++)
            {
                x = float.Parse(parts[i*3], System.Globalization.CultureInfo.InvariantCulture)/normOrigin.x;
                y = float.Parse(parts[i*3+1], System.Globalization.CultureInfo.InvariantCulture)/normOrigin.y;
                z = float.Parse(parts[i*3+2], System.Globalization.CultureInfo.InvariantCulture)/normOrigin.z;
            
                fullCoords[i] = new Vector3(x*coordPhysical.x/realEnv.x, y*coordPhysical.y/realEnv.y, z*coordPhysical.z/realEnv.z);

            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing message: {e}");
        }
    }

    void Update()
    {

        palmObject.transform.localPosition = fullCoords[0];
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                insideFingers[j + i*4].transform.localPosition = fullCoords[j + i*4 + 1];
            }
        }

    }

    void OnDrawGizmos()
    {
    	if(palmObject != null)
    	{
    		Gizmos.DrawLine(palmObject.transform.position, insideFingers[0*4].transform.position);
	        // Gizmos.DrawLine(palmObject.transform.position, insideFingers[1*4].transform.position);
	        Gizmos.DrawLine(palmObject.transform.position, insideFingers[4*4].transform.position);
	        for(int i = 0; i < 5; i++)
	        {
	            if(i < 4)
	            {
	                Gizmos.DrawLine(insideFingers[i*4].transform.position, insideFingers[(i+1)*4].transform.position);
	            }
	            for(int j = 1; j < 4; j++)
	            {
	                Gizmos.DrawLine(insideFingers[i*4+j-1].transform.position, insideFingers[i*4+j].transform.position);

	            }
	        }
    	}
       
    }
        
   
}
