using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._14_ColourSwap
{

    /// <summary>
    /// Swaps Voxon device Color Palette
    /// </summary>
    public class ColorModeSwap : MonoBehaviour
    {
        // Start is called before the first frame update
        /// <summary>
        /// Default color mode is RGB.
        /// </summary>
        public ColorMode currentColor = ColorMode.RGB;


        /// <summary>
        /// Called per frame.
        /// Will iterate through available color modes when "ColorSwap" key
        /// is pressed.
        /// </summary>
        void Update()
        {
            if (Voxon.Input.GetKeyDown("ColorSwap"))
            {
                currentColor--;
                if (currentColor < ColorMode.CYAN)
                {
                    currentColor = ColorMode.BG;
                }
                Debug.Log((currentColor.ToString()) + " : " + (int)currentColor);
                VXProcess.Runtime.SetDisplayColor(currentColor);
            }
        }
    }
}