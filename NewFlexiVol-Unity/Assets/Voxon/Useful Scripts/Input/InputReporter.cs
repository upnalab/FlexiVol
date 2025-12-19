using UnityEngine;

namespace Voxon.Examples.Input
{
	public class InputReporter : MonoBehaviour {

		// Update is called once per frame
		void Update () {

			if (VXProcess.Instance.active)
			{
				int but = (int)InputController.GetButton("Jump", 1);
				for (int i = 0; i < 4; i++)
				{
					if (VXProcess.Runtime.GetButtonDown(but, i))
					{
						VXProcess.add_log_line("Player " + i);
					}
				}
			}
		}
	}
}
