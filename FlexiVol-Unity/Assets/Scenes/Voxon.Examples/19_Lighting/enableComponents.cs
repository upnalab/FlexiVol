using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._19_Lighting
{
	/// <summary>
	/// Toggle Between two game Objects. 
	/// Lit and Unlight, and turn light on or off
	/// depending on which is active
	/// </summary>
	public class enableComponents : MonoBehaviour
	{
		/// <summary>
		/// Object which can receive light
		/// </summary>
		public GameObject LitObject;
		/// <summary>
		/// Object which cannot receive light
		/// </summary>
		public GameObject UnlitObject;
		/// <summary>
		/// Where light information is stored
		/// </summary>
		public new lightbuffer light;

		/// <summary>
		/// Is the light currently enabled?
		/// </summary>
		new public bool enabled = false;
		
		/// <summary>
		/// Call on Start.
		/// Sets each component enabled or not based
		/// on <see cref="enabled"/>
		/// </summary>
		void Start()
		{
			Toggle();
		}

		/// <summary>
		/// Toggle <see cref="enabled"/> state and apply to objects
		/// </summary>
		void Toggle()
		{
			UnlitObject.SetActive(enabled);
			LitObject.SetActive(!enabled);
			light.enabled = !enabled;
		}

		/// <summary>
		/// Call per frame
		/// Add input for:
		/// "ToggleComponent" : Swaps enabled and applies
		/// </summary>
		void Update()
		{
			if (Voxon.Input.GetKeyDown("ToggleComponent"))
			{
				enabled = !enabled;
				Toggle();
			}
		}
	}
}