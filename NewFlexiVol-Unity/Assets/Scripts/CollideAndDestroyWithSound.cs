using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Voxon;

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
    public Vector3 originalScale;
    // Start is called before the first frame update
    // void Awake()
    // {
    // 	this.GetComponent<AudioSource>().playOnAwake = false;

    // }
    void Start()
    {
        originalScale = this.transform.localScale;
        this.GetComponent<Collider>().isTrigger = true;
        this.isTouched = false;
    	audioData = this.GetComponent<AudioSource>();
        audioData.playOnAwake = false;
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTouched && !soundPlay)
        {
            StartCoroutine(SoundAndDestroy());
            this.isTouched = false;
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
    		this.GetComponent<Renderer>().enabled = false;
    	}
    	yield return new WaitForSeconds(0.5f);
        Vector3 newPosition = new Vector3(this.transform.position.x + Random.Range(-2.0f, 2.0f), this.transform.position.y, this.transform.position.z + Random.Range(-2.0f, 2.0f));
        GameObject newObject = (GameObject)Instantiate(this.gameObject, newPosition, Quaternion.identity, this.transform.parent.gameObject.transform);
        newObject.transform.localScale = originalScale;
        Destroy(this.gameObject);
    }
}
