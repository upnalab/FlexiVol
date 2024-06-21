using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._9_SpaceNav
{
    /// <summary>
    /// Displays Help Message to TouchScreen
    /// </summary>
    public class HelpMessage : MonoBehaviour
    {
        /// <summary>
        /// Call on Start
        /// Displays controls for Space Navigator on screen
        /// </summary>
        void Start()
        {
            VXProcess.add_log_line("");
            VXProcess.add_log_line("Shift model using Space Navigator");
            VXProcess.add_log_line("Zoom with left and right button");
        }
    }
}