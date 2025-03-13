using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class ScenarioVideoFolder : MonoBehaviour
{
	public GameObject folders;
	public GameObject files;

	private float posXOrigin, newPosX;
	private int state = -1;
	private float scale = 1;
	private Vector3 scaleOrigin;

	public bool lesgoFolders;
    // Start is called before the first frame update
    void Start()
    {
        posXOrigin = folders.transform.localPosition.x;
        newPosX = folders.transform.localPosition.x;
        scaleOrigin = new Vector3(1000, 1000, 1000);
    }

    // Update is called once per frame
    void Update()
    {
    	switch(state)
    	{
    		case -1:
    			files.SetActive(false);
    			if(Voxon.Input.GetKeyDown("Space"))
		        {
		        	lesgoFolders = true;
		        }

		        if(lesgoFolders && newPosX >= -0.8f)
		        {
		        	newPosX = newPosX - 0.05f;
		        }
		        
		        if(newPosX <= -0.8f)
		        {
		        	lesgoFolders = false;
		        	state = 0;
		        }
		        folders.transform.localPosition = new Vector3(newPosX, folders.transform.localPosition.y, folders.transform.localPosition.z);

		        break;

		    case 0:
		    	
		    	if(Voxon.Input.GetKeyDown("Space"))
		        {
		        	lesgoFolders = true;
		        }

		        if(lesgoFolders && newPosX >= -1.5f)
		        {
		        	newPosX = newPosX - 0.05f;
		        }

		        if(newPosX <= -1.5f)
		        {
		        	lesgoFolders = false;
		        	state = 1;
		        }

		        for(int i = 12; i < 21; i++)
		        {
		        	if(folders.transform.GetChild(i).gameObject.GetComponent<CorrectionMesh>() == null)
		        	{
		        		folders.transform.GetChild(i).gameObject.AddComponent<CorrectionMesh>();
		        	}
		        }


		        folders.transform.localPosition = new Vector3(newPosX, folders.transform.localPosition.y, folders.transform.localPosition.z);


    			break;

    		case 1:
    			if(Voxon.Input.GetKeyDown("Space"))
		        {
		        	lesgoFolders = true;
		        }

		        if(lesgoFolders)
		        {
		        	scale = 1.3f*scale;
		        	folders.transform.GetChild(25).gameObject.transform.localScale = scale*scaleOrigin;
		        }

		        for(int i = 21; i < 26; i++)
		        {
		        	if(folders.transform.GetChild(i).gameObject.GetComponent<CorrectionMesh>() == null)
		        	{
		        		folders.transform.GetChild(i).gameObject.AddComponent<CorrectionMesh>();
		        	}
		        }

		        if(scale > 10)
		        {
		        	lesgoFolders = false;
		        	folders.SetActive(false);
		        	state = 2;
		        }


    			break;

    		case 2:
    			files.SetActive(true);

    			if(Voxon.Input.GetKeyDown("Space"))
		        {
		        	state = 3;
		        }
    			break;

    		case 3:
    			files.SetActive(false);
    			folders.SetActive(true);

    			scale = 1;
    			lesgoFolders = false;
	        	folders.transform.GetChild(25).gameObject.transform.localScale = scale*scaleOrigin;
	        	newPosX = posXOrigin;
	        	folders.transform.localPosition = new Vector3(newPosX, folders.transform.localPosition.y, folders.transform.localPosition.z);


    			if(Voxon.Input.GetKeyDown("Space"))
		        {
		        	state = -1;
		        }


    			break;
    	}
        
    }
}
