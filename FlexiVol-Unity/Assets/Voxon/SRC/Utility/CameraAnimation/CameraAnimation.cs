using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
	/// <summary
	/// Notes: Camera transformation utilises a mix of linear and bezier curves
	/// to provide the director with increased control over the camera movement.
	/// Camera animation is based on per frame (due to possible recording delays)
	/// Thus it is possible to reference a specific frame and determine between
	/// which key frames the frame occurs within and then determine the correct
	/// matrix which should be applied to the camera.
	/// </summary>

	bool is_playing = false;
	bool is_recording = false;
	

	int active_frame = 0;
	//int spline_len = 0;

	// Need to consider frames as Ranges; a start and stop point
	// Key frames determine start and type (including that types parameters)
	// End is determined by the next key frame.
	// When recording new frames are compared to the  previous frame. 
	// If they are the same, the new frame is not considered important and skipped
	// this means that during recording all frames as linear by default
	// they can be converted to bezier by editing afterwards
	bool is_playback_loaded = false;

	public bool play_on_load = false;
	public bool record_on_load = false;

	public string playback_file = "";
	public string recording_file = "";

	// TEMPORARY
	// Two distinct Objects which Camera Animation System Interacts with.
	// Need to support contiguous unchanged frames else we'll be overrun with data
	public CameraTimeLine playback_frames = null;

	CameraTimeLine saved_frames = null;

	public void Start()
	{
		if (play_on_load)
		{
			is_playing = true;
		}
	}

	public bool BeginRecording()
	{
		if(recording_file != "")
		{
			saved_frames = new CameraTimeLine();
			is_recording = true;
		}

		return is_recording;
	}

	public bool StopRecording()
	{
		if (is_recording)
		{
			is_recording = false;
		}

		return !is_recording;
	}

	public bool LoadPlaybackFile()
	{
		Debug.Log("Loading Playback Files");

		if (playback_file == "")
		{
			Debug.LogWarning("No playback file name provided");
			playback_frames = new CameraTimeLine();
			return false;
		}

		string filePath = Path.Combine(Application.streamingAssetsPath, "CameraAnimations", playback_file);

		if (!File.Exists(filePath))
		{
			Debug.LogError($"Error: File does not exist. {filePath}");
			return false;
		}

		string jsonString = File.ReadAllText(filePath);

		playback_frames = new CameraTimeLine();
		JsonUtility.FromJsonOverwrite(jsonString, playback_frames);

		is_playback_loaded = true;
		return true;

	}

	public bool SaveEditedPlaybackFile()
	{
		if (playback_frames == null || playback_frames.instants.Count == 0)
		{
			return false;
		}

		try
		{
			string msg = playback_frames.ToJSON();
			// Debug.Log(msg);

			if (recording_file == "")
			{
				recording_file = "saved_frames.json";
			}
			if (!recording_file.EndsWith(".json"))
			{
				recording_file += ".json";
			}

			string filePath = Path.Combine(Application.streamingAssetsPath, "CameraAnimations", recording_file);

			if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "CameraAnimations")))
			{
				System.IO.FileInfo file = new System.IO.FileInfo(filePath);
				file.Directory.Create();
			}

			File.WriteAllText(filePath, msg);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			return false;
		}

		return true;
	}

	public bool SaveRecording()
	{
		if (saved_frames == null || saved_frames.instants.Count == 0)
		{
			return false;
		}

		try
		{
			string msg = saved_frames.ToJSON();
			// Debug.Log(msg);

			if (recording_file == "")
			{
				recording_file = "saved_frames.json";
			}
			if (!recording_file.EndsWith(".json"))
			{
				recording_file += ".json";
			}

			string filePath = Path.Combine(Application.streamingAssetsPath, "CameraAnimations", recording_file);

			if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "CameraAnimations")))
			{
				FileInfo file = new FileInfo(filePath);
				file.Directory.Create();
			}

			File.WriteAllText(filePath, msg);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			return false;
		}


		return true;
	}

	public int GetAnimationLength()
	{
		if(playback_frames != null)
		{
			return playback_frames.AnimationLength;
		} else
		{
			return 0;
		}
	}

	// Do we want to be able to set out of range frames?
	public bool SetFrame(int frame)
	{
		active_frame = frame;

		return false;
	}

	public void IncrementFrame()
	{
		active_frame++;
	}

	public int GetFrameIndex()
	{
		return active_frame;
	}

	public int GetFrame()
	{
		if(playback_frames == null || playback_frames.AnimationLength < 1)
		{
			return 0;
		} else
		{
			return playback_frames.GetFrame(active_frame);
		}
	}

	public CameraInstant GetFrameTransform()
	{
		// Need a system to handle out of range frames
		return playback_frames.GetInstant(active_frame);
	}

	public void LoadTransform(VXCamera camera)
	{
		if(!is_playing || playback_file == "")
		{
			return;
		}

		// < - If we're in playback mode but no file is loaded - >
		if(!is_playback_loaded)
		{
			// we want playback to have something atleast
			LoadPlaybackFile();
		}

		if(!is_playback_loaded)
		{
			Debug.Log($"Frames not loaded! Skipping");
			return;
		}

		try { 
			if (active_frame < playback_frames.FinalFrame)
			{
				CameraInstant ci = GetFrameTransform();
				camera.transform.position = ci._Position;
				camera.transform.rotation = ci._Rotation;
				camera.ViewFinderDimensions = ci._ViewFinderDimensions;
				camera.BaseScale = ci._BaseScale;

				active_frame++;
			}
		} catch (Exception E){
			Debug.Log(E.Message);
			is_playing = false;
		}
	}

	public void SaveTransform(Transform camTransform, float camScale, Vector3 camViewRatio)
	{
		if(active_frame == 0 && record_on_load)
		{
			BeginRecording();
		}

		if (is_recording)
		{
			// Add camera transform to listing
			saved_frames.AddInstant(new CameraInstant(active_frame++, camTransform.position, camTransform.rotation, camScale, camViewRatio));
		}
	}

	public bool BeginPlayback()
	{
		if(playback_file != "")
		{
			LoadPlaybackFile();
		   // Load Playback file into playback_frames
		   is_playing = true;
		}

		return is_playing;
	}

	public bool StopPlayback()
	{
		if (is_playing)
		{
			is_playing = false;
		}

		return !is_playing;
	}

	public bool isRecording()
	{
		return is_recording;
	}

	public bool isPlaying()
	{
		return is_playing;
	}
}
