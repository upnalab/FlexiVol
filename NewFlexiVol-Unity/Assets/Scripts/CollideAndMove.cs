using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideAndMove : MonoBehaviour
{
	public bool availableToCollide;

	private Vector3 originalPosPlane;
	private Vector3 originalFingerPos;
	private GameObject objectOfInterest;
	public bool index, middle;
    public string previous;
    public float scale = 0.005f;
    public float threshold = 0.005f;
    private string nextOne;

    private Vector3 normA, normB, bisector, midPoint;
    public int countSwitch;
    private int newCountSwitch;

    private GameObject indexObject, middleObject, palmObject, thumbObject, middleTip, indexTip;
    private int state;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshCollider>().convex = true;
        this.GetComponent<MeshCollider>().isTrigger = true;
        availableToCollide = true;

        if(this.gameObject.GetComponent<Rigidbody>() == null)
    	{
        	this.gameObject.AddComponent<Rigidbody>();
        	this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        	this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
    	}
        state = 1;

    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case 1:
                    indexObject = GameObject.Find("1_index_first");
                    middleObject = GameObject.Find("1_middle_first");
                    thumbObject = GameObject.Find("1_thumb_first");
                    palmObject = GameObject.Find("1_palm_base");
                    indexTip = GameObject.Find("1_index_tip");
                    middleTip = GameObject.Find("1_middle_tip");
                    state = 2;
                    StartCoroutine(CountSwitches());
                    break;

                case 2:

                    RunTheRest();
                    break;
        }
        
    }

    void RunTheRest()
    {
        if(!index && !middle)
        {
            availableToCollide = true;
        }
        else
        {
            availableToCollide = false;
        }

        normA = (indexObject.transform.position - thumbObject.transform.position).normalized;
        normB = (middleObject.transform.position - palmObject.transform.position).normalized;

        midPoint = indexObject.transform.position - (indexObject.transform.position - middleObject.transform.position)/2;
        bisector = (normA + normB).normalized;

            // By debugging values, we find that a dot of 2 in a good threshold to detect switches
        // Debug.Log(Vector3.Dot(Vector3.ProjectOnPlane(indexTip.transform.position, this.gameObject.transform.up) - Vector3.ProjectOnPlane(middleTip.transform.position, this.gameObject.transform.up), bisector));
        if(Vector3.Dot(Vector3.ProjectOnPlane(indexTip.transform.position, this.gameObject.transform.up) - Vector3.ProjectOnPlane(middleTip.transform.position, this.gameObject.transform.up), bisector) > 2)
        {
            countSwitch = newCountSwitch + 1;
        }
        else
        {
            newCountSwitch = countSwitch + 1;
        }

    }

    void OnTriggerEnter(Collider other)
    {    	
        if(other.GetComponent<Collider>().name == "1_index_tip")
    	{
    		index = true;
            if(!middle)
            {
                // originalPosPlane = this.transform.position;
                originalFingerPos = other.transform.gameObject.transform.localPosition;
                previous = "1_index_tip";
                StartCoroutine("SwitchFinger", other.transform.gameObject);
            }
            
    	}

    	if(other.GetComponent<Collider>().name == "1_middle_tip")
    	{
    		middle = true;
            if(!index)
            {
                // originalPosPlane = this.transform.position;
                originalFingerPos = other.transform.gameObject.transform.localPosition;
                previous = "1_middle_tip";
    	        StartCoroutine("SwitchFinger", other.transform.gameObject);
            }
    	}

    }

    void OnTriggerStay(Collider other)
    {
    	if(previous == "1_index_tip")
    	{
	    	objectOfInterest = indexTip;
    	}
    	if(previous == "1_middle_tip")
    	{
	    	objectOfInterest = middleTip;

    	}
        if(!availableToCollide && (countSwitch > 3))
        {
            float orientation = -1; //Mathf.Sign(Vector3.Dot((originalFingerPos - objectOfInterest.transform.localPosition), bisector));
            Debug.Log(orientation);
            Vector3 movement = bisector*orientation*scale;
            
            if(Mathf.Sqrt(Mathf.Pow(objectOfInterest.transform.localPosition.x - originalFingerPos.x, 2) + Mathf.Pow(objectOfInterest.transform.localPosition.z - originalFingerPos.z, 2)) > threshold)
            {
                this.transform.position = new Vector3(this.transform.position.x + movement.x, this.transform.position.y, this.transform.position.z + movement.z);

            }
        }   	

    }


    void OnTriggerExit(Collider other)
    {
    	if((other.GetComponent<Collider>().name == "1_middle_tip") && (middle == true))
    	{
    		middle = false;
    	}
    	if((other.GetComponent<Collider>().name == "1_index_tip") && (index == true))
    	{
    		index = false;
    	}
    	
    }

    IEnumerator SwitchFinger(GameObject collidingObject)
    {
    	// Limit the time it can slide with the one finger
        yield return new WaitForSeconds(0.1f);
        if(!availableToCollide)
        {
            if(previous == "1_index_tip")
            {
                if(middle)
                {
                    nextOne = "1_middle_tip";
                }
            }

            if(previous == "1_middle_tip")
            {
                if(index)
                {
                    nextOne = "1_index_tip";
                }
            }
            // originalPosPlane = this.transform.position;
            originalFingerPos = GameObject.Find(nextOne).transform.localPosition;
            StartCoroutine("SwitchFinger", GameObject.Find(nextOne));
            previous = nextOne;
        }

    }

    IEnumerator CountSwitches()
    {
        
        yield return new WaitForSeconds(1.5f);
        scale = threshold*countSwitch;
        countSwitch = 0;

        StartCoroutine(CountSwitches());
    }

    void OnDrawGizmos()
    {
        if(palmObject != null)
        {

            // Vector3 normA = (indexObject.transform.position - thumbObject.transform.position).normalized;
            // Vector3 normB = (middleObject.transform.position - GameObject.Find("palm_base").transform.position).normalized;

            // Vector3 midPoint = indexObject.transform.position - (indexObject.transform.position - middleObject.transform.position)/2;
            // Vector3 bisector = (normA + normB).normalized;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(midPoint, midPoint + bisector*5);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.ProjectOnPlane(midPoint, this.gameObject.transform.up), Vector3.ProjectOnPlane(midPoint, this.gameObject.transform.up) + bisector*5);
            // Debug.DrawRay(indexObject.transform.position, normalToPhalange, Color.blue);
        }
    }
}
