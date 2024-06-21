using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._5_DeltaTimeText
{
	/// <summary>
	/// Report the current Frame Delta Time
	/// to a Voxon Text component on screen
	/// </summary>
	public class DeltaTimeText : MonoBehaviour
	{
		/// <summary>
		/// Voxon Text Component
		/// </summary>
		Voxon.VXTextComponent text;
		
		/// <summary>
		/// Called on Start.
		/// Get TextComponent
		/// </summary>
		void Start()
		{
			text = GetComponent<Voxon.VXTextComponent>();

			// text.forceUpdatePerFrame = false;
		}

		/// <summary>
		/// Called per frame.
		/// Updates the displayed text to current Time Delta
		/// </summary>
		void Update()
		{
			// If text component is rebuilding string per frame; we just set the value
			if (text.forceUpdatePerFrame)
			{
				text.text = Time.deltaTime.ToString();
			}
			else // Otherwise; we should call setString to make sure it's constructed properly
			{
				text.SetString(Time.deltaTime.ToString());
			}
		}
	}
}