using UnityEngine;

namespace Voxon.Examples.Animation
{
	public class MoveText : MonoBehaviour {
		private int _step;

		// Update is called once per frame
		private void Update () {
			if(_step > 300)
			{
				GameObject o;
				(o = gameObject).transform.position = new Vector3(Random.Range(-4.9f, 4.9f), Random.Range(-1.9f, 1.9f), Random.Range(-3.9f, 3.9f));
				var position = o.transform.position;
				Vector3 pos = position;
				pos.x += 0.5f;
				position = pos;
				o.transform.position = position;
				Debug.Log($"Base Location: {position}");
				gameObject.GetComponent<VXTextComponent>().UpdateLocation();
				_step = 0;
			}
			_step++;
	    
		}
	}
}
