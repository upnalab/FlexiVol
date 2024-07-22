namespace OpenCvSharp.Demo
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
	public Material[] changedMaterials;
	public int numberMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // numberMaterial = (GameObject.FindObjectOfType<GenerateBitPlanes>().numberToRun) % 2;
        numberMaterial = (GameObject.FindObjectOfType<GenerateBitPlanes>().numberToRun + 1) % 2;
        this.GetComponent<MeshRenderer>().material = changedMaterials[numberMaterial];
    }
}
}