using UnityEngine;

namespace Voxon.tests
{
	public class DrawTexture : MonoBehaviour {

		// Use this for initialization
		private void Start () {
			//gameObject.GetComponent<MeshRenderer>().material.mainTexture = TestObjects.tTexture();
			gameObject.GetComponent<MeshRenderer>().material = TestObjects.Material();
		}
	}
}
