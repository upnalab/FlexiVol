namespace OpenCvSharp.Demo
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
	// public Material[] changedMaterials;
	// public bytes[] imageAssets;
	public int numberMaterial;
	// private Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        // tex = new Texture2D(256, 256);

    }

    // Update is called once per frame
    void Update()
    {
        // tex = new Texture2D(2, 2);

        // numberMaterial = (GameObject.FindObjectOfType<GenerateBitPlanes>().numberToRun) % 2;
        numberMaterial = (GameObject.FindObjectOfType<GenerateBitPlanes>().numberToRun) % 2;
        // bytes[] imageAssets = System.IO.File.ReadAllBytes("./Resources/bitPlanedImage"+numberMaterial+".png");
        // tex.LoadImage(imageAssets);
        // tex.Apply();
        // this.GetComponent<Renderer>().material = Resources.Load("Materials/bitPlanedImage"+numberMaterial) as Material;
        // this.GetComponent<Renderer>().material.mainTexture = tex;

        // changedMaterials[numberMaterial] = Resources.Load("Materials/bitPlanedImage"+numberMaterial) as Material;
        this.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");

        // this.GetComponent<MeshRenderer>().material = changedMaterials[numberMaterial];
		this.GetComponent<MeshRenderer>().material = Resources.Load("Materials/bitPlanedImage"+numberMaterial) as Material;
    }
}
}