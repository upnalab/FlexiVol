
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEditor;

public class ChangeTexture : MonoBehaviour
{
	public string m_MainTexture;
	public GenerateBitPlanes gameManager;
    // Start is called before the first frame update
    void Start()
    {
    	gameManager = GameObject.FindObjectOfType<GenerateBitPlanes>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
    	// gameManager = GameObject.FindObjectOfType<GenerateBitPlanes>();

        
    	// this.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");
		

		// this.GetComponent<MeshRenderer>().material = Resources.Load("./Assets/Shaders/Materials/Materials/ReconstructedImage") as Material;
    	// this.GetComponent<Renderer>().material.SetTexture("./Assets/Shaders/Materials/ReconstructedImage.bmp", m_MainTexture);
    	// this.GetComponent<Renderer>().material = AsyncImage.GenerateTexture("./Assets/Shaders/Materials/ReconstructedImage.bmp");
    	// GenerateTexture
    	// this.GetComponent<Renderer>().material.SetTexture(m_MainTexture, LoadImage("./Assets/Shaders/Materials/ReconstructedImage.bmp"));
		// StartCoroutine(UpdateFkinMat());
		
		if(gameManager.newPatternUpload)
		{
			// Texture2D newMat = Instantiate(LoadImage("./Assets/Shaders/Materials/ReconstructedImage.bmp"));
			// this.GetComponent<MeshRenderer>().material = Resources.Load("./Assets/Shaders/Materials/Materials/ReconstructedImage") as Material;
	    	this.GetComponent<Renderer>().material.SetTexture(m_MainTexture, LoadImage("./Assets/Shaders/Materials/ReconstructedImage.bmp"));

			AssetDatabase.Refresh();
			// Destroy(newMat);
		
		}

  //   	bytes[] byteMe = System.IO.File.LoadAllBytes("./Assets/Shaders/Materials/ReconstructedImage.bmp");
		// texture = new Texture2D(2, 2);
		// texture.LoadImage(byteMe);
    }

 //    IEnumerator UpdateFkinMat()
 //    {
 //    	Texture2D newMat = Instantiate(LoadImage("./Assets/Shaders/Materials/ReconstructedImage.bmp"));

 //    	this.GetComponent<Renderer>().material.SetTexture(m_MainTexture, newMat);
	// 	yield return new WaitForEndOfFrame();
	// 	Destroy(newMat);

 //    }


	// public static Texture2D LoadImage(string path)
	// {
	//     if (File.Exists(path))
	//     {
	//         byte[] bytes = File.ReadAllBytes(path);
	//         Texture2D tex = new Texture2D(2, 2);
	//         tex.LoadImage(bytes);

	//         return tex;
	//     }
	//     else
	//     {
	//         return null;
	//     }
	// }


	public static Texture2D LoadImage(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        return tex;
    
    }

}
