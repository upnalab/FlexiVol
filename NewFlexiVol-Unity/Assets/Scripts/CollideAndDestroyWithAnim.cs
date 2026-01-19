using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Voxon;

/* This is the script that makes an object go into flame (child of this object needs to contain particles) */
// [RequireComponent(typeof(VXDynamicComponent))]
// [RequireComponent(typeof(CorrectionMesh))]
// [RequireComponent(typeof(RemoveVXComponent))]
public class CollideAndDestroyWithAnim : MonoBehaviour
{
	public bool isTouched = false;
    private Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Collider>().isTrigger = true;
        this.isTouched = false;
        originalScale = this.transform.localScale;
        // this.gameObject.AddComponent<VXDynamicComponent>();
        // if(this.GetComponent<VXComponent>() != null)
        // {
        //     Destroy(this.GetComponent<VXComponent>());
        // }
        // this.gameObject.AddComponent<CorrectionMesh>();
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
    	this.GetComponent<Renderer>().enabled = false;
    	this.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        Vector3 newPosition = new Vector3(this.transform.localPosition.x + Random.Range(-2.0f, 2.0f), this.transform.localPosition.y, this.transform.localPosition.z + Random.Range(-2.0f, 2.0f));
        GameObject newObject = (GameObject)Instantiate(this.gameObject, newPosition, Quaternion.identity);
        newObject.transform.GetChild(0).gameObject.SetActive(false);
        newObject.transform.localScale = originalScale;
        Destroy(this.gameObject);

    }

}
