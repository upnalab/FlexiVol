using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMouseGrow : MonoBehaviour
{
    public float scale_factor = 0.000005f;

	public float movement_speed = 1f;
	public float zoom_speed = 1f;
	private float original_size = 0;
	private Vector3 original_pos = Vector3.zero;
	private Quaternion original_rot = Quaternion.identity;

	public float max_scale = 4;
	public float min_scale = 0.0001f;
	public float max_distance = 2;

	VXCamera cam = null;
	// Start is called before the first frame update
	void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		if(cam == null)
		{
			cam = Voxon.VXProcess.Instance.Camera;
			if (cam == null) return;
		}

        if (Voxon.Input.GetSpaceNavButton("LeftButton") && transform.localScale.x < max_scale)
        {
            cam.BaseScale *= (1 + zoom_speed / 10);
		}
        
        if (Voxon.Input.GetSpaceNavButton("RightButton") && transform.localScale.x > min_scale)
        {
			cam.BaseScale *= (1 - zoom_speed / 10);
		}

		if (original_size == 0)
		{
			original_size = cam.BaseScale;
		}

		if (original_pos == Vector3.zero)
		{
			original_pos = transform.position;
		}

		if (original_rot == Quaternion.identity)
		{
			original_rot = Quaternion.identity;
		}

		
		/*
		var rotation = Voxon.VXProcess.Runtime.GetSpaceNavRotation();

		if (rotation != null)
		{
			var v3rot = new Vector3(0, rotation[2] / 70, 0);
			transform.Rotate(v3rot);
		}
		*/

		var position = Voxon.VXProcess.Runtime.GetSpaceNavPosition();
		var v3pos = transform.position;
		if (position != null)
		{
			v3pos.x += movement_speed * (position[0] / 35.0f);
			v3pos.y += movement_speed * (position[2] / 35.0f);
			v3pos.z -= movement_speed * (position[1] / 35.0f);

			float distance = Vector3.Distance(original_pos, v3pos);
			if (distance > max_distance)
			{
				v3pos = Vector3.MoveTowards(v3pos, original_pos, distance - max_distance);
			}
			transform.position = v3pos;
		}

		if (Voxon.Input.GetSpaceNavButton("LeftButton") && Voxon.Input.GetSpaceNavButton("RightButton"))
		{
			transform.position = original_pos;
			// transform.rotation = original_rot;
			cam.BaseScale = original_size;
		}
	}
}
