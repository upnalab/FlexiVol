using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMe : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Voxon.Input.GetKey("RotateUp"))
        {
        	gameObject.transform.Rotate(new Vector3(0,0.5f,0));
        }
        if(Voxon.Input.GetKey("RotateDown"))
        {
        	gameObject.transform.Rotate(new Vector3(0,-0.5f,0));
        }
        if(Voxon.Input.GetKey("RotateSideL"))
        {
        	gameObject.transform.Rotate(new Vector3(0.5f,0,0));
        }
        if(Voxon.Input.GetKey("RotateSideR"))
        {
        	gameObject.transform.Rotate(new Vector3(-0.5f,0,0));
        }

        if(Voxon.Input.GetKey("Left"))
        {
        	gameObject.transform.Translate(new Vector3(0.05f,0,0));
        }
        if(Voxon.Input.GetKey("Right"))
        {
        	gameObject.transform.Translate(new Vector3(-0.05f,0,0));
        }
        if(Voxon.Input.GetKey("Up"))
        {
        	gameObject.transform.Rotate(new Vector3(0,0,0.5f));
        }
        if(Voxon.Input.GetKey("Down"))
        {
        	gameObject.transform.Rotate(new Vector3(0,0,-0.5f));
        }
    }
}
