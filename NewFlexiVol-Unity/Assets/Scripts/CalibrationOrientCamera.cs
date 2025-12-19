using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class CalibrationOrientCamera : MonoBehaviour
{
	public Vector3 calib0, calib1, calib2;
	public Plane plane;

    public float xLeftIndex, yLeftIndex, zLeftIndex = 0;
    public float xLeftThumb, yLeftThumb, zLeftThumb = 0;

    public float xRightIndex, yRightIndex, zRightIndex = 0;
    public float xRightThumb, yRightThumb, zRightThumb = 0;

    public HandData handData;
    // Start is called before the first frame update
    void Start()
    {
        handData = GameObject.FindObjectOfType<HandData>();
    }

    // Update is called once per frame
    void Update()
    {
    	xLeftIndex = handData.xLeftIndex;
    	yLeftIndex = handData.yLeftIndex;
    	zLeftIndex = handData.zLeftIndex;

    	xRightIndex = handData.xRightIndex;
    	yRightIndex = handData.yRightIndex;
    	zRightIndex = handData.zRightIndex;

    	xLeftThumb = handData.xLeftThumb;
    	yLeftThumb = handData.yLeftThumb;
    	zLeftThumb = handData.zLeftThumb;

    	xRightThumb = handData.xRightThumb;
    	yRightThumb = handData.yRightThumb;
    	zRightThumb = handData.zRightThumb;

        if(Voxon.Input.GetKeyDown("Calib1"))
        {
            if(!GameObject.Find("GameManager").GetComponent<SelectionUX>().realGame)
            {
                calib0 = GameObject.Find("Sphere").transform.position;
            }
            else
            {
                 if(GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded)
                {
                    calib0 = new Vector3(xRightIndex, yRightIndex, zRightIndex);
                }
                else
                {
                    calib0 = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
                }
            }
           
            // calib0 = GameObject.Find("Sphere").transform.position;
        }
        if(Voxon.Input.GetKeyDown("Calib2"))
        {
            if(!GameObject.Find("GameManager").GetComponent<SelectionUX>().realGame)
            {
                calib1 = GameObject.Find("Sphere (1)").transform.position;
            }
            else
            {
                if(GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded)
                {
                    calib1 = new Vector3(xRightIndex, yRightIndex, zRightIndex);
                }
                else
                {
                    calib1 = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
                }
            }
            // calib1 = GameObject.Find("Sphere (1)").transform.position;
        }
        if(Voxon.Input.GetKeyDown("Calib3"))
        {
            if(!GameObject.Find("GameManager").GetComponent<SelectionUX>().realGame)
            {
                calib2 = GameObject.Find("Sphere (2)").transform.position;
            }
            else
            {
                if(GameObject.Find("GameManager").GetComponent<SelectionUX>().rightHanded)
                {
                    calib2 = new Vector3(xRightIndex, yRightIndex, zRightIndex);
                }
                else
                {
                    calib2 = new Vector3(xLeftIndex, yLeftIndex, zLeftIndex);
                }
            }

            // calib2 = GameObject.Find("Sphere (2)").transform.position;
        }
        if(Voxon.Input.GetKeyDown("CalibFull"))
        {
            plane.Set3Points(calib0, calib1, calib2);
            this.transform.parent.transform.up = -plane.normal;
            this.transform.parent.transform.position = new Vector3(calib0.x + (calib1.x - calib0.x)/2, (calib0.y+calib1.y+calib2.y)/3 - 2, calib0.z - (calib0.z - calib2.z)/2);
            // planeVisual.transform.up = -plane.normal;
            // planeVisual.transform.position = new Vector3(calib0.x + (calib1.x - calib0.x)/2, (calib0.y+calib1.y+calib2.y)/3 - 4, calib0.z - (calib0.z - calib2.z)/2);
        }
    }
}
