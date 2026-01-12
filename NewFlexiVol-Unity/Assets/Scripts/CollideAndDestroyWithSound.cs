using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

/* This is the script to enable the popping of a gameObject */
// [RequireComponent(typeof(VXDynamicComponent))]
// [RequireComponent(typeof(CorrectionMesh))]
// [RequireComponent(typeof(RemoveVXComponent))]

[RequireComponent(typeof(AudioSource))]
public class CollideAndDestroyWithSound : MonoBehaviour
{
    public bool isTouched = false;
    bool soundPlay = false;
    AudioSource audioData;
    private int state = 0;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Collider>().isTrigger = true;
    	audioData = this.GetComponent<AudioSource>();
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // switch(state)
        // {
        //     case 0:
        //         this.gameObject.AddComponent<VXDynamicComponent>();
        //         if(this.GetComponent<VXComponent>() != null)
        //         {
        //             Destroy(this.GetComponent<VXComponent>());
        //         }
        //         this.gameObject.AddComponent<CorrectionMesh>();
        //         state = 1;
        //         break;
        //     case 1:
        //         break;
        // }
        // if(Voxon.Input.GetKeyDown("Space"))
        // {
        // 	isTouched = true;
        // }
        
        if(isTouched && !soundPlay)
        {
            StartCoroutine(SoundAndDestroy());
        }
    }

	void OnTriggerEnter(Collider other)
    {
    	if(other.GetComponent<Collider>().tag == "InteractiveObject")
    	{
    		this.isTouched = true;
    	}

    }

    IEnumerator SoundAndDestroy()
    {
    	this.transform.localScale = this.transform.localScale*1.05f;
    	yield return new WaitForSeconds(0.2f);
    	if(!soundPlay)
    	{
    		audioData.Play();
    		soundPlay = true;
    		this.GetComponent<VXDynamicComponent>().enabled = false;
    	}
    	yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
