using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Voxon.Examples._7_LevelLoading
{
	/// <summary>
	/// Loads a named level after a delay
	/// </summary>
	public class LoadLevel : MonoBehaviour
	{
		/// <summary>
		/// Level to be Loaded
		/// </summary>
		[FormerlySerializedAs("LevelName")] public string levelName;

		/// <summary>
		/// How long to wait until loading new level
		/// </summary>
		[FormerlySerializedAs("LoadTime")] [Tooltip("Seconds")]
		public float loadTime;
		
		/// <summary>
		/// Called on Start
		/// Begins delayed load action
		/// </summary>
		private void Start()
		{
			Invoke(nameof(DelayedStart),loadTime);
		}

		/// <summary>
		/// Delayed Load Action
		/// </summary>
		private void DelayedStart()
		{
			StartCoroutine(routine: LoadScene());
		}

		/// <summary>
		/// Loads a scene asyncronously
		/// </summary>
		/// <returns><c>null</c> while level is loading</returns>
		private IEnumerator LoadScene()
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

			// Wait until the asynchronous scene fully loads
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}
}
