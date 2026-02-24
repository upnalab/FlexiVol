using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysPutAPlane : MonoBehaviour
{
	public GameObject terrainToReplicate;
	private Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = terrainToReplicate.transform.position;
        // terrainToReplicate.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindObjectOfType<CollideAndMove>() == null)
        {
        	GameObject newTerrain = Instantiate(terrainToReplicate, originalPos, Quaternion.identity);
        	newTerrain.SetActive(true);
        }
    }
}
