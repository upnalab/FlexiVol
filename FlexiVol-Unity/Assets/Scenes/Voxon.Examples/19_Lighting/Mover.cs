using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._19_Lighting
{
	/// <summary>
	/// Add gameObject postion movement controls
	/// </summary>
	public class Mover : MonoBehaviour
	{
		/// <summary>
		/// Initial position of game object
		/// </summary>
		Vector3 initial_position;
		
		/// <summary>
		/// Call on Start.
		/// Set gameObject initial position
		/// </summary>
		void Start()
		{
			initial_position = transform.position;
		}

		/// <summary>
		/// Call per Frame.
		/// Adds the following controls:
		/// "Left"	: Move position +ve in x
		/// "Right"	: Move position -ve in x
		/// "Up"	: Move position +ve in z
		/// "Down"	: Move position -ve in z
		/// "RotLeft"	: Move position +ve in y
		/// "RotRight"	: Move position -ve in y
		/// "ToggleComponent": Return to initial position
		/// </summary>
		void Update()
		{
			if (Voxon.Input.GetKey("Left"))
			{
				transform.position += new Vector3(0.1f, 0, 0);
			}

			if (Voxon.Input.GetKey("Right"))
			{
				transform.position -= new Vector3(0.1f, 0, 0);
			}

			if (Voxon.Input.GetKey("Up"))
			{
				transform.position += new Vector3(0, 0, 00.1f);
			}

			if (Voxon.Input.GetKey("Down"))
			{
				transform.position -= new Vector3(0, 0, 00.1f);
			}

			if (Voxon.Input.GetKey("RotLeft"))
			{
				transform.position += new Vector3(0, 0.1f, 0);
			}

			if (Voxon.Input.GetKey("RotRight"))
			{
				transform.position -= new Vector3(0, 0.1f, 0);
			}

			if (Voxon.Input.GetKey("ToggleComponent"))
			{
				transform.position = initial_position;
			}
		}
	}
}