using UnityEngine;

namespace Voxon.Examples.Animation
{
	public class Rotating : MonoBehaviour {

		// Update is called once per frame
		void Update () {
			gameObject.transform.Rotate(new Vector3(0, Mathf.Sin(Time.time), 0));
		}
	}
}
