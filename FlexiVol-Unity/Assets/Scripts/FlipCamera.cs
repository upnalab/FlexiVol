using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCamera : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 mat = this.GetComponent<Camera>().projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(-1,1,1));
        this.GetComponent<Camera>().projectionMatrix = mat;
    }
}
