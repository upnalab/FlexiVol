using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Controls camera aspect ratio during operation.
 * Can load existing aspect ratio data from local config
 * Alternatively can set own aspect ratio during play
 */

[ExecuteInEditMode]
public class VxViewFinder : MonoBehaviour
{
	Vector3 base_position = Vector3.zero;
	Quaternion base_rotation = Quaternion.identity;
	Vector3 local_scalar = new Vector3(1, 0.4f, 1);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
		if (gameObject.transform.hasChanged)
		{
			transform.localPosition = base_position;
			transform.localRotation = base_rotation;
			transform.localScale = local_scalar;

			gameObject.transform.hasChanged = false;
		}
#endif
	}

	public void SetAspectRatio(Vector3 scalar)
	{
		local_scalar = scalar;
		transform.localScale = scalar;
	}
}
