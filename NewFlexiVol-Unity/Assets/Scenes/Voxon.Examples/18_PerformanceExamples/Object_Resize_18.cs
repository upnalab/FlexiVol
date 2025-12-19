using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Resize_18 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Voxon.Input.GetKey("Increase"))
        {
            gameObject.transform.localScale *= 1.1f;
        }
        else if (Voxon.Input.GetKey("Decrease"))
        {
            gameObject.transform.localScale *= 0.9f;
        }
    }
}
