using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using Voxon;

public class HandData : MonoBehaviour
{
    // Start is called before the first frame update
    public Plane plane;
    // public GameObject planeVisual;
    //Fingers
    [Header("Indexes")]
    public GameObject leftIndex;
    public GameObject rightIndex;

    [Header("Thumbs")]
    public GameObject leftThumb;
    public GameObject rightThumb;

    [Header("The rest")]
    private Thread receiveThread;
    private UdpClient udpClient;
    public int port = 12345;
    public string receivedMessage;
    public string palabraInicial;

    public float xRightIndex, yRightIndex, zRightIndex = 0;
    public float xRightThumb, yRightThumb, zRightThumb = 0;

    public float xLeftIndex, yLeftIndex, zLeftIndex = 0;
    public float xLeftThumb, yLeftThumb, zLeftThumb = 0;

    public bool leftPinch = false;
    public bool rightPinch = false;

    public GameObject pickedObjectLeft = null;
    public GameObject pickedObjectRight = null;

    private float x, y, z;
    private bool rightHanded;

    // public Vector3 calib0, calib1, calib2;

    void Start()
    {
        if(GameObject.Find("GameManager").GetComponent<SelectionUX>() != null)
        {
            rightHanded = GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded;
        }
        else
        {
            rightHanded = GameObject.Find("GameManager").GetComponent<DockingUX>().rightHanded;
        }
        // planeVisual = GameObject.Find("Plane");

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
            string[] parts = message.Split(':');
            if (parts.Length != 2) throw new Exception("Invalid message format.");

            palabraInicial = parts[0].Trim();
            string[] coords = parts[1].Split(',');
            if (coords.Length != 3) throw new Exception("Invalid coordinates format.");


            x = float.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture);
            y = float.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture);
            z = float.Parse(coords[2], System.Globalization.CultureInfo.InvariantCulture);

            //Y ahora cambiamos la posicion
            switch(palabraInicial)
            {
                case "Index Right":
                    xRightIndex = x;
                    yRightIndex = y;
                    zRightIndex = z;
                    break;
                case "Index Left":
                    xLeftIndex = x;
                    yLeftIndex = y;
                    zLeftIndex = z;
                    break;
                case "Thumb Right":
                    xRightThumb = x;
                    yRightThumb = y;
                    zRightThumb = z;
                    break;
                case "Thumb Left":
                    xLeftThumb = x;
                    yLeftThumb = y;
                    zLeftThumb = z;
                    break;

                default:
                    // Debug.Log("Shit");
                    break;

            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing message: {e}");
        }
    }

    void Update()
    {

        // if(Voxon.Input.GetKeyDown("Calib1"))
        // {
        //     if(!GameObject.Find("GameManager").GetComponent<SelectionUX>().realGame)
        //     {
        //         calib0 = GameObject.Find("Sphere").transform.position;
        //     }
        //     else
        //     {
        //          if(GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded)
        //         {
        //             calib0 = new Vector3(xRightIndex, yRightIndex, zRightIndex);
        //         }
        //         else
        //         {
        //             calib0 = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
        //         }
        //     }
           
        //     // calib0 = GameObject.Find("Sphere").transform.position;
        // }
        // if(Voxon.Input.GetKeyDown("Calib2"))
        // {
        //     if(!GameObject.Find("GameManager").GetComponent<SelectionUX>().realGame)
        //     {
        //         calib1 = GameObject.Find("Sphere (1)").transform.position;
        //     }
        //     else
        //     {
        //         if(GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded)
        //         {
        //             calib1 = new Vector3(xRightIndex, yRightIndex, zRightIndex);
        //         }
        //         else
        //         {
        //             calib1 = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
        //         }
        //     }
        //     // calib1 = GameObject.Find("Sphere (1)").transform.position;
        // }
        // if(Voxon.Input.GetKeyDown("Calib3"))
        // {
        //     if(!GameObject.Find("GameManager").GetComponent<SelectionUX>().realGame)
        //     {
        //         calib2 = GameObject.Find("Sphere (2)").transform.position;
        //     }
        //     else
        //     {
        //         if(GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded)
        //         {
        //             calib2 = new Vector3(xRightIndex, yRightIndex, zRightIndex);
        //         }
        //         else
        //         {
        //             calib2 = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
        //         }
        //     }

        //     // calib2 = GameObject.Find("Sphere (2)").transform.position;
        // }
        // if(Voxon.Input.GetKeyDown("CalibFull"))
        // {
        //     plane.Set3Points(calib0, calib1, calib2);
        //     this.transform.parent.transform.up = -plane.normal;
        //     this.transform.parent.transform.position = new Vector3(calib0.x + (calib1.x - calib0.x)/2, (calib0.y+calib1.y+calib2.y)/3 - 2, calib0.z - (calib0.z - calib2.z)/2);
        //     // planeVisual.transform.up = -plane.normal;
        //     // planeVisual.transform.position = new Vector3(calib0.x + (calib1.x - calib0.x)/2, (calib0.y+calib1.y+calib2.y)/3 - 4, calib0.z - (calib0.z - calib2.z)/2);
        // }

        // Opcional: Aquí puedes hacer algo con los datos recibidos, como actualizar la UI o mover un objeto.
        leftIndex.transform.localPosition = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
        rightIndex.transform.localPosition = new Vector3(xRightIndex, yRightIndex, zRightIndex);

        if(rightHanded)
        {
            if(rightThumb != null)
            {
                rightThumb.transform.localPosition = new Vector3(xRightThumb, yRightThumb, zRightThumb);

                     //Check if right is pinching
                if (AreSpheresColliding(rightIndex.GetComponent<Collider>(), rightThumb.GetComponent<Collider>())) rightPinch = true;
                else rightPinch = false;            
            }
        }
        else
        {
             if(leftThumb != null) // OR?
            {
                leftThumb.transform.localPosition = new Vector3(xLeftThumb, yLeftThumb, zLeftThumb);
                rightThumb.transform.localPosition = new Vector3(xRightThumb, yRightThumb, zRightThumb);

                //Check if left is pinching
                if (AreSpheresColliding(leftIndex.GetComponent<Collider>(), leftThumb.GetComponent<Collider>())) leftPinch = true;
                else leftPinch = false;
            }
        }
        
    }

    bool AreSpheresColliding(Collider collider1, Collider collider2)
    {
        // Calcula la distancia entre los centros de las esferas
        float distance = Vector3.Distance(collider1.transform.position, collider2.transform.position);

        // Calcula la suma de los radios de las esferas
        float sumOfRadii = collider1.transform.lossyScale.x/2 + collider2.transform.lossyScale.x/2;

        // Si la distancia entre los centros es menor o igual a la suma de los radios, las esferas están colisionando
        return distance <= sumOfRadii;
    }

}
