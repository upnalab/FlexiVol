using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._21_VCBRecording
{
	/// <summary>
	/// Class to call Runtime VCB (Volume Recording) behaviours.
	/// Recording will be performed with each frame having a frametime
	/// of 66.67~ milliseconds (fixed playback at 15 vps).
	/// </summary>
	public class VCBRecorder : MonoBehaviour
	{
		/// <summary>
		/// Destination VCB filename / path
		/// </summary>
		public string filename;
		/// <summary>
		/// Current recording status
		/// </summary>
		bool is_recording = false;


		/// <summary>
		/// Called per Frame
		/// Tests Input - 
		/// "VCBRecord":
		///		- If Recording, Stops recording
		///		- If not Records, Starts recording at fixed frame time (15 frame / second).
		/// </summary>
		void Update()
		{
			if (Voxon.Input.GetKeyDown("VCBRecord"))
			{
				if (!is_recording)
				{
					Debug.Log($"Recorder: {filename}");
					Voxon.VXProcess.Runtime.StartRecording(filename, 15);
				}
				else
				{
					Voxon.VXProcess.Runtime.EndRecording();
				}
				is_recording = !is_recording;
			}
		}
	}
}