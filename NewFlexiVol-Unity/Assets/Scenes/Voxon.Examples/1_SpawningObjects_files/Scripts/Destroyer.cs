using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

// namespace  Voxon.Examples._1_SpawningObject
// {
    // / <summary>
    // / Class to destroy game objects which trigger collisions with the attached game object
    // / </summary>
public class Destroyer : MonoBehaviour {

    public float deathDelay = 0.25f;
    // / <summary>
    // / Event to trigger destruction of colliding entity
    // / </summary>
    // / <param name="collision"></param>

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Remove(collision.gameObject));
        // Destroy(collision.gameObject);
    }

    IEnumerator<object> Remove(GameObject go)
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(go);
    }
}
// }

