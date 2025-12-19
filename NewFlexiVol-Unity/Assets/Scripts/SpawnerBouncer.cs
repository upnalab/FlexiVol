using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBouncer : MonoBehaviour
{
    private GameObject voxonSpace;
    private GameObject objectToLoad;
    public GameObject primitiveToInstantiate;

    // Start is called before the first frame update
    void Start()
    {
        voxonSpace = GameObject.Find("constrained_size");

    }

    // Update is called once per frame
    void Update()
    {
        if(Voxon.Input.GetKey("Space"))
        {

            objectToLoad = (GameObject)Instantiate(primitiveToInstantiate, voxonSpace.transform.parent.transform);
            float randomPosX = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.x - voxonSpace.transform.localScale.x/2), (float)(voxonSpace.transform.parent.transform.position.x + voxonSpace.transform.localScale.x/2));
            float randomPosY = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.y - voxonSpace.transform.localScale.y/2), (float)(voxonSpace.transform.parent.transform.position.y + voxonSpace.transform.localScale.y/2));
            float randomPosZ = UnityEngine.Random.Range((float)(voxonSpace.transform.parent.transform.position.z - voxonSpace.transform.localScale.z/2), (float)(voxonSpace.transform.parent.transform.position.z + voxonSpace.transform.localScale.z/2));

            objectToLoad.transform.position = new Vector3(randomPosX, randomPosY, randomPosZ);
            objectToLoad.gameObject.tag = "InteractiveObject";
            objectToLoad.AddComponent<Rigidbody>();
     

        }

        if(Voxon.Input.GetKey("GoBack"))
        {
            if(GameObject.FindGameObjectWithTag("InteractiveObject") != null)
            {
                Destroy(GameObject.FindGameObjectWithTag("InteractiveObject"));
            }    

        }




    }
}
