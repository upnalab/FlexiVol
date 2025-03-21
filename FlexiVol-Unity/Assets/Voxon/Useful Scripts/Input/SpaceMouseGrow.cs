﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMouseGrow : MonoBehaviour
{
    public float scale_factor = 0.000005f;

	public float movement_speed = 0.03f;
	public float zoom_speed = 1f;
	private float original_size = 0;
	private Vector3 original_pos = Vector3.zero;
	private Quaternion original_rot = Quaternion.identity;

	public float max_scale = 4;
	public float min_scale = 0.0001f;
	public float max_distance = 2;
		public float scaleMe = 1;
	VXCamera cam = null;

	private Vector3 viewFinderScale;
	private VXCamera _camera;
	private int state;

	private float orX, orY, orZ;

	// Start is called before the first frame update
	void Start()
    {
    	_camera = GameObject.FindObjectOfType<VXCamera>();
		viewFinderScale = GameObject.Find("constrained_size").transform.localScale*_camera.BaseScale;
		// Debug.Log(_camera.BaseScale);
		original_size = this.transform.localScale.x;
		original_rot = this.transform.rotation;
		original_pos = this.transform.localPosition;
		scaleMe = 1;
		orX = this.transform.localScale.x;
		orY = this.transform.localScale.y;
		orZ = this.transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
		if(cam == null)
		{
			cam = Voxon.VXProcess.Instance.Camera;
			if (cam == null) return;
		}

		// if(GameObject.FindObjectOfType<SelectionUX>() != null)
		// {
		// 	state = GameObject.FindObjectOfType<SelectionUX>().state;
		// }
		// if(GameObject.FindObjectOfType<DockingUX>() != null)
		// {
		// 	state = GameObject.FindObjectOfType<DockingUX>().state;
		// }
		// if(GameObject.FindObjectOfType<TracingUX>() != null)
		// {
		// 	state = GameObject.FindObjectOfType<TracingUX>().state;
		// }

        if (Voxon.Input.GetSpaceNavButton("LeftButton"))// && transform.localScale.x < max_scale)
        {
            // cam.BaseScale *= (1 + zoom_speed / 10);
            scaleMe *= (1 + zoom_speed / 10);
	        transform.localScale = new Vector3(orX*scaleMe, orY*scaleMe, orZ*scaleMe);

		}
        
        if (Voxon.Input.GetSpaceNavButton("RightButton"))// && transform.localScale.x > min_scale)
        {
			// cam.BaseScale *= (1 - zoom_speed / 10);
			scaleMe *= (1 - zoom_speed / 10);
	        transform.localScale = new Vector3(orX*scaleMe, orY*scaleMe, orZ*scaleMe);
		}

		if (original_size == 0)
		{
			// original_size = cam.BaseScale;
			original_size = 0.1f;

		}

		if (original_pos == Vector3.zero)
		{
			original_pos = transform.position;
		}

		if (original_rot == Quaternion.identity)
		{
			original_rot = Quaternion.identity;
		}

		
		
		var rotation = Voxon.VXProcess.Runtime.GetSpaceNavRotation();

		if (rotation != null)
		{
			var v3rot = new Vector3(rotation[2] / 70, 0, 0);
			transform.Rotate(v3rot);
		}

		if (Voxon.Input.GetKey("RotateSideL"))
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 1, transform.eulerAngles.z);
		}
		if (Voxon.Input.GetKey("RotateSideR"))
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 1, transform.eulerAngles.z);
		}
		

		var position = Voxon.VXProcess.Runtime.GetSpaceNavPosition();
		var v3pos = transform.position;
		if (position != null)
		{
			v3pos.x += movement_speed * (position[0] / 35.0f);
			v3pos.y -= movement_speed * (position[2] / 35.0f);
			v3pos.z -= movement_speed * (position[1] / 35.0f);

			// float distance = Vector3.Distance(original_pos, v3pos);
			// if (distance > max_distance)
			// {
			// 	v3pos = Vector3.MoveTowards(v3pos, original_pos, distance - max_distance);
			// }
			transform.position = v3pos;
		}

		// this.transform.position = new Vector3(Mathf.Clamp(v3pos.x, -(float)viewFinderScale.x/2, (float)viewFinderScale.x/2), Mathf.Clamp(v3pos.y, -(float)viewFinderScale.y/2, (float)viewFinderScale.y/2), Mathf.Clamp(v3pos.z, -(float)viewFinderScale.z/2, (float)viewFinderScale.z/2));

		if (Voxon.Input.GetSpaceNavButton("LeftButton") && Voxon.Input.GetSpaceNavButton("RightButton"))
		{
			transform.position = original_pos;
			transform.rotation = original_rot;
			// transform.localScale = original_size;
			transform.localScale = new Vector3(original_size, original_size, original_size);
		}


		// if(state == -2)
		// {
		if (Voxon.Input.GetKeyDown("Down"))
		{
			movement_speed -= 0.005f;
			if(movement_speed <= 0.005f)
			{
				movement_speed = 0.005f;
			}
		}
		if (Voxon.Input.GetKeyDown("Up"))
		{
			movement_speed += 0.005f;
		}
		// }
	}
}
