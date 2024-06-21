using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._14_ColourSwap
{
    /// <summary>
    /// Help message to be displayed in simulator or touch screen when
    /// example 14 is used.
    /// </summary>
    public class HelpMessage : MonoBehaviour
    {
        /// <summary>
        /// Called on Start.
        /// Adds help information to Voxon Log
        /// </summary>
        void Start()
        {
            VXProcess.add_log_line("");
            VXProcess.add_log_line("Cycle between color modes with the <SpaceBar>");
            VXProcess.add_log_line("Use the Voxon menu to confirm color mode changes.");
        }
    }
}