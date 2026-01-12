using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

/* This is the script that makes an object go into flame (child of this object needs to contain particles) */
[RequireComponent(typeof(VXDynamicComponent))]
[RequireComponent(typeof(CorrectionMesh))]
[RequireComponent(typeof(RemoveVXComponent))]
public class CollideAndDestroyWithAnim : MonoBehaviour
{
	public bool isTouched = false;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if(Voxon.Input.GetKeyDown("Space"))
        // {
        // 	isTouched = true;
        // }
        
        if(isTouched)
        {
            StartCoroutine(AnimAndDestroy());
        }
    }

	void OnTriggerEnter(Collider other)
    {
    	if(other.GetComponent<Collider>().tag == "InteractiveObject")
    	{
    		this.isTouched = true;
    	}

    }

    IEnumerator AnimAndDestroy()
    {
    	this.transform.localScale = this.transform.localScale*0.9f;
    	yield return new WaitForSeconds(0.8f);
    	this.GetComponent<VXDynamicComponent>().enabled = false;
    	this.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

}
