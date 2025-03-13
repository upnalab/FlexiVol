using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : MonoBehaviour
{
    // Start is called before the first frame update
	public GameObject otherPlane;
	public float deathDelay = 0.25f;
    // / <summary>
    // / </summary>
    // / <param name="collision"></param>

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Remove(collision.gameObject));
        collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        // Destroy(collision.gameObject);
    }

    IEnumerator<object> Remove(GameObject go)
    {
        yield return new WaitForSeconds(deathDelay);
        go.transform.position = new Vector3( go.transform.position.x, otherPlane.transform.position.y+1,  go.transform.position.z);
        go.GetComponent<Rigidbody>().isKinematic = false;
    }
}