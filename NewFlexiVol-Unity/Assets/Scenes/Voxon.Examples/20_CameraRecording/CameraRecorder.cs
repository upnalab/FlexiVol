using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._20_CameraRecorder
{
	/// <summary>
	/// Class to track and record camera movement through
	/// scene for later playback
	/// </summary>
	[RequireComponent(typeof(CameraAnimation))]
	public class CameraRecorder : MonoBehaviour
	{
		/// <summary>
		/// File to store recorded data within
		/// </summary>
		public string filename;
		/// <summary>
		/// If recording is currently active
		/// </summary>
		bool is_recording = false;

		/// <summary>
		/// Object to store recorded camera animation data within
		/// </summary>
		CameraAnimation camAn;

		/// <summary>
		/// Called on Start
		/// Initialise / Collect Camera Animation
		/// </summary>
		void Start()
		{
			camAn = GetComponent<CameraAnimation>();
		}

		/// <summary>
		/// Called per Frame
		/// Test Input:
		/// "Cam_Record" : 
		///		- If isn't recording, begins recording from frame 0
		///		- If is recording, stops recording
		/// </summary>
		void Update()
		{
			if (Voxon.Input.GetKeyDown("Cam_Record"))
			{
				if (!is_recording)
				{
					Debug.Log($"Camera Animation Recording: {filename}");
					camAn.recording_file = filename;
					camAn.SetFrame(0);
					camAn.BeginRecording();
				}
				else
				{
					Debug.Log($"Camera Animation Recording Stopped");
					camAn.StopRecording();
				}
				is_recording = !is_recording;
			}
		}
	}
}