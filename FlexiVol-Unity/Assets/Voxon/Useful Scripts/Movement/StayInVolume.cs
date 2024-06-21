using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 *  VX script to confine GameObjects within the volume. Assumes the position of the object is 1:1 within the volume 
 * 
 *  15/2/2023 - Matthew Vecchio for Voxon
 * 
 */ 



public class StayInVolume : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = transform.position;
        // clap position to be in the display....

        float[] aspf = Voxon.VXProcess.Runtime.GetAspectRatio();
        
        Vector3 asp = new Vector3(aspf[0], aspf[1], aspf[2]);

        if (pos.x > asp[0]) pos.x = asp[0] - 0.01f;
        if (pos.y > asp[1]) pos.y = asp[1] - 0.01f;
        if (pos.z > asp[2]) pos.z = asp[2] - 0.01f;
        if (pos.x < -asp[0]) pos.x = -asp[0] + 0.01f;
        if (pos.y < -asp[1]) pos.y = -asp[1] + 0.01f;
        if (pos.z < -asp[2]) pos.z = -asp[2] + 0.01f;

        transform.position = pos;
    }
}
