using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._15_TouchPanelMenu
{
    /// <summary>
    /// Help message to be displayed in simulator or touch screen when
    /// example 15 is used.
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
            VXProcess.add_log_line("See [Test Menu] option.");
            VXProcess.add_log_line("This button demonstrates the use of sliders, buttons and editor boxes");
        }
    }
}